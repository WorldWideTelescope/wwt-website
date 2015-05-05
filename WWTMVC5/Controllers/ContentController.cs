//-----------------------------------------------------------------------
// <copyright file="ContentController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Newtonsoft.Json;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;
using WWTMVC5.WebServices;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the Content page request which makes request to repository and get the
    /// data about the Content and pushes them to the Content View.
    /// </summary>
    public class ContentController : ControllerBase
    {
        #region Private Variables

        /// <summary>
        /// Instance of Content Service
        /// </summary>
        private IContentService contentService;

        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService notificationService;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ContentController class.
        /// </summary>
        /// <param name="contentService">Instance of Content Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public ContentController(IContentService contentService, IProfileService profileService, INotificationService queueService)
            : base(profileService)
        {
            this.contentService = contentService;
            this.notificationService = queueService;
        }

        #endregion Constructor

        #region Action Methods

        /// <summary>
        /// Index Action which is default action rendering the home page.
        /// </summary>
        /// <param name="id">Content id to be processed</param>
        /// <returns>Returns the View to be used</returns>
        [HttpPost]
        [Route("Content/RenderDetailJson/{Id}")]
        public ActionResult Index(long? id)
        {
            this.CheckNotNull(() => new { id });

            ContentViewModel contentViewModel = null;
            ContentDetails contentDetail = this.contentService.GetContentDetails(id.Value, this.CurrentUserID);

            this.CheckNotNull(() => new { contentDetail });

            contentViewModel = new ContentViewModel();
            contentViewModel.SetValuesFrom(contentDetail);

            // Extract only the text from the Description HTML which needs to be used in Mail sharing. HTML tags which are meant for
            // styling the description is not required.
            string description = contentViewModel.Description.GetTextFromHtmlString();
            description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Length > 100 ? description.Substring(0, 97) + "..." : description;

            // This should not be there in the extension method since it is needed only in cases where description is sent as query string.
            //description = description.Replace("&", "%26");

            // Get the share URL's.
            //contentViewModel.ShareUrl = new ShareViewModel();
            //contentViewModel.ShareUrl.FacebookUrl = new Uri(
            //        string.Format(CultureInfo.InvariantCulture, Constants.FacebookShareLinkFormat, HttpContext.Request.Url));

            //contentViewModel.ShareUrl.TwitterUrl = new Uri(
            //        string.Format(CultureInfo.InvariantCulture, Constants.TwitterShareLinkFormat));

            //contentViewModel.ShareUrl.MailToUrl = new Uri(string.Format(
            //    CultureInfo.InvariantCulture,
            //    Constants.MailToLinkFormat,
            //    contentViewModel.Name,
            //    HttpContext.Request.Url,
            //    description));

            // It creates the prefix for id of links
            SetSiteAnalyticsPrefix(HighlightType.None);

            return new JsonResult { Data = contentViewModel};
        }

        /// <summary>
        /// Controller action which gets the details about the new content.
        /// </summary>
        /// <returns>View having page which gets details about new content</returns>
        [HttpGet]
        
        public ActionResult New(long? id)
        {
            ContentInputViewModel contentInputViewModel = new ContentInputViewModel();

            // Populating the category dropdown list.
            contentInputViewModel.CategoryList = CategoryType.All.ToSelectList(CategoryType.All);
            contentInputViewModel.CategoryID = (int)CategoryType.GeneralInterest;

            // Populating the parent communities for the current user.
            // TODO: Need to show the parent communities/folders in tree view dropdown.
            IEnumerable<Community> parentCommunities = this.contentService.GetParentCommunities(CurrentUserID);
            contentInputViewModel.ParentList = new SelectList(parentCommunities, "CommunityID", "Name");

            // Default access type is public (2).
            contentInputViewModel.AccessTypeID = 2;

            // Set the thumbnail URL.
            contentInputViewModel.ThumbnailLink = Url.Content("~/content/images/defaultgenericthumbnail.png");

            Community parentCommunity = null;
            if (id.HasValue)
            {
                // Set the Category, Distributed by and Tags from parent
                parentCommunity = parentCommunities.FirstOrDefault(community => community.CommunityID == id);
            }
            else
            {
                // Get Visitor Community.
                parentCommunity = parentCommunities.FirstOrDefault(community => community.CommunityTypeID == (int)CommunityTypes.User);
            }

            if (parentCommunity != null)
            {
                contentInputViewModel.ParentID = parentCommunity.CommunityID;

                contentInputViewModel.CategoryID = parentCommunity.CategoryID;
                contentInputViewModel.DistributedBy = parentCommunity.DistributedBy;
            }

            return View("Save", contentInputViewModel);
        }
        [HttpPost]
        [Route("Content/User/CommunityList")]
        public JsonResult GetUserCommunityList()
        {
            return new JsonResult
            {
                Data = new SelectList(this.contentService.GetParentCommunities(CurrentUserID), "CommunityID", "Name").ToList()
            };
        }

        [HttpPost]
        [Route("Content/User/Communities")]
        public JsonResult GetUserCommunities()
        {
            var communities = this.contentService.GetParentCommunities(CurrentUserID);
            var result = new List<object>();
            foreach (Community c in communities)
            {
                result.Add(new
                {
                    c.AccessType,
                    c.CategoryID,
                    c.CommunityID,
                    c.CreatedByID,
                    c.CreatedDatetime,
                    c.Description,
                    c.CommunityTags,
                    c.CommunityType,
                    c.Name//,
                    //c.CommunityContents
                });
            }
            return new JsonResult
            {
                Data = result
            };
        }

        /// <summary>
        /// Controller action which inserts a new content to the Layerscape database.
        /// </summary>
        /// <param name="contentInputViewModel">ViewModel holding the details about the content</param>
        /// <returns>Returns a redirection view</returns>
        [HttpPost]
        //
        //[ValidateAntiForgeryToken]
        [Route("Content/Create/New")]
        public JsonResult New(ContentInputViewModel contentInputViewModel, string id)
        {
            // Make sure contentInputViewModel is not null
            this.CheckNotNull(() => new { contentInputViewModel });

            //contentInputViewModel.CreatedByID = CurrentUserID;

            // Populating the parent communities for the current user.
            // TODO: Need to show the parent communities/folders in tree view dropdown.
            //IEnumerable<Community> parentCommunities = this.contentService.GetParentCommunities(CurrentUserID);
            

            if (ModelState.IsValid)
            {
                ContentDetails contentDetails = new ContentDetails();
                contentDetails.SetValuesFrom(contentInputViewModel);

                contentDetails.CreatedByID = CurrentUserID;

                contentInputViewModel.ID = contentDetails.ID = this.contentService.CreateContent(contentDetails);

                // Send New Content Request.
                //this.notificationService.NotifyNewEntityRequest(contentDetails, HttpContext.Request.Url.GetServerLink());
                //TODO: Wire up approvals
                var adminsvc = new AdministrationService();
                adminsvc.MarkAsPublicContent(contentDetails.ID.ToString());

            }
            
            // In case of any validation error stay in the same page.
            return new JsonResult { Data = contentInputViewModel };
            
        }

        /// <summary>
        /// Controller action which gets the details about the content which is being edited.
        /// </summary>
        /// <param name="id">Id of the content getting edited.</param>
        /// <returns>View having page which gets details about content getting updated</returns>
        [HttpPost]
        [Route("Content/Edit/{id}")]
        public ActionResult Edit(long id)
        {
            ContentInputViewModel contentInputViewModel = new ContentInputViewModel();
            
            //contentInputViewModel.CategoryList = CategoryType.All.ToSelectList(CategoryType.All);

            // Populating the parent communities for the current user.
            // TODO: Need to show the parent communities/folders in tree view dropdown.
            //IEnumerable<Community> parentCommunities = this.contentService.GetParentCommunities(CurrentUserID);
            //contentInputViewModel.ParentList = new SelectList(parentCommunities, "CommunityID", "Name");
            
            ContentDetails contentDetails = this.contentService.GetContentDetailsForEdit(id, this.CurrentUserID);

            // Make sure communitieView is not null
            this.CheckNotNull(() => new { contentDetails });

            // Set value from ContentDetials to ContentInputViewModel.
            contentInputViewModel.SetValuesFrom(contentDetails);

            // Set Thumbnail URL.
            if (contentInputViewModel.ThumbnailID != Guid.Empty)
            {
                contentInputViewModel.ThumbnailLink = Url.Action("Thumbnail", "File", new { id = contentInputViewModel.ThumbnailID });
            }
            else
            {
                contentInputViewModel.ThumbnailLink = Url.Content("~/content/images/default" + Enum.GetName(typeof(ContentTypes), contentDetails.ContentData.ContentType) + "thumbnail.png");
            }
            
            return new JsonResult{Data=contentInputViewModel};
        }

        /// <summary>
        /// Controller action which updates the details in Layerscape database about the content which is being edited.
        /// </summary>
        /// <param name="contentInputViewModel">ViewModel holding the details about the content</param>
        /// <returns>Json result</returns>
        [HttpPost]
        [Route("Content/Save/Edits")]
        public JsonResult SaveEdits(string contentInputViewModel)
        {
            //TODO: understand why can't cast the viewmodel directly  the way we do with new content??
            var viewModel = JsonConvert.DeserializeObject<ContentInputViewModel>(contentInputViewModel);
            
            this.CheckNotNull(() => new { viewModel });
            var isValid = ModelState.IsValid;
            if (isValid)
            {
                if (CurrentUserID != 0 && viewModel.ID.HasValue)
                {
                    var contentDetails = new ContentDetails();
                    contentDetails.SetValuesFrom(viewModel);

                    // Update contents.
                    contentService.UpdateContent(contentDetails, CurrentUserID);

                    return Json(contentDetails);
                }
                return Json("error: User not logged in");

            }
            else
            {
                return Json("error: Could not save changes to content");
            }
        }

        /// <summary>
        /// Deletes the specified content from the  database.
        /// </summary>
        /// <param name="id">Id of the content to be deleted.</param>
        
        /// <returns>Returns status</returns>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "TODO: Custom Exception handling to be added."), HttpPost]
        [Route("Content/Delete/{id}")]
        public JsonResult Delete(long id)
        {
            OperationStatus status = null;
            if (CurrentUserID != 0)
            {
                status = this.contentService.DeleteContent(id, this.CurrentUserID);

                // TODO: Need to add failure functionality.
                //if (!status.Succeeded)
                
            }
            return new JsonResult{Data=status};
        }

        /// <summary>
        /// Controller action which gets the content file upload view.
        /// </summary>
        [HttpGet]
        
        public void AddContent(ContentDataViewModel contentDataViewModel)
        {
            try
            {
                if (contentDataViewModel != null && !string.IsNullOrWhiteSpace(contentDataViewModel.ContentFileDetail))
                {
                    string[] fileDetails = contentDataViewModel.ContentFileDetail.Split('~');
                    if (fileDetails.Length == 5)
                    {
                        contentDataViewModel.ContentID = Convert.ToInt64(fileDetails[4], CultureInfo.InvariantCulture);
                    }
                }

                PartialView("AddContentView", contentDataViewModel).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Controller action which gets the content file upload view.
        /// </summary>
        /// <param name="contentFile">HttpPostedFileBase instance</param>
        [HttpPost]
        //
        //[ValidateAntiForgeryToken]
        [Route("Content/AddContent/{id}")]
        public JsonResult AddContent(HttpPostedFileBase contentFile, string id)
        {
            
                ContentDataViewModel contentDataViewModel = new ContentDataViewModel();

                if (contentFile != null)
                {
                    // Get File details.
                    var fileDetail = new FileDetail();
                    fileDetail.SetValuesFrom(contentFile);

                    contentDataViewModel.ContentDataID = fileDetail.AzureID;
                    contentDataViewModel.ContentFileName = Path.GetFileName(contentFile.FileName);
                    contentDataViewModel.ContentFileDetail = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}~{1}~{2}~{3}~-1",
                        Path.GetExtension(contentFile.FileName),
                        contentFile.ContentLength,
                        fileDetail.AzureID,
                        contentFile.ContentType);

                    contentDataViewModel.ThumbnailLink = Url.Content("~/content/images/default" + Path.GetExtension(contentDataViewModel.ContentFileName).GetContentTypes().ToString() + "thumbnail.png");

                    // Upload associated file in the temporary container. Once the user publishes the content 
                    // then we will move the file from temporary container to the actual container.
                    // TODO: Need to have clean up task which will delete all unused file from temporary container.
                    this.contentService.UploadTemporaryFile(fileDetail);

                    // Only for tour files, properties of the tour like title, description, author and thumbnail should be taken.
                    if (Constants.TourFileExtension.Equals(Path.GetExtension(contentFile.FileName), StringComparison.OrdinalIgnoreCase))
                    {
                        XmlDocument tourDoc = new XmlDocument();
                        contentFile.InputStream.Seek(0, SeekOrigin.Begin);
                        tourDoc = tourDoc.SetXmlFromTour(contentFile.InputStream);

                        if (tourDoc != null)
                        {
                            // Note that the spelling of Description is wrong because that's how WWT generates the WTT file.
                            contentDataViewModel.TourTitle = tourDoc.GetAttributeValue("Tour", "Title");
                            contentDataViewModel.TourThumbnail = tourDoc.GetAttributeValue("Tour", "ThumbnailUrl");
                            contentDataViewModel.TourDescription = tourDoc.GetAttributeValue("Tour", "Descirption");
                            contentDataViewModel.TourDistributedBy = tourDoc.GetAttributeValue("Tour", "Author");
                            contentDataViewModel.TourLength = tourDoc.GetAttributeValue("Tour", "RunTime");
                        }
                    }
                    else if (Constants.CollectionFileExtension.Equals(Path.GetExtension(contentFile.FileName), StringComparison.OrdinalIgnoreCase))
                    {
                        //// Only for WTML files, properties of the collection like title, thumbnail should be taken.
                        XmlDocument tourDoc = new XmlDocument();
                        contentFile.InputStream.Seek(0, SeekOrigin.Begin);
                        tourDoc = tourDoc.SetXmlFromWtml(contentFile.InputStream);

                        if (tourDoc != null)
                        {
                            contentDataViewModel.TourTitle = tourDoc.GetAttributeValue("Folder", "Name");
                        }
                    }
                }

                //PartialView("AddContentView", contentDataViewModel).ExecuteResult(this.ControllerContext);
            return new JsonResult{Data=contentDataViewModel};
        }

        /// <summary>
        /// Controller action which gets the associated content upload view.
        /// </summary>
        [HttpGet]
        
        public void AssociatedContent()
        {
            try
            {
                PartialView("AddAssociatedContentView").ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Controller action which gets the associated content upload view.
        /// </summary>
        /// <param name="associatedFile">HttpPostedFileBase instance</param>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public void AssociatedContent(HttpPostedFileBase associatedFile)
        {
            try
            {
                ViewData["PostedFileName"] = string.Empty;
                ViewData["PostedFileDetail"] = string.Empty;

                if (associatedFile != null)
                {
                    // Get File details.
                    var fileDetail = new FileDetail();
                    fileDetail.SetValuesFrom(associatedFile);

                    ViewData["PostedFileName"] = Path.GetFileNameWithoutExtension(associatedFile.FileName);
                    ViewData["PostedFileDetail"] = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}~{1}~{2}~{3}~-1",
                        Path.GetExtension(associatedFile.FileName),
                        associatedFile.ContentLength,
                        fileDetail.AzureID,
                        associatedFile.ContentType);

                    // Upload associated file in the temporary container. Once the user publishes the content 
                    //  then we will move the file from temporary container to the actual container.
                    // TODO: Need to have clean up task which will delete all unused file from temporary container.
                    this.contentService.UploadTemporaryFile(fileDetail);
                }

                PartialView("AddAssociatedContentView").ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Action for incrementing the download count of the content.
        /// </summary>
        /// <param name="id">Content id.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void IncrementDownloadCount(long id)
        {
            if (id > 0)
            {
                this.contentService.IncrementDownloadCount(id, this.CurrentUserID);
            }
        }

        /// <summary>
        /// Controller action which gets the video file upload view.
        /// </summary>
        [HttpGet]
        
        public void AddVideo()
        {
            VideoDataViewModel videoDataViewModel = new VideoDataViewModel();
            PartialView("AddVideoView", videoDataViewModel).ExecuteResult(this.ControllerContext);
        }

        /// <summary>
        /// Controller action which gets the video file upload view.
        /// </summary>
        /// <param name="video">HttpPostedFileBase instance</param>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public void AddVideo(HttpPostedFileBase video)
        {
            try
            {
                VideoDataViewModel videoDataViewModel = new VideoDataViewModel();

                if (video != null)
                {
                    // Get File details.
                    var fileDetail = new FileDetail();
                    fileDetail.SetValuesFrom(video);

                    videoDataViewModel.VideoID = fileDetail.AzureID;
                    videoDataViewModel.VideoName = Path.GetFileName(video.FileName);
                    videoDataViewModel.VideoFileDetail = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}~{1}~{2}~{3}~-1",
                        Path.GetExtension(video.FileName),
                        video.ContentLength,
                        fileDetail.AzureID,
                        video.ContentType);

                    // Upload video file in the temporary container. Once the user publishes the content 
                    // then we will move the file from temporary container to the actual container.
                    this.contentService.UploadTemporaryFile(fileDetail);
                }

                PartialView("AddVideoView", videoDataViewModel).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }
        #endregion Action Methods
    }
}
