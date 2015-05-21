using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.WebServices;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// This controller used by the WWT Windows Client
    /// </summary>
    public class ResourcesController : ControllerBase
    {
        #region Private Variables

        /// <summary>
        /// Instance of Content Service
        /// </summary>
        private IContentService contentService;
        private ICommunityService communityService;

        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService notificationService;

        private IProfileService profileService;
        
        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ContentController class.
        /// </summary>
        /// <param name="contentService">Instance of Content Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        /// <param name="communityService"></param>
        /// <param name="queueService"></param>
        public ResourcesController(IContentService contentService, IProfileService profileService, ICommunityService communityService, INotificationService queueService)
            : base(profileService)
        {
            this.contentService = contentService;
            this.notificationService = queueService;
            this.profileService = profileService;
            this.communityService = communityService;
        }

        #endregion Constructor

        
        #region Web Methods

        #region Mandatory profile check calls - Only logged in users

        //This method not needed
        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/User")]
        public async Task<bool> CheckIfUserExists()
        {
            ProfileDetails profileDetails = await ValidateAuthentication();
            return profileDetails != null;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Resource/Service/User")]
        public async Task<bool> RegisterUser()
        {
            ProfileDetails profileDetails = await ValidateAuthentication();

            if (profileDetails == null)
            {
                var svc = new LiveIdAuth();
                dynamic jsonResult = svc.GetMeInfo(System.Web.HttpContext.Current.Request.Headers["LiveUserToken"]);
                profileDetails = new ProfileDetails(jsonResult);
                // While creating the user, IsSubscribed to be true always.
                profileDetails.IsSubscribed = true;

                // When creating the user, by default the user type will be of regular. 
                profileDetails.UserType = UserTypes.Regular;
                profileDetails.ID = this.ProfileService.CreateProfile(profileDetails);
                    
                // This will used as the default community when user is uploading a new content.
                // This community will need to have the following details:
                CommunityDetails communityDetails = new CommunityDetails
                {
                    CommunityType = CommunityTypes.User,// 1. This community type should be User
                    CreatedByID = profileDetails.ID,// 2. CreatedBy will be the new USER.
                    IsFeatured = false,// 3. This community is not featured.
                    Name = Resources.UserCommunityName,// 4. Name should be NONE.
                    AccessTypeID = (int) AccessType.Private,// 5. Access type should be private.
                    CategoryID = (int) CategoryType.GeneralInterest// 6. Set the category ID of general interest. We need to set the Category ID as it is a foreign key and cannot be null.
                };

                // 7. Create the community
                communityService.CreateCommunity(communityDetails);

                // Send New user notification.
                this.notificationService.NotifyNewEntityRequest(profileDetails,
                    HttpContext.Request.Url.GetServerLink());
            }
            else
            {
                throw new WebFaultException<string>("User already registered", HttpStatusCode.BadRequest);
            } 
            return true;
        }

           
        
        [AllowAnonymous]
        [HttpDelete]
        [Route("Resource/Service/Community/{id}")]
        public async Task<bool> DeleteCommunity(string id)
        {
            ProfileDetails profileDetails = await ValidateAuthentication();
            if (profileDetails != null)
            {
                long communityId = ValidateEntityId(id);
                ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;

                OperationStatus status = communityService.DeleteCommunity(communityId, profileDetails.ID);
                return status != null && status.Succeeded;
            }
            else
            {
                throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
            }
            

            return false;
        }
        [AllowAnonymous]
        [HttpDelete]
        [Route("Resource/Service/Content/{id}")]
        public async Task<bool> DeleteContent(string id)
        {
            ProfileDetails profileDetails = await ValidateAuthentication();
                
            if (profileDetails != null)
            {
                long contentId = ValidateEntityId(id);
                    
                OperationStatus status = contentService.DeleteContent(contentId, profileDetails.ID);
                return status != null && status.Succeeded;
            }
            else
            {
                throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
            }
            

            return false;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Communities")]
        public async Task<FileStreamResult> GetMyCommunities()
        {
            Stream resultStream = null;
            ProfileDetails profileDetails = await ValidateAuthentication();
            
            if (profileDetails != null)
            {
                // Get the payload XML for My Communities.
                ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
                
                var payloadDetails = communityService.GetRootCommunities(profileDetails.ID);

                // Rewrite URL with Community and not with Folder
                RewritePayloadUrls(payloadDetails, true);
                resultStream = GetOutputStream(payloadDetails);
            }
            else
            {
                throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
            }


            return new FileStreamResult(resultStream,Response.ContentType);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Contents")]
        public async Task<FileStreamResult> GetMyContents()
        {
            Stream resultStream = null;
            ProfileDetails profileDetails = await ValidateAuthentication();
                if (profileDetails != null)
                {
                    IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
                    var payloadDetails = contentService.GetUserContents(profileDetails.ID);
                    RewritePayloadUrls(payloadDetails, false);
                    resultStream = GetOutputStream(payloadDetails);
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }


                return new FileStreamResult(resultStream, Response.ContentType); ;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Resource/Service/Content/Publish/{filename}")]
        public async Task<long> PublishContent(string filename, Stream fileContent)
        {
            long contentId = -1;
            ProfileDetails profileDetails = await ValidateAuthentication();
            if (!string.IsNullOrWhiteSpace(filename) && fileContent != null)
            {
                
                //// TODO: Check permissions if the user can add content to the parent.
                if (profileDetails != null)
                {
                    ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;

                    // If id is null then we need to get the Visitor community for the current user.
                    CommunityDetails parentCommunity = communityService.GetDefaultCommunity(profileDetails.ID);
                    switch (Path.GetExtension(filename).ToUpperInvariant())
                    {
                        case Constants.TourFileExtension:
                            // Update properties from the Tour file.
                            contentId = CreateTour(filename, fileContent, profileDetails, parentCommunity);
                            break;
                        case Constants.CollectionFileExtension:
                            // Update properties from the WTML file.
                            contentId = CreateCollection(filename, fileContent, profileDetails, parentCommunity);
                            break;
                        default:
                            contentId = CreateContent(filename, fileContent, profileDetails, parentCommunity);
                            break;
                    }
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return contentId;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Resource/Service/Content/Rate/{id}/{rating}")]
        public async Task<bool> RateContent(string id, string rating)
        {
            ProfileDetails profileDetails = await ValidateAuthentication();

            if (profileDetails != null)
            {
                long contentId = ValidateEntityId(id);
                int ratingValue = 5;
                if (int.TryParse(rating, out ratingValue))
                {
                    // Dummy Call
                }
                    
                RatingDetails ratingDetails = new RatingDetails()
                {
                    Rating = ratingValue >= 5 || ratingValue <= 0 ? 5 : ratingValue,
                    RatedByID = profileDetails.ID,
                    ParentID = contentId 
                };

                IRatingService ratingService = DependencyResolver.Current.GetService(typeof(IRatingService)) as IRatingService;
                ratingService.UpdateContentRating(ratingDetails);

                return true;
            }
            else
            {
                throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
            }

            return false;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Resource/Service/Community/Rate/{id}/{rating}")]
        public async Task<bool> RateCommunity(string id, string rating)
        {
            ProfileDetails profileDetails = await ValidateAuthentication();
            if (profileDetails != null)
                {
                    long contentId = ValidateEntityId(id);
                    int ratingValue = 5;
                    if (int.TryParse(rating, out ratingValue))
                    {
                        // Dummy Call
                    }
                    
                    RatingDetails ratingDetails = new RatingDetails()
                    {
                        Rating = ratingValue >= 5 || ratingValue <= 0 ? 5 : ratingValue,
                        RatedByID = profileDetails.ID,
                        ParentID = contentId
                    };

                    IRatingService ratingService = DependencyResolver.Current.GetService(typeof(IRatingService)) as IRatingService;
                    ratingService.UpdateCommunityRating(ratingDetails);

                    return true;
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            

            return false;
        }

        #endregion Mandatory profile check calls - Only logged in users

        #region Optional profile check calls - Logged in & anonymous users
        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Payload")]
        public FileStreamResult GetProfilePayload()
        {
            string serviceBaseUri = ServiceBaseUri();
            // If we are not doing Re-writes of Url, we need to set the thumbnails explicitly at all places
            PayloadDetails payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.Thumbnail = RewriteThumbnailUrl(payloadDetails.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Name = Resources.MyFolderLabel;

            // Get My communities node.
            var myCommunities = PayloadDetailsExtensions.InitializePayload();
            myCommunities.Name = Resources.MyCommunitiesWWTLabel;
            myCommunities.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Communities", serviceBaseUri);
            myCommunities.Thumbnail = RewriteThumbnailUrl(myCommunities.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(myCommunities);

            // Get My contents node.
            var myContents = PayloadDetailsExtensions.InitializePayload();
            myContents.Name = Resources.MyContentsWWTLabel;
            myContents.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Contents", serviceBaseUri);
            myContents.Thumbnail = RewriteThumbnailUrl(myContents.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(myContents);

            // Get Browse EO node.
            var browseDetails = PayloadDetailsExtensions.InitializePayload();
            browseDetails.Name = Resources.BrowseWWTLabel;
            browseDetails.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse", serviceBaseUri);
            browseDetails.Thumbnail = RewriteThumbnailUrl(browseDetails.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(browseDetails);

            // TODO: Get Search EO node.
            var searchEOLink = new Place();
            searchEOLink.Name = Resources.SearchWWTLabel;
            searchEOLink.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Community/index{1}", BaseUri(), "?wwtfull=true");
            searchEOLink.Permission = Permission.Reader;
            searchEOLink.Thumbnail = RewriteThumbnailUrl(searchEOLink.Thumbnail, "searchwwtthumbnail");

            // This will make sure that the additional context menu options specific to folders are not shown in WWT.
            searchEOLink.MSRComponentId = -1;

            payloadDetails.Links.Add(searchEOLink);

            // TODO: Get Help node.
            var helpLink = new Place();
            helpLink.Name = Resources.HelpWWTLabel;
            helpLink.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Support/index{1}", BaseUri(), "?wwtfull=true");
            helpLink.Permission = Permission.Reader;
            helpLink.Thumbnail = RewriteThumbnailUrl(helpLink.Thumbnail, "helpwwtthumbnail");

            // This will make sure that the additional context menu options specific to folders are not shown in WWT.
            helpLink.MSRComponentId = -1;

            payloadDetails.Links.Add(helpLink);

            var payload = new FileStreamResult(GetOutputStream(payloadDetails), Response.ContentType);
            return payload;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Community/{id}")]
        public async Task<FileStreamResult> GetCommunity(string id)
        {
            // Parameter validation can be outside in cases where Live Id validation is optional
            long communityId = ValidateEntityId(id);
            Stream resultStream = null;
            ProfileDetails profileDetails = await ValidateAuthentication();
            PayloadDetails payloadDetails;
            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
            if (profileDetails != null)
            {
                //// TODO : Permissions check
                payloadDetails = communityService.GetPayload(communityId, profileDetails.ID);
                payloadDetails.CommunityType = CommunityTypes.Community;
            }
            else
            {
                //// TODO : In case of no user information, send only public content in payload
                payloadDetails = communityService.GetPayload(communityId, null);
                payloadDetails.CommunityType = CommunityTypes.Community;
            }

            //// This has to happen before Add Tour and Latest folder additions. Otherwise their Url will also be re-written
            //// Rewrite URL with Folder in this case
            RewritePayloadUrls(payloadDetails, false);

            // TODO : Need to decide when to add 
            AddTourFolders(payloadDetails);
            resultStream = GetOutputStream(payloadDetails);
            return new FileStreamResult(resultStream, Response.ContentType);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Folder/{id}")]
        public async Task<FileStreamResult> GetFolder(string id)
        {
            // Parameter validation can be outside in cases where Live Id validation is optional
            long communityId = ValidateEntityId(id);
            Stream resultStream = null;
            ProfileDetails profileDetails = await ValidateAuthentication();
            PayloadDetails payloadDetails;
            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
            if (profileDetails != null)
            {
                // TODO : Permissions check
                payloadDetails = communityService.GetPayload(communityId, profileDetails.ID);
                payloadDetails.CommunityType = CommunityTypes.Folder;
                payloadDetails.Searchable = false.ToString();
            }
            else
            {
                // TODO : In case of no user information, send only public content in payload
                payloadDetails = communityService.GetPayload(communityId, null);
                payloadDetails.CommunityType = CommunityTypes.Folder;
                payloadDetails.Searchable = false.ToString();
            }

            RewritePayloadUrls(payloadDetails, false);
            resultStream = GetOutputStream(payloadDetails);
            return new FileStreamResult(resultStream, Response.ContentType); 
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Community/{id}/Tours")]
        public async Task<FileStreamResult> GetAllTours(string id)
        {
            // Parameter validation can be outside in cases where Live Id validation is optional
            long communityId = ValidateEntityId(id);
            Stream resultStream = null;
            ProfileDetails profileDetails = await ValidateAuthentication();
            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
            var payloadDetails = profileDetails != null ? 
                communityService.GetAllTours(communityId, profileDetails.ID) : 
                communityService.GetAllTours(communityId, null);

            RewritePayloadUrls(payloadDetails, false);
            resultStream = GetOutputStream(payloadDetails);
            return new FileStreamResult(resultStream, Response.ContentType);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Community/{id}/Latest")]
        public async Task<FileStreamResult> GetLatest(string id)
        {
            // Parameter validation can be outside in cases where Live Id validation is optional
            long communityId = ValidateEntityId(id);
            Stream resultStream = null;
            ProfileDetails profileDetails = await ValidateAuthentication();
            PayloadDetails payloadDetails;
            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;

            payloadDetails = profileDetails != null ? communityService.GetLatestContent(communityId, profileDetails.ID) : communityService.GetLatestContent(communityId, null);

            RewritePayloadUrls(payloadDetails, false);
            resultStream = GetOutputStream(payloadDetails);
            return new FileStreamResult(resultStream, Response.ContentType);
        }

        #endregion Optional profile check calls - Logged in & anonymous users

        #region No profile check calls - Anybody
        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Thumbnail/{id}")]
        public FileStreamResult GetThumbnail(string id)
        {
            Guid thumbnailId = ValidateGuid(id);
            //// TODO : Authenticate?
            IBlobService blobService = DependencyResolver.Current.GetService(typeof(IBlobService)) as IBlobService;
            var blobDetails = blobService.GetThumbnail(thumbnailId);
            if (blobDetails != null && blobDetails.Data != null)
            {
                // Update the size of the thumbnail to 160 X 96 and upload it to temporary container in Azure.
                blobDetails.Data = blobDetails.Data.GenerateThumbnail(Constants.DefaultClientThumbnailWidth, Constants.DefaultClientThumbnailHeight, Constants.DefaultThumbnailImageFormat);
                blobDetails.MimeType = Constants.DefaultThumbnailMimeType;
                var resultStream = GetFileStream(blobDetails);
                return new FileStreamResult(resultStream, Response.ContentType);
            }

            return null;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/File/{id}")]
        public FileStreamResult GetFile(string id)
        {
            Guid contentId = ValidateGuid(id);
            //// TODO : Authenticate?
            IBlobService blobService = DependencyResolver.Current.GetService(typeof(IBlobService)) as IBlobService;
            var blobDetails = blobService.GetFile(contentId);
            
            if (blobDetails != null && blobDetails.Data != null)
            {
                Stream resultStream =  GetFileStream(blobDetails);
                return new FileStreamResult(resultStream, Response.ContentType);
            }

            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Resource/Service/Download/{id}/{name}")]
        public FileStreamResult DownloadFile(string id, string name)
        {
            Guid contentId = ValidateGuid(id);
            //// TODO : Authenticate?
            IBlobService blobService = DependencyResolver.Current.GetService(typeof(IBlobService)) as IBlobService;
            var blobDetails = blobService.GetFile(contentId);
            if (blobDetails != null && blobDetails.Data != null)
            {
                Response.Headers.Add("Content-Encoding", blobDetails.MimeType);
                Response.Headers.Add("content-disposition", "attachment;filename=" + name);
                Stream resultStream = GetFileStream(blobDetails);
                return new FileStreamResult(resultStream, Response.ContentType);
            }

            return null;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Browse")]
        public FileStreamResult GetBrowsePayload()
        {
            string baseUri = BaseUri();
            string serviceUri = ServiceBaseUri();
            PayloadDetails payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.Name = "Browse";
            payloadDetails.Thumbnail = RewriteThumbnailUrl(payloadDetails.Thumbnail, "defaultfolderwwtthumbnail");

            // Get Latest Content.
            var latestContent = PayloadDetailsExtensions.InitializePayload();
            latestContent.Name = "Latest Content";
            latestContent.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/LatestContent", serviceUri);
            latestContent.Thumbnail = RewriteThumbnailUrl(latestContent.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(latestContent);

            // Get Top Rated Content.
            var topRatedContent = PayloadDetailsExtensions.InitializePayload();
            topRatedContent.Name = "Top Rated Content";
            topRatedContent.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/TopRatedContent", serviceUri);
            topRatedContent.Thumbnail = RewriteThumbnailUrl(topRatedContent.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(topRatedContent);

            // Get Top Downloaded Content
            var topDownloadedContent = PayloadDetailsExtensions.InitializePayload();
            topDownloadedContent.Name = "Most Downloaded Content";
            topDownloadedContent.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/MostDownloadedContent", serviceUri);
            topDownloadedContent.Thumbnail = RewriteThumbnailUrl(topDownloadedContent.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(topDownloadedContent);

            // Get Latest Community.
            var latestCommunity = PayloadDetailsExtensions.InitializePayload();
            latestCommunity.Name = "Latest Communities";
            latestCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/LatestCommunity", serviceUri);
            latestCommunity.Thumbnail = RewriteThumbnailUrl(latestCommunity.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(latestCommunity);

            // Get Top Rated Community.
            var topRatedCommunity = PayloadDetailsExtensions.InitializePayload();
            topRatedCommunity.Name = "Top Rated Communities";
            topRatedCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/TopRatedCommunity", serviceUri);
            topRatedCommunity.Thumbnail = RewriteThumbnailUrl(topRatedCommunity.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(topRatedCommunity);

            // Get Categories.
            var categories = PayloadDetailsExtensions.InitializePayload();
            categories.Name = "Categories";
            categories.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/Categories", serviceUri);
            categories.Thumbnail = RewriteThumbnailUrl(categories.Thumbnail, "defaultfolderwwtthumbnail");
            payloadDetails.Children.Add(categories);

            Stream resultStream = GetOutputStream(payloadDetails);
            return new FileStreamResult(resultStream, Response.ContentType);
            
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Browse/MostDownloadedContent")]
        public FileStreamResult GetTopDownloadedContent()
        {
            Stream resultStream = GetTopContent(new EntityHighlightFilter(HighlightType.MostDownloaded, CategoryType.All, null));
            return new FileStreamResult(resultStream, Response.ContentType);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Browse/TopRatedContent")]
        public FileStreamResult GetTopRatedContent()
        {
            Stream resultStream = GetTopContent(new EntityHighlightFilter(HighlightType.Popular, CategoryType.All, null));
            return new FileStreamResult(resultStream, Response.ContentType);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Browse/LatestContent")]
        public FileStreamResult GetLatestContent()
        {
            Stream resultStream = GetTopContent(new EntityHighlightFilter(HighlightType.Latest, CategoryType.All, null));
            return new FileStreamResult(resultStream, Response.ContentType);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Browse/TopRatedCommunity")]
        public FileStreamResult GetTopRatedCommunity()
        {
            Stream resultStream = GetTopCommunity(new EntityHighlightFilter(HighlightType.Popular, CategoryType.All, null));
            return new FileStreamResult(resultStream, Response.ContentType);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Browse/LatestCommunity")]
        public FileStreamResult GetLatestCommunity()
        {
            Stream resultStream = GetTopCommunity(new EntityHighlightFilter(HighlightType.Latest, CategoryType.All, null));
            return new FileStreamResult(resultStream, Response.ContentType);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Browse/Categories")]
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Not locale specific")]
        public FileStreamResult GetCategories()
        {
            var categories = CategoryType.All.ToSelectList(CategoryType.All);
            PayloadDetails payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.Name = "Categories";
            payloadDetails.Permission = Permission.Reader;
            payloadDetails.SetValuesFrom(categories);

            string baseUri = BaseUri();
            foreach (var childCategory in payloadDetails.Children)
            {
                childCategory.Thumbnail = RewriteThumbnailUrl(childCategory.Thumbnail, childCategory.Id.ToEnum<string, CategoryType>(CategoryType.GeneralInterest).ToString().ToLowerInvariant() + "wwtthumbnail");
                childCategory.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Resource/Service/Browse/Category/{1}", baseUri, childCategory.Id);
            }
            Stream resultStream = GetOutputStream(payloadDetails);
            return new FileStreamResult(resultStream, Response.ContentType);

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Resource/Service/Browse/Category/{id}")]
        public FileStreamResult GetCategory(string id)
        {
            long categoryId = ValidateEntityId(id);
            Stream resultStream = GetTopContent(new EntityHighlightFilter(HighlightType.MostDownloaded, ((int)categoryId).ToEnum<int, CategoryType>(CategoryType.All), null));
            return new FileStreamResult(resultStream, Response.ContentType);
        }

        #endregion No profile check calls - Anybody

        #endregion

        protected string BaseUri()
        {
            return string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
        }

        protected string ServiceBaseUri()
        {
            return string.Format("{0}://{1}/Resource/Service", Request.Url.Scheme, Request.Url.Authority);
        }

        #region Private static methods


        /// <summary>
        /// Rewrites the Thumbnail path and the URL of the Payload for the community.
        /// </summary>
        /// <param name="payloadDetails">PayloadDetails object</param>
        /// <param name="hasChildCommunities"></param>
        
        private void RewritePayloadUrls(PayloadDetails payloadDetails, bool hasChildCommunities)
        {
            var baseUri = BaseUri();

            if (payloadDetails.CommunityType == CommunityTypes.Community)
            {
                payloadDetails.Thumbnail = RewriteThumbnailUrl(payloadDetails.Thumbnail, "defaultcommunitywwtthumbnail");
            }
            else
            {
                payloadDetails.Thumbnail = RewriteThumbnailUrl(payloadDetails.Thumbnail, "defaultfolderwwtthumbnail");
            }

            // Update Place Urls
            foreach (var place in payloadDetails.Links)
            {
                place.Thumbnail = RewriteThumbnailUrl(place.Thumbnail, place.FileType.GetContentTypeImage());
                var fileExt = "none";
                if (place.Url.Contains(".") && place.FileType != ContentTypes.Link)
                {
                    fileExt = place.Url.Substring(place.Url.LastIndexOf(".") + 1);
                }
                
                place.Url = place.FileType == ContentTypes.Link
                    ? place.ContentLink
                    : string.Format(CultureInfo.InvariantCulture, "{0}/File/Download/{1}/{2}/{3}/{4}", baseUri,
                        place.ContentAzureID, Uri.EscapeDataString(place.Name.Replace("&", "and")), fileExt,
                        fileExt == "wtml" ? "wwtfull=true":string.Empty); 
            }

            // Update Content Urls
            foreach (var childCommunity in payloadDetails.Children)
            {
                if (childCommunity.IsCollection)
                {
                    // Only for collection set the default thumbnail, not for folders.
                    childCommunity.Thumbnail = RewriteThumbnailUrl(childCommunity.Thumbnail, "defaultwtmlwwtthumbnail");
                    childCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/File/Download/{1}/ChildCommunityData", baseUri, childCommunity.Id);
                }
                else
                {
                    if (hasChildCommunities)
                    {
                        childCommunity.Thumbnail = RewriteThumbnailUrl(childCommunity.Thumbnail, "defaultcommunitywwtthumbnail");
                        childCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Community/{1}", ServiceBaseUri(), childCommunity.Id);
                    }
                    else
                    {
                        if (childCommunity.CommunityType == CommunityTypes.Community)
                        {
                            childCommunity.Thumbnail = RewriteThumbnailUrl(childCommunity.Thumbnail, "defaultcommunitywwtthumbnail");
                        }
                        else
                        {
                            childCommunity.Thumbnail = RewriteThumbnailUrl(childCommunity.Thumbnail, "defaultfolderwwtthumbnail");
                        }

                        childCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Folder/{1}", ServiceBaseUri(), childCommunity.Id);
                    }
                }
            }

            // Get profile details of all the user who have created the tours
            IEnumerable<ProfileDetails> profileDetails = new List<ProfileDetails>();
            if (payloadDetails.Tours.Any())
            {
                // AuthorImageUrl has the created by ID of the user. This needs to be replaced with the URL for users picture
                var userList = payloadDetails.Tours.Select(item => item.AuthorImageUrl).ToList().ConvertAll<long>(Convert.ToInt64).AsEnumerable();
                profileDetails = profileService.GetProfiles(userList.Distinct());
            }

            // Update Tour Urls including thumbnail, author and author image
            foreach (var tour in payloadDetails.Tours)
            {
                tour.ThumbnailUrl = RewriteThumbnailUrl(tour.ThumbnailUrl, "defaulttourswwtthumbnail");
                tour.TourUrl = string.Format(CultureInfo.InvariantCulture, "{0}/File/Download/{1}/{2}/wtt/?wwtfull=true", baseUri, tour.TourUrl, Uri.EscapeDataString(tour.Title.Replace("&", "and")));
                tour.AuthorURL = string.Format(CultureInfo.InvariantCulture, "{0}/Profile/Index/{1}", baseUri, tour.AuthorURL);

                // Get profile picture ID using AuthorImageUrl which is CreatedByID value.
                var firstOrDefault = profileDetails.FirstOrDefault(item => item.ID.ToString(CultureInfo.InvariantCulture) == tour.AuthorImageUrl);
                if (firstOrDefault != null)
                {
                    var pictureID = firstOrDefault.PictureID;
                    if (pictureID.HasValue)
                    {
                        tour.AuthorImageUrl = string.Format(CultureInfo.InvariantCulture, "{0}/File/Thumbnail/{1}", baseUri, pictureID.Value);
                    }
                    else 
                    {
                        // return default profile image
                        tour.AuthorImageUrl = RewriteThumbnailUrl(string.Empty, "wwtprofile");
                    }
                }
            }
        }

        /// <summary>
        /// Rewrites the thumbnail URL for the payload XML. In case if the thumbnail is provided, proper URL to access the thumbnail 
        /// using the File controller and thumbnail action will be formed and returned.
        /// In case the thumbnail is not provided and default image is provided, then default thumbnail URL will be formed and returned.
        /// In other cases (both thumbnail and default image is not provided), same thumbnail will be returned.
        /// </summary>
        /// <param name="thumbnail">Thumbnail string to be verified</param>
        /// <param name="defaultImage">Default image name</param>
        /// <param name="context"></param>
        /// <returns>Rewritten thumbnail URL</returns>
        private string RewriteThumbnailUrl(string thumbnail, string defaultImage)
        {
            if (!string.IsNullOrWhiteSpace(thumbnail))
            {
                thumbnail = string.Format(CultureInfo.InvariantCulture, "{0}/File/Thumbnail/{1}", BaseUri(), thumbnail);
            }
            else if (!string.IsNullOrWhiteSpace(defaultImage))
            {
                thumbnail = string.Format(CultureInfo.InvariantCulture, "{0}/Content/Images/{1}.png", BaseUri(), defaultImage);
            }

            return thumbnail;
        }

        /// <summary>
        /// Adds all tours and latest tours to the Payload for the community.
        /// </summary>
        /// <param name="payloadDetails">PayloadDetails object</param>
        private void AddTourFolders(PayloadDetails payloadDetails)
        {
            string baseUri = ServiceBaseUri();
            var allTours = PayloadDetailsExtensions.InitializePayload();
            allTours.Name = "All Tours";
            allTours.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Community/{1}/Tours", baseUri, payloadDetails.Id);

            var latestTours = PayloadDetailsExtensions.InitializePayload();
            latestTours.Name = "Latest";
            latestTours.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Community/{1}/Latest", baseUri, payloadDetails.Id);

            // Insert tour folders at the start
            payloadDetails.Children.Insert(0, latestTours);
            payloadDetails.Children.Insert(0, allTours);
        }

        /// <summary>
        /// Get File stream for all the tours in a folder
        /// </summary>
        /// <param name="payloadDetails">payload Details object</param>
        /// <returns>Result Stream.</returns>
        private Stream GetOutputStream(PayloadDetails payloadDetails)
        {
            Response.ContentType = "application/xml";
            var stream = payloadDetails.GetXmlStream();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Gets the file stream based on the blobDetails
        /// </summary>
        /// <param name="blobDetails">Details of the blob</param>
        /// <returns>File Stream Result </returns>
        private Stream GetFileStream(BlobDetails blobDetails)
        {
            // Update the response header.
            Response.ContentType = blobDetails.MimeType;

            // Set the position to Begin.
            blobDetails.Data.Seek(0, SeekOrigin.Begin);
            return blobDetails.Data;
        }

       

        private long CreateContent(string filename, Stream fileContent, ProfileDetails profileDetails, CommunityDetails parentCommunity)
        {
            ContentDetails content = null;

            // No need to get into a memory stream as the stream is sent as is and need not be loaded
            using (Stream writablefileContent = new MemoryStream())
            {
                fileContent.CopyTo(writablefileContent);
                writablefileContent.Position = 0;
                content = GetContentDetail(filename, writablefileContent, profileDetails, parentCommunity);
                content.Name = Path.GetFileNameWithoutExtension(filename);
            }

            IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
            long contentID = content.ID = contentService.CreateContent(content);

            if (contentID > 0)
            {
                INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
                notificationService.NotifyNewEntityRequest(content, BaseUri() + "/");
            }

            return contentID;
        }

        private long CreateTour(string filename, Stream fileContent, ProfileDetails profileDetails, CommunityDetails parentCommunity)
        {
            XmlDocument tourDoc = new XmlDocument();
            ContentDetails content = null;

            // NetworkStream is not Seek able. Need to load into memory stream so that it can be sought
            using (Stream writablefileContent = new MemoryStream())
            {
                fileContent.CopyTo(writablefileContent);
                writablefileContent.Position = 0;
                content = GetContentDetail(filename, writablefileContent, profileDetails, parentCommunity);
                writablefileContent.Position = 0;
                tourDoc = tourDoc.SetXmlFromTour(writablefileContent);
            }

            if (tourDoc != null)
            {
                // Note that the spelling of Description is wrong because that's how WWT generates the WTT file.
                content.Name = tourDoc.GetAttributeValue("Tour", "Title");
                content.Description = tourDoc.GetAttributeValue("Tour", "Descirption");
                content.DistributedBy = tourDoc.GetAttributeValue("Tour", "Author");
                content.TourLength = tourDoc.GetAttributeValue("Tour", "RunTime");
            }

            IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
            long contentID = content.ID = contentService.CreateContent(content);

            if (contentID > 0)
            {
                INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
                notificationService.NotifyNewEntityRequest(content, BaseUri() + "/");
            }

            return contentID;
        }

        private long CreateCollection(string filename, Stream fileContent, ProfileDetails profileDetails, CommunityDetails parentCommunity)
        {
            // Get name/ tags/ category from Tour.
            XmlDocument collectionDoc = new XmlDocument();
            ContentDetails content = null;

            // NetworkStream is not Seek able. Need to load into memory stream so that it can be sought
            using (Stream writablefileContent = new MemoryStream())
            {
                fileContent.CopyTo(writablefileContent);
                writablefileContent.Position = 0;
                content = GetContentDetail(filename, writablefileContent, profileDetails, parentCommunity);
                writablefileContent.Seek(0, SeekOrigin.Begin);
                collectionDoc = collectionDoc.SetXmlFromWtml(writablefileContent);
            }

            if (collectionDoc != null)
            {
                content.Name = collectionDoc.GetAttributeValue("Folder", "Name");
            }

            IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
            long contentID = content.ID = contentService.CreateContent(content);

            if (contentID > 0)
            {
                INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
                notificationService.NotifyNewEntityRequest(content, BaseUri() + "/");
            }

            return contentID;
        }

        private ContentDetails GetContentDetail(string filename, Stream fileContent, ProfileDetails profileDetails, CommunityDetails parentCommunity)
        {
            ContentDetails content = new ContentDetails();

            var fileDetail = UpdateFileDetails(filename, fileContent);

            content.ContentData = fileDetail;
            content.CreatedByID = profileDetails.ID;

            // By Default the access type will be private.
            content.AccessTypeID = (int)AccessType.Public;

            // Get the Category/Tags from Parent.
            content.ParentID = parentCommunity.ID;
            content.ParentType = parentCommunity.CommunityType;
            content.CategoryID = parentCommunity.CategoryID;

            return content;
        }

        private FileDetail UpdateFileDetails(string filename, Stream fileContent)
        {
            IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;

            var fileDetail = new FileDetail();
            fileDetail.SetValuesFrom(filename, fileContent);

            // TODO: Update mime type and size from the incoming request.
            fileDetail.MimeType = Path.GetExtension(filename).GetWwtMimeType(Request.ContentType);
            fileDetail.Size = Request.ContentLength;

            contentService.UploadTemporaryFile(fileDetail);

            return fileDetail;
        }


        private Stream GetTopContent(EntityHighlightFilter entityHighlightFilter)
        {
            IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
            PageDetails pageDetails = new PageDetails(1);
            pageDetails.ItemsPerPage = 20;
            var contents = entityService.GetContents(entityHighlightFilter, pageDetails);

            var payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.SetValuesFrom(contents);

            RewritePayloadUrls(payloadDetails, false);
            return GetOutputStream(payloadDetails);
        }

        private Stream GetTopCommunity(EntityHighlightFilter entityHighlightFilter)
        {
            IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
            PageDetails pageDetails = new PageDetails(1);
            pageDetails.ItemsPerPage = 20;
            var communities = entityService.GetCommunities(entityHighlightFilter, pageDetails);

            var payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.SetValuesFrom(communities);

            RewritePayloadUrls(payloadDetails, false);
            return GetOutputStream(payloadDetails);
        }


        #endregion
    }
}
