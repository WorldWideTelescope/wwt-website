//-----------------------------------------------------------------------
// <copyright file="ContentController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp.Extensions;
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
        private IContentService _contentService;

        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService _notificationService;

        private ICommunityService _communityService;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ContentController class.
        /// </summary>
        /// <param name="contentService">Instance of Content Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public ContentController(IContentService contentService, IProfileService profileService, INotificationService queueService, ICommunityService communityService)
            : base(profileService)
        {
            _contentService = contentService;
            _notificationService = queueService;
            _communityService = communityService;
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
            var contentDetail = _contentService.GetContentDetails(id.Value, CurrentUserId);

            var contentViewModel = new ContentViewModel();
            contentViewModel.SetValuesFrom(contentDetail);


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
            var contentInputViewModel = new ContentInputViewModel
            {
                CategoryList = CategoryType.All.ToSelectList(CategoryType.All),
                CategoryID = (int) CategoryType.GeneralInterest
            };

            // Populating the category dropdown list.

            // Populating the parent communities for the current user.
            // TODO: Need to show the parent communities/folders in tree view dropdown.
            IEnumerable<Community> parentCommunities = this._contentService.GetParentCommunities(CurrentUserId);
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
                Data = new SelectList(this._contentService.GetParentCommunities(CurrentUserId), "CommunityID", "Name").ToList()
            };
        }

        [HttpPost]
        [Route("Content/User/Communities")]
        public JsonResult GetUserCommunities()
        {
            var communities = this._contentService.GetParentCommunities(CurrentUserId);
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
        [Route("Content/Create/New")]
        public JsonResult New(ContentInputViewModel contentInputViewModel, string id)
        {
            if (ModelState.IsValid)
            {
                ContentDetails contentDetails = new ContentDetails();
                contentDetails.SetValuesFrom(contentInputViewModel);
                contentDetails.CreatedByID = CurrentUserId;
                contentInputViewModel.ID = contentDetails.ID = this._contentService.CreateContent(contentDetails);
            }
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
            
            ContentDetails contentDetails = this._contentService.GetContentDetailsForEdit(id, this.CurrentUserId);

            
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
        public async Task<JsonResult> SaveEdits(string contentInputViewModel)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }

            //TODO: understand why can't cast the viewmodel directly  the way we do with new content??
            var viewModel = JsonConvert.DeserializeObject<ContentInputViewModel>(contentInputViewModel);
            
            this.CheckNotNull(() => new { viewModel });
            var isValid = ModelState.IsValid;
            if (isValid)
            {
                if (CurrentUserId != 0 && viewModel.ID.HasValue)
                {
                    var contentDetails = new ContentDetails();
                    contentDetails.SetValuesFrom(viewModel);

                    // Update contents.
                    _contentService.UpdateContent(contentDetails, CurrentUserId);

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
            if (CurrentUserId != 0)
            {
                status = this._contentService.DeleteContent(id, this.CurrentUserId);

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
        [Route("Content/AddContent/{id}/{extended=false}")]
        public JsonResult AddContent(HttpPostedFileBase contentFile, string id, bool extended)
        {
            
            var contentDataViewModel = new ContentDataViewModel();
            XmlDocument tourDoc = null;
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

                contentDataViewModel.ThumbnailLink = Url.Content("~/content/images/default" + Path.GetExtension(contentDataViewModel.ContentFileName).GetContentTypes().ToString().ToLower() + "thumbnail.png");

                // Upload associated file in the temporary container. Once the user publishes the content 
                // then we will move the file from temporary container to the actual container.
                // TODO: Need to have clean up task which will delete all unused file from temporary container.
                _contentService.UploadTemporaryFile(fileDetail);

                // Only for tour files, properties of the tour like title, description, author and thumbnail should be taken.
                if (Constants.TourFileExtension.Equals(Path.GetExtension(contentFile.FileName), StringComparison.OrdinalIgnoreCase))
                {
                    tourDoc = new XmlDocument();
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
                    tourDoc = new XmlDocument();
                    contentFile.InputStream.Seek(0, SeekOrigin.Begin);
                    tourDoc = tourDoc.SetXmlFromWtml(contentFile.InputStream);

                    if (tourDoc != null)
                    {
                        contentDataViewModel.TourTitle = tourDoc.GetAttributeValue("Folder", "Name");
                    }
                }
            }

            
            return Json(new
            {
                contentData=contentDataViewModel,
                extendedData = extended ? new {
                    tags=GetTourAttr(tourDoc,"Keywords"),
                    taxonomy = GetTourAttr(tourDoc, "Taxonomy"),
                    tourGuid = GetTourAttr(tourDoc, "ID"),
                    userLevel = GetTourAttr(tourDoc, "UserLevel"),
                    author = contentDataViewModel.TourDistributedBy,
                    authorEmail = GetTourAttr(tourDoc, "AuthorEmail"),
                    organization = GetTourAttr(tourDoc, "OrganizationName"),
                    organizationUrl = GetTourAttr(tourDoc, "OrganizationUrl")
                } : null
            });
        }


        private string GetTourAttr(XmlDocument tourDoc, string attr)
        {
            if (tourDoc != null)
            {
                return tourDoc.GetAttributeValue("Tour", attr);
            }
            return null;
        }




        /// <summary>
        /// Controller action which gets the associated content upload view.
        /// </summary>
        /// <param name="associatedFile">HttpPostedFileBase instance</param>
        [HttpPost]
        
        [Route("Content/Add/AssociatedContent")]
        public JsonResult AssociatedContent(HttpPostedFileBase associatedFile)
        {
            
            if (associatedFile != null)
            {
                // Get File details.
                var fileDetail = new FileDetail();
                fileDetail.SetValuesFrom(associatedFile);

                string fileName = Path.GetFileNameWithoutExtension(associatedFile.FileName);
                string fileDetailString = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}~{1}~{2}~{3}~-1",
                    Path.GetExtension(associatedFile.FileName),
                    associatedFile.ContentLength,
                    fileDetail.AzureID,
                    associatedFile.ContentType);

                // Upload associated file in the temporary container. Once the user publishes the content 
                //  then we will move the file from temporary container to the actual container.
                // TODO: Need to have clean up task which will delete all unused file from temporary container.
                _contentService.UploadTemporaryFile(fileDetail);
                return new JsonResult { Data = new
                {
                    fileName,fileDetailString
                } };
            }

            return new JsonResult{Data="error: no file"};
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
                this._contentService.IncrementDownloadCount(id, this.CurrentUserId);
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
                    this._contentService.UploadTemporaryFile(fileDetail);
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
