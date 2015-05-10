//-----------------------------------------------------------------------
// <copyright file="CommunityController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the Community page request which makes request to repository and get the
    /// required data and pushes them to the View.
    /// </summary>
    public class CommunityController : ControllerBase
    {
        #region Private Variables

        /// <summary>
        /// Instance of community Service
        /// </summary>
        private ICommunityService communityService;

        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService notificationService;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CommunityController class.
        /// </summary>
        /// <param name="communityService">Instance of community Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public CommunityController(ICommunityService communityService, IProfileService profileService, INotificationService queueService)
            : base(profileService)
        {
            this.communityService = communityService;
            this.notificationService = queueService;
        }

        #endregion Constructor

        #region Action Methods

        /// <summary>
        /// Index Action which is default action rendering the home page.
        /// </summary>
        /// <param name="id">Community id to be processed</param>
        /// <returns>Returns the View to be used</returns>
        
        [HttpPost]
        [Route("Community/Get/Detail")]
        public JsonResult Index(long? id)
        {
            
            CommunityViewModel communityViewModel = null;
            CommunityDetails communityDetails = this.communityService.GetCommunityDetails(id.Value, this.CurrentUserID, true, true);

            communityViewModel = new CommunityViewModel();

            Mapper.Map(communityDetails, communityViewModel);

            // Extract only the text from the Description HTML which needs to be used in Mail sharing. HTML tags which are meant for
            // styling the description is not required.
            string description = communityViewModel.Description.GetTextFromHtmlString();
            description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Length > 100 ? description.Substring(0, 97) + "..." : description;

            // This should not be there in the extension method since it is needed only in cases where description is sent as query string.
            description = description.Replace("&", "%26");

            if (communityViewModel.AccessType != AccessType.Private || communityViewModel.UserPermission >= WWTMVC5.Permission.Reader)
            {
                return Json(communityViewModel);
            }
            else
            {

                return new JsonResult
                {
                    Data = new {communityViewModel.Name, communityViewModel.Id, error = "insufficient permission"}
                };
            }
        }

        [HttpPost]
        [Route("Community/Contents/{communityId}")]
        public JsonResult GetCommunityContents(long communityId)
        {
            var contents = communityService.GetCommunityContents(communityId, CurrentUserID);
            var entities = new List<EntityViewModel>();
            foreach (var item in contents)
            {
                ContentViewModel contentViewModel = new ContentViewModel();
                contentViewModel.SetValuesFrom(item);
                entities.Add(contentViewModel);
            }
            var children = communityService.GetChildCommunities(communityId, CurrentUserID);
            var childCommunities = new List<CommunityViewModel>();
            foreach (var child in children)
            {
                var communityViewModel = new CommunityViewModel();
                Mapper.Map(child, communityViewModel);
                childCommunities.Add(communityViewModel);
            }
            return Json(new{entities,childCommunities});

        }

        /// <summary>
        /// Controller action which gets the details about the new community.
        /// </summary>
        /// <param name="id">Parent Community/Folder ID, will be passed only while creating sub community or folder</param>
        /// <param name="isFolder">Whether New action is called for community or folder, will be passed only while creating sub community or sub folder</param>
        /// <returns>View having page which gets details about new community</returns>
        [HttpGet]
        public ActionResult New(long? id, bool? isFolder)
        {
            CommunityInputViewModel communityInputViewModel = new CommunityInputViewModel();

            // Populating the category dropdown list.
            communityInputViewModel.CategoryList = CategoryType.All.ToSelectList(CategoryType.All);
            communityInputViewModel.CategoryID = (int)CategoryType.GeneralInterest;

            // Populating the parent communities for the current user. Pass -1 as current community while creating new communities
            // since there are not current community which needs to be ignored.
            // TODO: Need to show the parent communities/folders in tree view dropdown.
            IEnumerable<Community> parentCommunities = this.communityService.GetParentCommunities(-1, CurrentUserID);
            communityInputViewModel.ParentList = new SelectList(parentCommunities, "CommunityID", "Name");

            // Default creation is community.
            communityInputViewModel.CommunityType = CommunityTypes.Community;

            // Default access type is public (2).
            communityInputViewModel.AccessTypeID = 2;

            // Set the thumbnail URL.
            communityInputViewModel.ThumbnailLink = Url.Content("~/content/images/default" + (isFolder == null || isFolder == false ? "community" : "folder") + "thumbnail.png");

            if (isFolder.HasValue && isFolder.Value)
            {
                communityInputViewModel.CommunityType = CommunityTypes.Folder;
            }

            if (id.HasValue)
            {
                communityInputViewModel.ParentID = id.Value;

                // Set the Category, Distributed by and Tags from parent
                Community parentCommunity = parentCommunities.FirstOrDefault(community => community.CommunityID == id);
                if (parentCommunity != null)
                {
                    communityInputViewModel.CategoryID = parentCommunity.CategoryID;
                    communityInputViewModel.DistributedBy = parentCommunity.DistributedBy;
                }
            }

            return View("Save", communityInputViewModel);
        }

        /// <summary>
        /// Controller action which inserts a new community to the Layerscape database.
        /// </summary>
        /// <param name="communityJson">ViewModel holding the details about the community</param>
        /// <returns>Returns a redirection view</returns>
        [HttpPost]
        [Route("Community/Create/New")]
        public ActionResult New(CommunityInputViewModel communityJson)
        {
            //// Make sure communityJson is not null
            //this.CheckNotNull(() => new { communityInputViewModel = communityJson });

            // Populating the category dropdown list.
            //communityJson.CategoryList = CategoryType.All.ToSelectList(CategoryType.All);

            // Populating the parent communities for the current user. Pass -1 as current community while creating new communities
            // since there are not current community which needs to be ignored.
            // TODO: Need to show the parent communities/folders in tree view dropdown.
            //IEnumerable<Community> parentCommunities = this.communityService.GetParentCommunities(-1, CurrentUserID);
            //communityJson.ParentList = new SelectList(parentCommunities, "CommunityID", "Name");

            if (ModelState.IsValid)
            {
                CommunityDetails communityDetails = new CommunityDetails();
                Mapper.Map(communityJson, communityDetails);

                // Set thumbnail properties
                communityDetails.Thumbnail = new FileDetail() { AzureID = communityJson.ThumbnailID };

                communityDetails.CreatedByID = CurrentUserID;

                communityJson.ID = communityDetails.ID = this.communityService.CreateCommunity(communityDetails);

                // Send Notification Mail
                this.notificationService.NotifyNewEntityRequest(communityDetails, HttpContext.Request.Url.GetServerLink());

                return new JsonResult { Data = new { ID = communityDetails.ID } };
            }
            else
            {
                // In case of any validation error stay in the same page.
                return new JsonResult { Data = false };
            }
        }

        /// <summary>
        /// Gets the parent community details from EarthOnline database
        /// </summary>
        /// <param name="id">Id of the parent community</param>
        /// <returns>Json community object</returns>
        [HttpGet]
        
        public JsonResult ParentCommunity(long id)
        {
            CommunityDetails parentCommunity = this.communityService.GetCommunityDetails(id, this.CurrentUserID);
            return Json(parentCommunity, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Controller action which gets the details about the community which is being edited.
        /// </summary>
        /// <param name="id">Id of the community getting edited.</param>
        /// <returns>View having page which gets details about community getting updated</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Community/Service/Edit")]
        public ActionResult Edit(long? id)
        {
            CommunityInputViewModel communityInputViewModel = new CommunityInputViewModel();

            // Populating the category dropdown list.
            communityInputViewModel.CategoryList = CategoryType.All.ToSelectList(CategoryType.All);

            // Populating the parent communities for the current user.
            // TODO: Need to show the parent communities/folders in tree view dropdown.
            IEnumerable<Community> parentCommunities = this.communityService.GetParentCommunities(id ?? -1, CurrentUserID);

            communityInputViewModel.ParentList = new SelectList(parentCommunities, "CommunityID", "Name");

            if (id.HasValue)
            {
                CommunityDetails communityDetails = this.communityService.GetCommunityDetailsForEdit(id.Value, this.CurrentUserID);

                // Make sure communitieView is not null
                this.CheckNotNull(() => new { communityDetails });

                if (communityDetails.CommunityType == CommunityTypes.User)
                {
                    // Community of type user should not be allowed to edit. Set it to null , so that user will get
                    // item not found message.
                    communityDetails = null;
                }
                
                Mapper.Map(communityDetails, communityInputViewModel);

                // Set Thumbnail URL.
                if (communityInputViewModel.ThumbnailID != Guid.Empty)
                {
                    communityInputViewModel.ThumbnailLink = Url.Action("Thumbnail", "File", new { id = communityInputViewModel.ThumbnailID });
                }
                else
                {
                    communityInputViewModel.ThumbnailLink = Url.Content("~/content/images/default" + (communityDetails.CommunityType == CommunityTypes.Community ? "community" : "folder") + "thumbnail.png");
                }
            }

            return View("Save", communityInputViewModel);
        }

        /// <summary>
        /// Controller action which updates the details in Layerscape database about the community which is being edited.
        /// </summary>
        /// <param name="communityInputViewModel">ViewModel holding the details about the community</param>
        /// <returns>Returns a redirection view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Edit(CommunityInputViewModel communityInputViewModel)
        {
            // Make sure communityInputViewModel is not null
            this.CheckNotNull(() => new { communityInputViewModel });

            // Populating the category dropdown list.
            communityInputViewModel.CategoryList = CategoryType.All.ToSelectList(CategoryType.All);

            // Populating the parent communities for the current user.
            // TODO: Need to show the parent communities/folders in tree view dropdown.
            IEnumerable<Community> parentCommunities = this.communityService.GetParentCommunities(communityInputViewModel.ID.Value, CurrentUserID);
            communityInputViewModel.ParentList = new SelectList(parentCommunities, "CommunityID", "Name");

            if (ModelState.IsValid)
            {
                CommunityDetails communityDetails = new CommunityDetails();
                Mapper.Map(communityInputViewModel, communityDetails);

                // Set thumbnail properties
                communityDetails.Thumbnail = new FileDetail() { AzureID = communityInputViewModel.ThumbnailID };

                // TODO : Better way to do this. 
                // Get the ID from Session
                // Centralize the permission check for entities at one place (ProfileService)
                var identity = HttpContext.GetIdentityName();
                if (!string.IsNullOrWhiteSpace(identity) && communityInputViewModel.ID.HasValue)
                {
                    this.communityService.UpdateCommunity(communityDetails, this.CurrentUserID);
                }

                return RedirectToAction("Index", new { id = communityInputViewModel.ID });
            }
            else
            {
                // In case of any validation error stay in the same page.
                return View("Save", communityInputViewModel);
            }
        }

        /// <summary>
        /// Deletes the specified community from the Layerscape database.
        /// </summary>
        /// <param name="id">Id of the community to be deleted.</param>
        /// <param name="parentId">Parent entity to which user to be take after delete</param>
        /// <returns>Returns an action url</returns>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "TODO: Custom Exception handling to be added."), HttpPost]
        [ValidateAntiForgeryToken]
        
        public string Delete(long? id, long? parentId, string fromWhere)
        {
            var returnActionUrl = string.Empty;
            if (id.HasValue)
            {
                var identity = HttpContext.GetIdentityName();
                if (!string.IsNullOrWhiteSpace(identity))
                {
                    OperationStatus status = this.communityService.DeleteCommunity(id.Value, this.CurrentUserID);

                    // TODO: Need to add failure functionality.
                    if (!status.Succeeded)
                    {
                        throw new Exception(status.ErrorMessage, status.Exception);
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(fromWhere))
            {
                if (parentId.HasValue)
                {
                    returnActionUrl = Url.Action("Index/" + parentId, "Community");
                }
                else
                {
                    returnActionUrl = Url.Action("Index", "Home");
                }
            }

            return returnActionUrl;
        }

        /// <summary>
        /// Signup action to get the sign up file for the community id
        /// </summary>
        /// <param name="id">Community id</param>
        /// <returns>sign up Xml</returns>
        [HttpGet]
        public ActionResult Signup(long? id)
        {
            if (id.HasValue)
            {
                var communityDetails = this.communityService.GetCommunityDetails(id.Value, this.CurrentUserID);

                // Make sure communityDetails is not null
                this.CheckNotNull(() => new { communityDetails });
                if (communityDetails.CommunityType == CommunityTypes.Community || communityDetails.CommunityType == CommunityTypes.Folder)
                {
                    var signupDetails = new SignUpDetails();
                    Mapper.Map(communityDetails, signupDetails);

                    if (communityDetails.CommunityType == CommunityTypes.Community)
                    {
                        // Check Mock ability of HttpContext and Url while writing unit tests for Signup action
                        signupDetails.Url = Request.GetRootPath() + "/ResourceService/Community/" + id.Value;

                        // Set the thumbnail path. In case if no thumbnail is specified, set the default thumbnail path.
                        signupDetails.Thumbnail = this.RewriteThumbnailUrl(signupDetails.Thumbnail, "defaultcommunitywwtthumbnail");
                    }
                    else
                    {
                        // Check Mock ability of HttpContext and Url while writing unit tests for Signup action
                        signupDetails.Url = Request.GetRootPath() + "/ResourceService/Folder/" + id.Value;

                        // Set the thumbnail path. In case if no thumbnail is specified, set the default thumbnail path.
                        signupDetails.Thumbnail = this.RewriteThumbnailUrl(signupDetails.Thumbnail, "defaultfolderwwtthumbnail");
                    }

                    signupDetails.MSRCommunityId = id.Value; 

                    string signUpFileName = string.Format(CultureInfo.InvariantCulture, Constants.SignUpFileNameFormat, communityDetails.Name);
                    Response.AddHeader("Content-Encoding", "application/xml");
                    Response.AddHeader("content-disposition", "attachment;filename=" + signUpFileName);
                    var stream = signupDetails.GetXmlStream();
                    stream.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(stream, "application/xml");
                }
            }

            return null;
        }

        /// <summary>
        /// When user clicks on the edit permission, it returns the permission view
        /// </summary>
        /// <param name="id">Community Id</param>
        /// <param name="name">Name of the community</param>
        /// <param name="request">Permission to decide whether requests are needed or not</param>
        /// <returns>Returns the permission view</returns>
        [HttpGet]
        
        public ActionResult Permission(long id, string name, Permission request)
        {
            this.CheckNotNull(() => new { request });

            PermissionBaseViewModel permission = new PermissionBaseViewModel();
            permission.Id = id;

            if (!string.IsNullOrWhiteSpace(name))
            {
                // Special case for apostrophe (') because HtmlEncoded string(containing ') gives error.
                permission.Name = name.Replace("%27", "'");
            }

            // This is needed only for showing the requests tab.
            permission.Role = request.CanSetOwnerPermits() ? UserRole.Owner : (request.CanSetPermits() ? UserRole.Moderator : UserRole.Contributor);
            return View(permission);
        }

        /// <summary>
        /// It returns the permission list view on the basis of user type
        /// </summary>
        /// <param name="communityID">Community Id</param>
        /// <param name="permissionsTab">Either user/requestor</param>
        [ChildActionOnly]
        
        public void RenderPermission(long? communityID, PermissionsTab permissionsTab)
        {
            try
            {
                PartialView("PermissionListView", GetUserPermissionDetails(communityID, permissionsTab, 1)).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }



        /// <summary>
        /// It returns the permission list view on the basis of user type
        /// </summary>
        /// <param name="entityId">Community Id</param>
        /// <param name="permissionsTab">Either user / requestor</param>
        /// <param name="currentPage">Current page to be rendered</param>
        [HttpPost]
        [Route("Community/Permission/{permissionsTab}/{currentPage}")]
        public JsonResult AjaxRenderPermission(long? entityId, PermissionsTab permissionsTab, int currentPage)
        {
            var details =  GetUserPermissionDetails(entityId, permissionsTab, currentPage);
            return Json(details);
        }

        /// <summary>
        /// It returns the invite requests list view for the given community
        /// </summary>
        /// <param name="communityID">Community Id</param>
        [ChildActionOnly]
        
        public void RenderInviteRequests(long communityID)
        {
            try
            {
                PartialView("InviteRequestListView", GetInviteRequests(communityID, 1)).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// It returns the permission list view on the basis of user type
        /// </summary>
        /// <param name="entityId">Community Id</param>
        /// <param name="currentPage">Current page to be rendered</param>
        [HttpPost]
        
        public void AjaxRenderInviteRequests(long entityId, int currentPage)
        {
            try
            {
                PartialView("InviteRequestListView", GetInviteRequests(entityId, currentPage)).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Updates the user permission request for the specified user and community
        /// </summary>
        /// <param name="entityId">Community Id</param>
        /// <param name="requestorId">Requestor Id</param>
        /// <param name="userRole"> Permission Type (Owner, contributor, reader etc.)</param>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public void UpdateUserRoles(long entityId, long requestorId, UserRole userRole, PermissionsTab permissionsTab)
        {
            try
            {
                PermissionItem permission = new PermissionItem();
                permission.UserID = requestorId;
                permission.CommunityID = entityId;
                permission.Role = userRole;

                OperationStatus operationStatus = this.ProfileService.UpdateUserRoles(permission, this.CurrentUserID);

                if (operationStatus.Succeeded)
                {
                    PermissionViewModel permissionViewModel = null;

                    // In case of leave community (PermissionsTab.None), should not get the user permission details.
                    if (permissionsTab != PermissionsTab.None)
                    {
                        permissionViewModel = GetUserPermissionDetails(entityId, PermissionsTab.Users, 1);
                    }

                    if (requestorId != this.CurrentUserID)
                    {
                        if (userRole == UserRole.None)
                        {
                            SendRemoveUserMail(permission);
                        }
                        else
                        {
                            // TODO: Do we need to send mail even if the role is not changed but Save button is clicked?
                            SendChangedUserRoleMail(permission);
                        } 
                    }

                    PartialView("PermissionListView", permissionViewModel).ExecuteResult(this.ControllerContext);
                }
                else
                {
                    Json(operationStatus).ExecuteResult(this.ControllerContext);
                }
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Updates the user permission request for the specified user and community
        /// </summary>
        /// <param name="entityId">Community Id</param>
        /// <param name="requestorId">Requestor Id</param>
        /// <param name="userRole"> Permission Type (Owner, contributor, reader etc.)</param>
        /// <param name="permissionsTab">User / Requestor</param>
        /// <param name="actionType">Accept / Decline</param>
        /// <param name="currentPage">Current page from where action performed</param>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public void UpdateUserPermissionRequest(long entityId, long requestorId, UserRole userRole, PermissionsTab permissionsTab, string actionType, int currentPage)
        {
            try
            {
                PermissionItem permission = new PermissionItem();
                permission.UserID = requestorId;
                permission.CommunityID = entityId;
                permission.Role = userRole;
                permission.Approved = "approve" == actionType ? true : false;

                OperationStatus operationStatus = this.ProfileService.UpdateUserPermissionRequest(permission, this.CurrentUserID);

                if (operationStatus.Succeeded)
                {
                    SendProcessingStatusMail(permission);

                    PartialView("PermissionListView", GetUserPermissionDetails(entityId, permissionsTab, currentPage)).ExecuteResult(this.ControllerContext);
                }
                else
                {
                    Json(operationStatus).ExecuteResult(this.ControllerContext);
                }
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Dummy method which checks whether user is authorized to perform the Ajax requests or not. This is 
        /// used in FlagPage and JoinCommunity Ajax methods.
        /// </summary>
        [HttpGet]
        
        public JsonResult CheckAuthorization()
        {
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Return success / failure
        /// </summary>
        /// <param name="communityId">Community Id</param>
        /// <param name="comments">Comments inserted by requestor</param>
        /// <param name="userRole">Permission type</param>
        /// <returns>Inserts the request of join</returns>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public string Join(long communityId, string comments, UserRole userRole)
        {
            string ajaxResponse = "Success";
            PermissionItem permission = new PermissionItem();
            permission.UserID = this.CurrentUserID;
            permission.CommunityID = communityId;
            permission.Role = userRole;
            permission.Comment = Server.UrlDecode(comments);
            OperationStatus operationStatus = this.ProfileService.JoinCommunity(permission);

            if (operationStatus.Succeeded)
            {
                SendJoinCommunityMail(permission);
            }
            else if (operationStatus.CustomErrorMessage)
            {
                ajaxResponse = operationStatus.ErrorMessage;
            }

            return ajaxResponse;
        }

        /// <summary>
        /// Joins the current user to community for which the invite request token was generated.
        /// </summary>
        /// <param name="id">Id of the community to which user is joining</param>
        /// <param name="inviteRequestToken">Token to be used for joining the community</param>
        /// <returns>Ajax response string based on operation status</returns>
        [HttpGet]
        
        public ActionResult Join(long id, Guid inviteRequestToken)
        {
            OperationStatus operationStatus = this.ProfileService.JoinCommunity(this.CurrentUserID, inviteRequestToken);

            if (operationStatus.Succeeded)
            {
                TempData["ShowNotice"] = true;
                return RedirectToAction("Index", new { id = id });
            }
            else if (operationStatus.CustomErrorMessage)
            {
                TempData["ErrorMessage"] = operationStatus.ErrorMessage;
                return RedirectToAction("General", "Error");
            }
            else
            {
                return RedirectToAction("General", "Error");
            }
        }

        /// <summary>
        /// Invites the people with specified permissions for the current community.
        /// Return success / failure based on service method call.
        /// </summary>
        /// <param name="communityId">Community id for which invite is being sent</param>
        /// <param name="communityName">Name of the community</param>
        /// <param name="emailids">Email ids of the users to whom the invite has to be sent</param>
        /// <param name="subject">Subject of the invite request</param>
        /// <param name="userRole">Permission type</param>
        /// <param name="body">Body of the invite request</param>
        /// <returns>Ajax response string</returns>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public string InvitePeople(long communityId, string communityName, string emailids, string subject, UserRole userRole, string body)
        {
            string ajaxResponse = string.Empty;

            try
            {
                InviteRequestItem inviteRequestItem = new InviteRequestItem();
                inviteRequestItem.CommunityID = communityId;

                if (!string.IsNullOrWhiteSpace(emailids))
                {
                    emailids.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(emailId => inviteRequestItem.EmailIdList.Add(emailId.Trim()));
                }

                inviteRequestItem.Subject = Server.UrlDecode(subject);
                
                // Since this is Rich Text content, need to replace the line breaks with <br />
                inviteRequestItem.Body = Server.UrlDecode(body);

                inviteRequestItem.RoleID = (int)userRole;
                inviteRequestItem.InvitedByID = this.CurrentUserID;

                IEnumerable<InviteRequestItem> invitedPeople = this.communityService.InvitePeople(inviteRequestItem);
                SendInviteRequestMail(invitedPeople, Server.UrlDecode(communityName));
                ajaxResponse = Resources.InviteSuccessMessage;
            }
            catch (Exception)
            {
                ajaxResponse = Resources.UnknownErrorMessage;
            }

            return ajaxResponse;
        }

        /// <summary>
        /// Removes the selected Invite for the current community.
        /// Return success / failure based on service method call.
        /// </summary>
        /// <param name="inviteRequestID">Community id for which invite is being sent</param>
        /// <param name="communityID">Community id for which invite request is being removed</param>
        /// <param name="currentPage">Current page from where action performed</param>
        /// <returns>Ajax response string</returns>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public string RemoveInviteRequest(int inviteRequestID, long communityID, int currentPage)
        {
            string ajaxResponse = string.Empty;

            OperationStatus operationStatus = this.ProfileService.RemoveInviteRequest(this.CurrentUserID, inviteRequestID);

            if (operationStatus.Succeeded)
            {
                PartialView("InviteRequestListView", GetInviteRequests(communityID, currentPage)).ExecuteResult(this.ControllerContext);
            }
            else if (operationStatus.CustomErrorMessage)
            {
                ajaxResponse = operationStatus.ErrorMessage;
            }

            return ajaxResponse;
        }

        #endregion Action Methods

        #region Private Methods

        /// <summary>
        /// Rewrites the thumbnail URL for the payload XML. In case if the thumbnail is provided, proper URL to access the thumbnail 
        /// using the File controller and thumbnail action will be formed and returned.
        /// In case the thumbnail is not provided and default image is provided, then default thumbnail URL will be formed and returned.
        /// In other cases (both thumbnail and default image is not provided), same thumbnail will be returned.
        /// </summary>
        /// <param name="thumbnail">Thumbnail string to be verified</param>
        /// <param name="defaultImage">Default image name</param>
        /// <returns>Rewritten thumbnail URL</returns>
        private string RewriteThumbnailUrl(string thumbnail, string defaultImage)
        {
            if (!string.IsNullOrWhiteSpace(thumbnail))
            {
                thumbnail = Request.GetRootPath() + "/ResourceService/Thumbnail/" + thumbnail; 
            }
            else if (!string.IsNullOrWhiteSpace(defaultImage))
            {
                thumbnail = Request.GetRootPath() + Url.Content(string.Format(CultureInfo.InvariantCulture, "~/Content/Images/{0}.png", defaultImage));
            }

            return thumbnail;
        }

        /// <summary>
        /// Gets the user permission details for the given community
        /// </summary>
        /// <param name="communityID">Community for which permission details to be fetched</param>
        /// <param name="permissionsTab">Permission tab (Users/Requests) for which data to be fetched</param>
        /// <param name="currentPage">Current page to be rendered</param>
        /// <returns>ViewModel with permission details</returns>
        private PermissionViewModel GetUserPermissionDetails(long? communityID, PermissionsTab permissionsTab, int currentPage)
        {
            PageDetails pageDetails = new PageDetails(currentPage);
            pageDetails.ItemsPerPage = Constants.PermissionsPerPage;

            PermissionDetails permissionDetails = null;

            if (permissionsTab == PermissionsTab.Users)
            {
                permissionDetails = this.ProfileService.GetUserPemissions(this.CurrentUserID, communityID.Value, pageDetails);
            }
            else if (permissionsTab == PermissionsTab.Requests)
            {
                permissionDetails = this.ProfileService.GetUserPemissionRequests(this.CurrentUserID, communityID, pageDetails);
            }
            else
            {
                permissionDetails = this.ProfileService.GetUserPemissionRequests(this.CurrentUserID, null, pageDetails);
            }

            this.CheckNotNull(() => new { permissionDetails });

            // Check if there is only one owner for the current community.
            bool singleOwner = permissionDetails.PermissionItemList.Where(p => p.Role == UserRole.Owner).Count() == 1;

            List<PermissionDetailsViewModel> permissionList = new List<PermissionDetailsViewModel>();
            for (var i = 0; i < permissionDetails.PermissionItemList.Count; i++)
            {
                PermissionDetailsViewModel model = new PermissionDetailsViewModel()
                {
                    Id = permissionDetails.PermissionItemList[i].UserID,
                    Name = permissionDetails.PermissionItemList[i].Name,
                    CommunityId = permissionDetails.PermissionItemList[i].CommunityID,
                    CommunityName = permissionDetails.PermissionItemList[i].CommunityName,
                    Comment = permissionDetails.PermissionItemList[i].Comment,
                    Date = permissionDetails.PermissionItemList[i].Date,
                    Role = permissionDetails.PermissionItemList[i].Role,
                    IsInherited = permissionDetails.PermissionItemList[i].IsInherited,
                    CurrentUserRole = permissionDetails.PermissionItemList[i].CurrentUserRole
                };

                model.CanShowEditLink = model.CanShowDeleteLink = true;

                if (model.Role == UserRole.Owner &&
                        (singleOwner || model.CurrentUserRole < UserRole.Owner))
                {
                    // 1. No edit/delete options should be shown if there is only one owner.
                    // 2. Only owners and site administrators can edit/delete owners permissions.
                    model.CanShowEditLink = model.CanShowDeleteLink = false;
                }
                else if (model.Id == this.CurrentUserID)
                {
                    // No edit/delete options should be shown in users permission page for the logged in user
                    model.CanShowEditLink = model.CanShowDeleteLink = false;
                }
                else if (permissionDetails.PermissionItemList[i].IsInherited)
                {
                    // If the role of user permission is is inherited, then user should not be allowed to delete.
                    model.CanShowDeleteLink = false;

                    // If the role of user permission is Owner and is inherited, then user should not be allowed to edit also.
                    if (model.Role == UserRole.Owner)
                    {
                        model.CanShowEditLink = false;
                    }
                }

                permissionList.Add(model);
            }
            
            PermissionViewModel permissionViewModel = new PermissionViewModel(
                permissionDetails.CurrentUserPermission, 
                permissionList, 
                pageDetails, 
                permissionsTab);

            return permissionViewModel;
        }

        /// <summary>
        /// Gets the Invite Request details for the given community
        /// </summary>
        /// <param name="communityID">Community for which Invite Request details to be fetched</param>
        /// <param name="currentPage">Current page to be rendered</param>
        /// <returns>ViewModel with invite request details</returns>
        private InviteRequestViewModel GetInviteRequests(long communityID, int currentPage)
        {
            PageDetails pageDetails = new PageDetails(currentPage);
            pageDetails.ItemsPerPage = Constants.PermissionsPerPage;

            IEnumerable<InviteRequestItem> inviteRequestItemList = this.ProfileService.GetInviteRequests(this.CurrentUserID, communityID, pageDetails);

            this.CheckNotNull(() => new { inviteRequestItemList });

            List<InviteRequestDetailsViewModel> inviteRequestList = new List<InviteRequestDetailsViewModel>();

            for (var i = 0; i < inviteRequestItemList.Count(); i++)
            {
                InviteRequestDetailsViewModel model = new InviteRequestDetailsViewModel()
                {
                    InviteRequestID = inviteRequestItemList.ElementAt(i).InviteRequestID,
                    CommunityID = inviteRequestItemList.ElementAt(i).CommunityID,
                    EmailId = inviteRequestItemList.ElementAt(i).EmailIdList[0],
                    Role = (UserRole)inviteRequestItemList.ElementAt(i).RoleID,
                    InvitedDate = inviteRequestItemList.ElementAt(i).InvitedDate
                };

                inviteRequestList.Add(model);
            }

            InviteRequestViewModel inviteRequestViewModel = new InviteRequestViewModel(inviteRequestList, pageDetails);

            return inviteRequestViewModel;
        }

        /// <summary>
        /// Send Join community mail.
        /// </summary>
        /// <param name="permission">Permission item.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendJoinCommunityMail(PermissionItem permission)
        {
            try
            {
                // Send Mail.
                JoinCommunityRequest request = new JoinCommunityRequest();
                request.CommunityID = permission.CommunityID;
                request.CommunityName = permission.CommunityName;
                request.RequestorID = permission.UserID;
                request.RequestorName = permission.Name;
                request.PermissionRequested = permission.Role.ToString();
                request.RequestorLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                request.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);

                this.notificationService.NotifyJoinCommunity(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }

        /// <summary>
        /// Send Join community mail.
        /// </summary>
        /// <param name="permission">Permission item.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendProcessingStatusMail(PermissionItem permission)
        {
            try
            {
                // Send Mail for moderators.
                ModeratorPermissionStatusRequest moderatorPermissionStatusRequest = new ModeratorPermissionStatusRequest();
                moderatorPermissionStatusRequest.CommunityID = permission.CommunityID;
                moderatorPermissionStatusRequest.CommunityName = permission.CommunityName;
                moderatorPermissionStatusRequest.RequestorID = permission.UserID;
                moderatorPermissionStatusRequest.RequestorName = permission.Name;
                moderatorPermissionStatusRequest.ApprovedRole = permission.Role;
                moderatorPermissionStatusRequest.IsApproved = permission.Approved == true;
                moderatorPermissionStatusRequest.RequestorLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                moderatorPermissionStatusRequest.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);

                this.notificationService.NotifyModeratorPermissionStatus(moderatorPermissionStatusRequest);

                // Send Mail for moderators.
                UserPermissionStatusRequest request = new UserPermissionStatusRequest();
                request.CommunityID = permission.CommunityID;
                request.CommunityName = permission.CommunityName;
                request.RequestorID = permission.UserID;
                request.RequestorName = permission.Name;
                request.IsApproved = permission.Approved == true;
                request.RequestorLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                request.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);

                this.notificationService.NotifyUserRequestPermissionStatus(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }

        /// <summary>
        /// Send Join community mail.
        /// </summary>
        /// <param name="permission">Permission item.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendRemoveUserMail(PermissionItem permission)
        {
            try
            {
                // Send Mail.
                RemoveUserRequest request = new RemoveUserRequest();
                request.CommunityID = permission.CommunityID;
                request.CommunityName = permission.CommunityName;
                request.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);
                request.UserID = permission.UserID;
                request.UserLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                request.UserName = permission.Name;

                this.notificationService.NotifyRemoveUser(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }

        /// <summary>
        /// Send Join community mail.
        /// </summary>
        /// <param name="permission">Permission item.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendChangedUserRoleMail(PermissionItem permission)
        {
            try
            {
                // Send Mail.
                UserPermissionChangedRequest request = new UserPermissionChangedRequest();
                request.CommunityID = permission.CommunityID;
                request.CommunityName = permission.CommunityName;
                request.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);
                request.UserID = permission.UserID;
                request.UserLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                request.UserName = permission.Name;
                request.Role = permission.Role;
                request.ModeratorID = this.CurrentUserID;
                request.ModeratorLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), request.ModeratorID);

                this.notificationService.NotifyUserPermissionChangedStatus(request);
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }

        /// <summary>
        /// Send mail for Invite Requests of a community.
        /// </summary>
        /// <param name="invitedPeople">Invite request items of invited people.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exception related to notification.")]
        private void SendInviteRequestMail(IEnumerable<InviteRequestItem> invitedPeople, string communityName)
        {
            try
            {
                foreach (var item in invitedPeople)
                {
                    // Update the body to include the token.
                    string joinCommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Join/{1}/{2}", HttpContext.Request.Url.GetServerLink(), item.CommunityID, item.InviteRequestToken);

                    NotifyInviteRequest notifyInviteRequest = new NotifyInviteRequest()
                    {
                        EmailId = item.EmailIdList.ElementAt(0),
                        CommunityID = item.CommunityID,
                        CommunityName = communityName,
                        CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), item.CommunityID),
                        Subject = item.Subject,
                        Body = item.Body,
                        InviteLink = joinCommunityLink
                    };

                    // Send Mail for each invited people separately since the token is going to be unique for each user.
                    this.notificationService.NotifyCommunityInviteRequest(notifyInviteRequest);
                }
            }
            catch (Exception)
            {
                // Ignore all exceptions.
            }
        }

        #endregion Private Methods
    }
}
