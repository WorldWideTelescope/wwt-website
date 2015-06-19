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
        [HttpGet]
        [Route("Content/Detail/{Id}")]
        public async Task<JsonResult> CommunityDetail(long? id)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            if (!id.HasValue)
            {
                return new JsonResult { Data = "error: invalid content id",
                JsonRequestBehavior=JsonRequestBehavior.AllowGet};
            }
            var contentDetail = _contentService.GetContentDetails(id.Value, CurrentUserId);

            var contentViewModel = new ContentViewModel();
            contentViewModel.SetValuesFrom(contentDetail);


            // It creates the prefix for id of links
            SetSiteAnalyticsPrefix(HighlightType.None);

            return new JsonResult { 
                Data = contentViewModel,
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        [Route("Content/User/CommunityList")]
        public async Task<JsonResult> GetUserCommunityList()
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            return new JsonResult
            {
                Data = new SelectList(_contentService.GetParentCommunities(CurrentUserId), "CommunityID", "Name").ToList(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        [Route("Content/User/Communities")]
        public async Task<JsonResult> GetUserCommunities()
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            var communities = _contentService.GetParentCommunities(CurrentUserId);
            var result = new List<object>();
            foreach (var c in communities)
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
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// Controller action which inserts a new content to the Layerscape database.
        /// </summary>
        /// <param name="contentInputViewModel">ViewModel holding the details about the content</param>
        /// <returns>Returns a redirection view</returns>
        [HttpPost]
        [Route("Content/Create/New")]
        public async Task<JsonResult> New(ContentInputViewModel contentInputViewModel, string id)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            if (ModelState.IsValid)
            {
                var contentDetails = new ContentDetails();
                contentDetails.SetValuesFrom(contentInputViewModel);
                contentDetails.CreatedByID = CurrentUserId;
                contentInputViewModel.ID = contentDetails.ID = _contentService.CreateContent(contentDetails);
            }
            return new JsonResult { Data = contentInputViewModel };
        }

        /// <summary>
        /// Controller action which gets the details about the content which is being edited.
        /// </summary>
        /// <param name="id">Id of the content getting edited.</param>
        /// <returns>View having page which gets details about content getting updated</returns>
        [HttpGet]
        [Route("Content/Edit/{id}")]
        public async Task<JsonResult> GetEditableContent(long id)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }

            var contentInputViewModel = new ContentInputViewModel();
            var contentDetails = _contentService.GetContentDetailsForEdit(id, CurrentUserId);

            // Set value from ContentDetials to ContentInputViewModel.
            contentInputViewModel.SetValuesFrom(contentDetails);

            // Set Thumbnail URL.
            contentInputViewModel.ThumbnailLink = contentInputViewModel.ThumbnailID != Guid.Empty ? 
                Url.Action("Thumbnail", "File", new { id = contentInputViewModel.ThumbnailID }) : 
                Url.Content("~/content/images/default" + Enum.GetName(typeof(ContentTypes), contentDetails.ContentData.ContentType) + "thumbnail.png");
            
            return new JsonResult{Data=contentInputViewModel,JsonRequestBehavior = JsonRequestBehavior.AllowGet};
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
            return Json("error: Could not save changes to content");
            
        }

        /// <summary>
        /// Deletes the specified content from the  database.
        /// </summary>
        /// <param name="id">Id of the content to be deleted.</param>
        
        /// <returns>Returns status</returns>
        [HttpPost]
        [Route("Content/Delete/{id}")]
        public async Task<JsonResult> Delete(long id)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            OperationStatus status = null;
            if (CurrentUserId != 0)
            {
                status = _contentService.DeleteContent(id, CurrentUserId);

                // TODO: Need to add failure functionality.
                //if (!status.Succeeded)
                
            }
            return new JsonResult{Data=status};
        }

        

        /// <summary>
        /// Controller action which gets the content file upload view.
        /// </summary>
        /// <param name="contentFile">HttpPostedFileBase instance</param>
        [HttpPost]
        [Route("Content/AddContent/{id}/{extended=false}")]
        public async Task<JsonResult> AddContent(HttpPostedFileBase contentFile, string id, bool extended)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
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
                    author = GetTourAttr(tourDoc, "Author"),
                    authorEmail = GetTourAttr(tourDoc, "AuthorEmail"),
                    authorUrl = GetTourAttr(tourDoc, "AuthorUrl"),
                    organization = GetTourAttr(tourDoc, "OrganizationName"),
                    organizationUrl = GetTourAttr(tourDoc, "OrganizationUrl"),
                    classification = GetTourAttr(tourDoc, "Classification"),
                    ithList = GetTourAttr(tourDoc, "ITHList")
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
        public async Task<JsonResult> AssociatedContent(HttpPostedFileBase associatedFile)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            if (associatedFile != null)
            {
                // Get File details.
                var fileDetail = new FileDetail();
                fileDetail.SetValuesFrom(associatedFile);

                var fileName = Path.GetFileNameWithoutExtension(associatedFile.FileName);
                var fileDetailString = string.Format(
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
        [Route("Content/Downloads/Increment/{id}")]
        public bool IncrementDownloadCount(long id)
        {
            try
            {
                if (id > 0)
                {
                    _contentService.IncrementDownloadCount(id, CurrentUserId);
                    return true;
                }
            }
            catch (Exception){}
            return false;
        }
        
        #endregion Action Methods
    }
}
