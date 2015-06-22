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
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using AutoMapper;
using Newtonsoft.Json;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;
using Formatting = System.Xml.Formatting;

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
        private ICommunityService _communityService;

        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService _notificationService;

        private IBlobService _blobService;
        private IContentService _contentService;
        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CommunityController class.
        /// </summary>
        /// <param name="communityService">Instance of community Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public CommunityController(ICommunityService communityService, IProfileService profileService, INotificationService queueService, IBlobService blobService, IContentService contentService)
            : base(profileService)
        {
            _communityService = communityService;
            _notificationService = queueService;
            _blobService = blobService;
            _contentService = contentService;
        }

        #endregion Constructor

        #region Action Methods

        /// <summary>
        /// Index Action which is default action rendering the home page.
        /// </summary>
        /// <param name="id">Community id to be processed</param>
        /// <param name="edit">gets editable community</param>
        /// <returns>Returns the View to be used</returns>
        [HttpGet]
        [Route("Community/Detail/{id}/{edit=false}")]
        public async Task<JsonResult> CommunityDetail(long? id, bool? edit)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            var communityDetails = await _communityService.GetCommunityDetails(id.Value, CurrentUserId, true, true);
            var permissionDetails = GetUserPermissionDetails(id.Value, PermissionsTab.Users, 0);
            
            if (edit == true)
            {
                var communityInputViewModel = new CommunityInputViewModel();
                Mapper.Map(communityDetails, communityInputViewModel);
                var json = new
                {
                    community = communityInputViewModel, 
                    permission = permissionDetails
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            var communityViewModel = new CommunityViewModel();
            Mapper.Map(communityDetails, communityViewModel);
            
            if (communityViewModel.AccessType != AccessType.Private || communityViewModel.UserPermission >= Permission.Reader)
            {
                var json = new
                {
                    community = communityViewModel, 
                    permission = permissionDetails
                };
                return Json(json,JsonRequestBehavior.AllowGet);
            }

            return new JsonResult
            {
                Data = new { communityViewModel.Name, communityViewModel.Id, error = "insufficient permission" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            
        }

        [HttpGet]
        [Route("Community/Contents/{communityId}")]
        public async Task<JsonResult> GetCommunityContents(long communityId)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            var contents = await _communityService.GetCommunityContents(communityId, CurrentUserId);
            var entities = new List<EntityViewModel>();
            foreach (var item in contents)
            {
                var contentViewModel = new ContentViewModel();
                contentViewModel.SetValuesFrom(item);
                entities.Add(contentViewModel);
            }
            var children = await _communityService.GetChildCommunities(communityId, CurrentUserId);
            var childCommunities = new List<CommunityViewModel>();
            foreach (var child in children)
            {
                var communityViewModel = new CommunityViewModel();
                Mapper.Map(child, communityViewModel);
                childCommunities.Add(communityViewModel);
            }
            return Json(new{entities,childCommunities},JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Community/TourThumbnail/{tourId}")]
        public ActionResult GetTourThumb(Guid tourId)
        {
            var tour = _contentService.GetContentDetails(tourId);
            // Get the thumbnail from Azure.
            Stream thumbStream = tour.Thumbnail.DataStream.GenerateThumbnail(Constants.DefaultClientThumbnailWidth, Constants.DefaultClientThumbnailHeight, Constants.DefaultThumbnailImageFormat);

            return File(thumbStream, Constants.DefaultThumbnailMimeType);

        }
        [HttpGet]
        [AllowAnonymous]
        [Route("Community/AuthorThumbnail/{tourId}")]
        public ActionResult GetTourAuthorThumb(Guid tourId)
        {
            var tour = _contentService.GetContentDetails(tourId);
            var thumb = tour.AssociatedFiles.FirstOrDefault();
            var thumbContent = new Content();
            thumbContent.SetValuesFrom(tour, thumb);

            var blobDetails = _blobService.GetFile(thumbContent.ContentAzureID);
            if (blobDetails != null && blobDetails.Data != null)
            {
                blobDetails.MimeType = Constants.DefaultThumbnailMimeType;

                // Update the response header.
                Response.AddHeader("Content-Encoding", blobDetails.MimeType);

                // Set the position to Begin.
                blobDetails.Data.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(blobDetails.Data, blobDetails.MimeType);
            }
            return new EmptyResult();

        }
        [HttpGet]
        [AllowAnonymous]
        [Route("Community/Fetch/Tours/{webclient=false}")]
        public ActionResult GetTourXml(bool webclient)
        {
            var storageAccount =
                    Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                        ConfigReader<string>.GetSetting("WWTWebBlobs"));

            var blobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = blobClient.GetContainerReference("tours");
            var toursBlob = cloudBlobContainer.GetBlobReferenceFromServer("alltours.wtml");

            toursBlob.DownloadToStream(Response.OutputStream);
            return new EmptyResult();
        }
    
        [HttpGet]
        [AllowAnonymous]
        [Route("Community/Rebuild/Tours/{webclient=false}")]
        public async Task<string> BuildPublicTourXml(bool webclient)
        {
            
            var adminCommunityId = 596915;
            var adminId = 184331;
            var tourDetailList = await _communityService.GetCommunityContents(adminCommunityId, adminId);
            var tourContentList = new List<ContentInputViewModel>();
            foreach (var item in tourDetailList)
            {
                
                var contentInputViewModel = new ContentInputViewModel();
                contentInputViewModel.SetValuesFrom(item);
                tourContentList.Add(contentInputViewModel);
                
            }
            var children = await _communityService.GetChildCommunities(adminCommunityId, adminId);
            var folders = new List<CommunityViewModel>();
            foreach (var child in children)
            {
                var communityViewModel = new CommunityViewModel();
                Mapper.Map(child, communityViewModel);
                folders.Add(communityViewModel);
            }
            using (var sw = new StringWriter())
            {
                using (var xmlWriter = new XmlTextWriter(sw))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                    xmlWriter.WriteStartElement("Folder");
                    foreach (var folder in folders)
                    {
                        
                        var tourIds = folder.Description != null ? folder.Description.Split(',') : new string[0];
                        var toursInFolder = false;
                        foreach (var tourId in tourIds)
                        {
                            try
                            {
                                var id = Convert.ToInt32(tourId);
                                var tourDetails = tourDetailList.Find(details => details.ID == id);
                                if (tourDetails == null || tourDetails.IsDeleted) continue;
                                var tourContents = tourContentList.Find(model => model.ID == id);
                                var json = tourDetails.Citation.Replace("json://", "");
                                var extData = JsonConvert.DeserializeObject<dynamic>(json);
                                if (webclient && (extData.webclient == null || extData.webclient != true)) continue;
                                Newtonsoft.Json.Linq.JArray related = extData.related;
                                string relatedTours = String.Empty;
                                if (webclient)
                                {
                                    foreach (Guid guid in related)
                                    {
                                        var relatedTour = tourContentList.Find(t => t.ContentDataID == guid);
                                        var relatedJson = relatedTour.Citation.Replace("json://", "");
                                        var relatedExtData = JsonConvert.DeserializeObject<dynamic>(relatedJson);
                                        if (relatedExtData.webclient != null && relatedExtData.webclient == true)
                                        {
                                            if (relatedTours.Length > 0)
                                            {
                                                relatedTours += ";";
                                            }
                                            relatedTours += guid.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    relatedTours = string.Join(";", related);
                                }
                                // write folder xml ONLY after first viable tour obj is found
                                if (!toursInFolder)
                                {
                                    xmlWriter.WriteStartElement("Folder");
                                    xmlWriter.WriteAttributeString("Name", folder.Name);
                                    xmlWriter.WriteAttributeString("Group", "Tour");
                                    xmlWriter.WriteAttributeString("Thumbnail", "");
                                    toursInFolder = true;
                                }
                                xmlWriter.WriteStartElement("Tour");
                                xmlWriter.WriteAttributeString("Title", tourDetails.Name.Replace("&", "&amp;"));
                                xmlWriter.WriteAttributeString("ID", tourContents.ContentDataID.ToString());
                                xmlWriter.WriteAttributeString("Description",
                                    tourDetails.Description.Replace("&", "&amp;"));
                                xmlWriter.WriteAttributeString("Classification",
                                    extData["classification"] != null ? extData["classification"].ToString() : "Other");
                                xmlWriter.WriteAttributeString("AuthorEmail",
                                    extData.authorEmail != null ? extData.authorEmail.ToString() : "");
                                xmlWriter.WriteAttributeString("Author",
                                    extData.author != null ? extData.author.ToString().Replace("&", "&amp;") : "");
                                xmlWriter.WriteAttributeString("AverageRating",
                                    tourDetails.AverageRating.ToString(CultureInfo.InvariantCulture));
                                xmlWriter.WriteAttributeString("LengthInSecs", tourContents.TourLength);
                                xmlWriter.WriteAttributeString("OrganizationUrl",
                                    extData.organizationUrl != null
                                        ? extData.organizationUrl.ToString().Replace("&", "&amp;")
                                        : "");
                                xmlWriter.WriteAttributeString("OrganizationName",
                                    extData.organization != null
                                        ? extData.organization.ToString().Replace("&", "&amp;")
                                        : "");
                                xmlWriter.WriteAttributeString("ITHList",
                                    extData.ithList != null
                                        ? extData.ithList.ToString()
                                        : extData.taxonomy != null ? extData.taxonomy.ToString() : "");
                                xmlWriter.WriteAttributeString("AstroObjectsList", string.Empty);
                                xmlWriter.WriteAttributeString("Keywords",
                                    tourDetails.Tags.Replace(',', ';').Replace(" ", ""));
                                xmlWriter.WriteAttributeString("RelatedTours", relatedTours);
                                xmlWriter.WriteEndElement();
                            }
                            catch (NullReferenceException)
                            {
                                //ignore - deleted tour
                            }
                        }
                        if (toursInFolder)
                            xmlWriter.WriteEndElement();
                    }
                    
                    xmlWriter.WriteEndElement();

                    xmlWriter.Close();
                }
                sw.Close();
                var xml = sw.ToString();
                var storageAccount =
                    Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                        ConfigReader<string>.GetSetting("WWTWebBlobs"));
                
                var blobClient = storageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = blobClient.GetContainerReference("tours");
                var toursBlob = cloudBlobContainer.GetBlobReferenceFromServer(webclient? "webclienttours.wtml" : "alltours.wtml");
                var bytes = new byte[xml.Length * sizeof(char)];
                Buffer.BlockCopy(xml.ToCharArray(), 0, bytes, 0, bytes.Length);
                toursBlob.UploadFromByteArray(bytes,0,bytes.Length);
                return toursBlob.StorageUri.PrimaryUri.AbsoluteUri;
            }
        }

        

        /// <summary>
        /// Controller action which inserts a new community to the Layerscape database.
        /// </summary>
        /// <param name="communityJson">ViewModel holding the details about the community</param>
        /// <returns>Returns a redirection view</returns>
        [HttpPost]
        [Route("Community/Create/New")]
        public async Task<ActionResult> New(CommunityInputViewModel communityJson)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }

            if (ModelState.IsValid)
            {
                var communityDetails = new CommunityDetails();
                Mapper.Map(communityJson, communityDetails);

                // Set thumbnail properties
                communityDetails.Thumbnail = new FileDetail() { AzureID = communityJson.ThumbnailID };
                communityDetails.CreatedByID = CurrentUserId;
                communityJson.ID = communityDetails.ID = _communityService.CreateCommunity(communityDetails);

                // Send Notification Mail
                _notificationService.NotifyNewEntityRequest(communityDetails, HttpContext.Request.Url.GetServerLink());

                return new JsonResult { Data = new { ID = communityDetails.ID } };
            }
            
            // In case of any validation error stay in the same page.
            return new JsonResult { Data = false };
        }

        

        /// <summary>
        /// Controller action which updates the details in Layerscape database about the community which is being edited.
        /// </summary>
        /// <param name="community">ViewModel holding the details about the community</param>
        
        /// <returns>Returns a redirection view</returns>
        [HttpPost]
        [Route("Community/Edit/Save")]
        public async Task<JsonResult> Edit(CommunityInputViewModel community)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            try
            {
                var communityDetails = new CommunityDetails();
                Mapper.Map(community, communityDetails);

                // Set thumbnail properties
                communityDetails.Thumbnail = new FileDetail {AzureID = community.ThumbnailID};

                if (CurrentUserId == 0)
                {
                    return Json("error: user not logged in");
                }
                _communityService.UpdateCommunity(communityDetails, CurrentUserId);
                return Json(new {id = community.ID});
            }
            catch (Exception exception)
            {
                
            }

            return Json("error: community not saved");
            
        }

        /// <summary>
        /// Deletes the specified community from the Layerscape database.
        /// </summary>
        /// <param name="id">Id of the community to be deleted.</param>
        /// <param name="parentId">Parent entity to which user to be take after delete</param>
        /// <returns>Returns an action url</returns>
        [HttpPost]
        [Route("Community/Delete/{id}/{parentId}")]
        public async Task<bool> Delete(long? id, long? parentId)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            if (id.HasValue)
            {
                if (CurrentUserId != 0)
                {
                    var status = _communityService.DeleteCommunity(id.Value, CurrentUserId);

                    return status.Succeeded;
                    // TODO: Need to add failure functionality.
                    
                }
            }

            return false;
        }
        
        /// <summary>
        /// It returns the permission list view on the basis of user type
        /// </summary>
        /// <param name="entityId">Community Id</param>
        /// <param name="permissionsTab">Either user / requestor</param>
        /// <param name="currentPage">Current page to be rendered</param>
        [HttpGet]
        [Route("Community/Permission/{permissionsTab}/{currentPage}")]
        public async Task<JsonResult> AjaxRenderPermission(long? entityId, PermissionsTab permissionsTab, int currentPage)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            var details =  GetUserPermissionDetails(entityId, permissionsTab, currentPage);
            return Json(details,JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Updates the user permission request for the specified user and community
        /// </summary>
        /// <param name="entityId">Community Id</param>
        /// <param name="requestorId">Requestor Id</param>
        /// <param name="userRole"> Permission Type (Owner, contributor, reader etc.)</param>
        /// <param name="approve">bool</param>
        [HttpPost]
        [Route("Community/Request/Reponse")]
        public async Task<JsonResult> UpdateUserPermissionRequest(long entityId, long requestorId, UserRole userRole, bool approve)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }

            var permission = new PermissionItem
            {
                UserID = requestorId,
                CommunityID = entityId,
                Role = userRole,
                Approved = approve
            };
            var operationStatus = ProfileService.UpdateUserPermissionRequest(permission, CurrentUserId);
            if (operationStatus.Succeeded)
            {
                try
                {
                    SendProcessingStatusMail(permission);
                }
                catch (Exception)
                {
                }
            }
            return Json(operationStatus.Succeeded);
        }

        
        /// <summary>
        /// Return success / failure
        /// </summary>
        /// <param name="communityId">Community Id</param>
        /// <param name="comments">Comments inserted by requestor</param>
        /// <param name="userRole">Permission type</param>
        /// <returns>Inserts the request of join</returns>
        [HttpPost]
        [Route("Community/Join/{communityId}/{userRole}")]
        public async Task<string> Join(long communityId, string comments, UserRole userRole)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            var ajaxResponse = "Success";
            var permission = new PermissionItem
            {
                UserID = CurrentUserId,
                CommunityID = communityId,
                Role = userRole,
                Comment = Server.UrlDecode(comments)
            };
            var operationStatus = ProfileService.JoinCommunity(permission);

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
        /// <param name="communityId">Community for which permission details to be fetched</param>
        /// <param name="permissionsTab">Permission tab (Users/Requests) for which data to be fetched</param>
        /// <param name="currentPage">Current page to be rendered</param>
        /// <returns>ViewModel with permission details</returns>
        private async Task<PermissionViewModel> GetUserPermissionDetails(long? communityId, PermissionsTab permissionsTab,
            int currentPage)
        {
            var pageDetails = new PageDetails(currentPage) {ItemsPerPage = Constants.PermissionsPerPage};

            PermissionDetails permissionDetails = null;

            if (permissionsTab == PermissionsTab.Users)
            {
                permissionDetails = await ProfileService.GetUserPemissions(CurrentUserId, communityId.Value,
                    pageDetails);
            }
            else if (permissionsTab == PermissionsTab.Requests)
            {
                permissionDetails = await ProfileService.GetUserPemissionRequests(CurrentUserId, communityId,
                    pageDetails);
            }
            else
            {
                permissionDetails = await ProfileService.GetUserPemissionRequests(CurrentUserId, null, pageDetails);
            }

            if (permissionDetails != null)
            {


                // Check if there is only one owner for the current community.
                var singleOwner = permissionDetails.PermissionItemList.Count(p => p.Role == UserRole.Owner) == 1;

                var permissionList = new List<PermissionDetailsViewModel>();
                foreach (var permissionItem in permissionDetails.PermissionItemList)
                {
                    var model = new PermissionDetailsViewModel()
                    {
                        Id = permissionItem.UserID,
                        Name = permissionItem.Name,
                        CommunityId = permissionItem.CommunityID,
                        CommunityName = permissionItem.CommunityName,
                        Comment = permissionItem.Comment,
                        Date = permissionItem.Date,
                        Role = permissionItem.Role,
                        IsInherited = permissionItem.IsInherited,
                        CurrentUserRole = permissionItem.CurrentUserRole
                    };
                    model.Requested = model.Date.GetFormattedDifference(DateTime.UtcNow);

                    model.CanShowEditLink = model.CanShowDeleteLink = true;

                    if (model.Role == UserRole.Owner &&
                        (singleOwner || model.CurrentUserRole < UserRole.Owner))
                    {
                        // 1. No edit/delete options should be shown if there is only one owner.
                        // 2. Only owners and site administrators can edit/delete owners permissions.
                        model.CanShowEditLink = model.CanShowDeleteLink = false;
                    }
                    else if (model.Id == CurrentUserId)
                    {
                        // No edit/delete options should be shown in users permission page for the logged in user
                        model.CanShowEditLink = model.CanShowDeleteLink = false;
                    }
                    else if (permissionItem.IsInherited)
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

                var permissionViewModel = new PermissionViewModel(
                    permissionDetails.CurrentUserPermission,
                    permissionList,
                    pageDetails,
                    permissionsTab);

                return permissionViewModel;
            }
            else return null;

        }

        /// <summary>
        /// Gets the Invite Request details for the given community
        /// </summary>
        /// <param name="communityId">Community for which Invite Request details to be fetched</param>
        /// <param name="currentPage">Current page to be rendered</param>
        /// <returns>ViewModel with invite request details</returns>
        private async Task<InviteRequestViewModel> GetInviteRequests(long communityId, int currentPage)
        {
            var pageDetails = new PageDetails(currentPage);
            pageDetails.ItemsPerPage = Constants.PermissionsPerPage;

            var inviteRequestItemList = await ProfileService.GetInviteRequests(CurrentUserId, communityId, pageDetails);

            this.CheckNotNull(() => new { inviteRequestItemList });

            var inviteRequestList = new List<InviteRequestDetailsViewModel>();

            for (var i = 0; i < inviteRequestItemList.Count(); i++)
            {
                var model = new InviteRequestDetailsViewModel()
                {
                    InviteRequestID = inviteRequestItemList.ElementAt(i).InviteRequestID,
                    CommunityID = inviteRequestItemList.ElementAt(i).CommunityID,
                    EmailId = inviteRequestItemList.ElementAt(i).EmailIdList[0],
                    Role = (UserRole)inviteRequestItemList.ElementAt(i).RoleID,
                    InvitedDate = inviteRequestItemList.ElementAt(i).InvitedDate
                };

                inviteRequestList.Add(model);
            }

            var inviteRequestViewModel = new InviteRequestViewModel(inviteRequestList, pageDetails);

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
                var request = new JoinCommunityRequest();
                request.CommunityID = permission.CommunityID;
                request.CommunityName = permission.CommunityName;
                request.RequestorID = permission.UserID;
                request.RequestorName = permission.Name;
                request.PermissionRequested = permission.Role.ToString();
                request.RequestorLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                request.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);

                _notificationService.NotifyJoinCommunity(request);
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
                var moderatorPermissionStatusRequest = new ModeratorPermissionStatusRequest();
                moderatorPermissionStatusRequest.CommunityID = permission.CommunityID;
                moderatorPermissionStatusRequest.CommunityName = permission.CommunityName;
                moderatorPermissionStatusRequest.RequestorID = permission.UserID;
                moderatorPermissionStatusRequest.RequestorName = permission.Name;
                moderatorPermissionStatusRequest.ApprovedRole = permission.Role;
                moderatorPermissionStatusRequest.IsApproved = permission.Approved == true;
                moderatorPermissionStatusRequest.RequestorLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                moderatorPermissionStatusRequest.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);

                _notificationService.NotifyModeratorPermissionStatus(moderatorPermissionStatusRequest);

                // Send Mail for moderators.
                var request = new UserPermissionStatusRequest();
                request.CommunityID = permission.CommunityID;
                request.CommunityName = permission.CommunityName;
                request.RequestorID = permission.UserID;
                request.RequestorName = permission.Name;
                request.IsApproved = permission.Approved == true;
                request.RequestorLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                request.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);

                _notificationService.NotifyUserRequestPermissionStatus(request);
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
                var request = new RemoveUserRequest();
                request.CommunityID = permission.CommunityID;
                request.CommunityName = permission.CommunityName;
                request.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);
                request.UserID = permission.UserID;
                request.UserLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                request.UserName = permission.Name;

                _notificationService.NotifyRemoveUser(request);
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
                var request = new UserPermissionChangedRequest();
                request.CommunityID = permission.CommunityID;
                request.CommunityName = permission.CommunityName;
                request.CommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.CommunityID);
                request.UserID = permission.UserID;
                request.UserLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), permission.UserID);
                request.UserName = permission.Name;
                request.Role = permission.Role;
                request.ModeratorID = CurrentUserId;
                request.ModeratorLink = string.Format(CultureInfo.InvariantCulture, "{0}Profile/Index/{1}", HttpContext.Request.Url.GetServerLink(), request.ModeratorID);

                _notificationService.NotifyUserPermissionChangedStatus(request);
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
                    var joinCommunityLink = string.Format(CultureInfo.InvariantCulture, "{0}Community/Join/{1}/{2}", HttpContext.Request.Url.GetServerLink(), item.CommunityID, item.InviteRequestToken);

                    var notifyInviteRequest = new NotifyInviteRequest()
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
                    _notificationService.NotifyCommunityInviteRequest(notifyInviteRequest);
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
