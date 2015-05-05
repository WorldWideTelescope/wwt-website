//-----------------------------------------------------------------------
// Moved to resourcescontroller.cs
//-----------------------------------------------------------------------

//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.ServiceModel.Activation;
//using System.ServiceModel.Web;
//using System.Web;
//using System.Web.Mvc;
//using System.Xml;
//using WWTMVC5.Extensions;
//using WWTMVC5.Models;
//using WWTMVC5.Properties;
//using WWTMVC5.Services.Interfaces;

//namespace WWTMVC5.WebServices
//{
//    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
//    public class ResourceService : ServiceBase, IResourceService
//    {
//       private string _baseUri;
//        public ResourceService(string baseUri)
//        {
//            _baseUri = baseUri;
//        }

//        #region Web Methods

//        #region Mandatory profile check calls - Only logged in users

//        public bool CheckIfUserExists()
//        {
//            ProfileDetails profileDetails;

//            // Throw web fault exception for calls that need Live ID token
//            if (ValidateAuthentication(true, out profileDetails))
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                profileDetails = profileService.GetProfile(profileDetails.PUID);
//                if (profileDetails != null)
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        public bool RegisterUser()
//        {
//            ProfileDetails profileDetails;

//            // Throw web fault exception for calls that need Live ID token
//            if (ValidateAuthentication(true, out profileDetails))
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                var profile = profileService.GetProfile(profileDetails.PUID);
//                if (profile == null)
//                {
//                    // While creating the user, IsSubscribed to be true always.
//                    profileDetails.IsSubscribed = true;

//                    // Create User in DB.
//                    profileDetails.ID = profileService.CreateProfile(profileDetails);

//                    // Create Default Community for user.
//                    CreateDefaultUserCommunity(profileDetails.ID);

//                    // Send new user registered notification.
//                    if (profileDetails.ID > 0)
//                    {
//                        INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
//                        notificationService.NotifyNewEntityRequest(profileDetails, _baseUri + "/");
//                    }

//                    return true;
//                }
//                else
//                {
//                    throw new WebFaultException<string>("User already registered", HttpStatusCode.BadRequest);
//                }
//            }

//            return false;
//        }

//        public bool DeleteCommunity(string id)
//        {
//            ProfileDetails profileDetails;

//            if (ValidateAuthentication(true, out profileDetails))
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                profileDetails = profileService.GetProfile(profileDetails.PUID);
//                if (profileDetails != null)
//                {
//                    long communityId = ValidateEntityId(id);
//                    ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;

//                    OperationStatus status = communityService.DeleteCommunity(communityId, profileDetails.ID);
//                    return status != null && status.Succeeded;
//                }
//                else
//                {
//                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
//                }
//            }

//            return false;
//        }

//        public bool DeleteContent(string id)
//        {
//            ProfileDetails profileDetails;

//            if (ValidateAuthentication(true, out profileDetails))
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                profileDetails = profileService.GetProfile(profileDetails.PUID);
//                if (profileDetails != null)
//                {
//                    long contentId = ValidateEntityId(id);
//                    IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;

//                    OperationStatus status = contentService.DeleteContent(contentId, profileDetails.ID);
//                    return status == null ? false : status.Succeeded;
//                }
//                else
//                {
//                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
//                }
//            }

//            return false;
//        }

//        public Stream GetMyCommunities()
//        {
//            Stream resultStream = null;
//            ProfileDetails profileDetails;

//            if (ValidateAuthentication(true, out profileDetails))
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                profileDetails = profileService.GetProfile(profileDetails.PUID);
//                if (profileDetails != null)
//                {
//                    // Get the payload XML for My Communities.
//                    ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
//                    var payloadDetails = communityService.GetRootCommunities(profileDetails.ID);

//                    // Rewrite URL with Community and not with Folder
//                    RewritePayloadUrls(payloadDetails, true, _baseUri);
//                    resultStream = GetOutputStream(payloadDetails);
//                }
//                else
//                {
//                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
//                }
//            }

//            return resultStream;
//        }

//        public Stream GetMyContents()
//        {
//            Stream resultStream = null;
//            ProfileDetails profileDetails;

//            if (ValidateAuthentication(true, out profileDetails))
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                profileDetails = profileService.GetProfile(profileDetails.PUID);
//                if (profileDetails != null)
//                {
//                    IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
//                    var payloadDetails = contentService.GetUserContents(profileDetails.ID);
//                    RewritePayloadUrls(payloadDetails, false,_baseUri);
//                    resultStream = GetOutputStream(payloadDetails);
//                }
//                else
//                {
//                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
//                }
//            }

//            return resultStream;
//        }

//        public long PublishContent(string filename, Stream fileContent)
//        {
//            long contentId = -1;
//            ProfileDetails profileDetails;
//            if (!string.IsNullOrWhiteSpace(filename) && fileContent != null && ValidateAuthentication(false, out profileDetails))
//            {
//                // Get Profile details.
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                profileDetails = profileService.GetProfile(profileDetails.PUID);
//                //// TODO: Check permissions if the user can add content to the parent.
//                if (profileDetails != null)
//                {
//                    ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;

//                    // If id is null then we need to get the Visitor community for the current user.
//                    CommunityDetails parentCommunity = communityService.GetDefaultCommunity(profileDetails.ID);
//                    switch (Path.GetExtension(filename).ToUpperInvariant())
//                    {
//                        case Constants.TourFileExtension:
//                            // Update properties from the Tour file.
//                            contentId = CreateTour(filename, fileContent, profileDetails, parentCommunity,_baseUri);
//                            break;
//                        case Constants.CollectionFileExtension:
//                            // Update properties from the WTML file.
//                            contentId = CreateCollection(filename, fileContent, profileDetails, parentCommunity, _baseUri);
//                            break;
//                        default:
//                            contentId = CreateContent(filename, fileContent, profileDetails, parentCommunity,_baseUri);
//                            break;
//                    }
//                }
//                else
//                {
//                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
//                }
//            }

//            return contentId;
//        }

//        public bool RateContent(string id, string rating)
//        {
//            ProfileDetails profileDetails;

//            if (ValidateAuthentication(true, out profileDetails))
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                profileDetails = profileService.GetProfile(profileDetails.PUID);
//                if (profileDetails != null)
//                {
//                    long contentId = ValidateEntityId(id);
//                    int ratingValue = 5;
//                    if (int.TryParse(rating, out ratingValue))
//                    {
//                        // Dummy Call
//                    }
                    
//                    RatingDetails ratingDetails = new RatingDetails()
//                    {
//                        Rating = ratingValue >= 5 || ratingValue <= 0 ? 5 : ratingValue,
//                        RatedByID = profileDetails.ID,
//                        ParentID = contentId 
//                    };

//                    IRatingService ratingService = DependencyResolver.Current.GetService(typeof(IRatingService)) as IRatingService;
//                    ratingService.UpdateContentRating(ratingDetails);

//                    return true;
//                }
//                else
//                {
//                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
//                }
//            }

//            return false;
//        }

//        public bool RateCommunity(string id, string rating)
//        {
//            ProfileDetails profileDetails;
//            if (ValidateAuthentication(true, out profileDetails))
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
//                profileDetails = profileService.GetProfile(profileDetails.PUID);
//                if (profileDetails != null)
//                {
//                    long contentId = ValidateEntityId(id);
//                    int ratingValue = 5;
//                    if (int.TryParse(rating, out ratingValue))
//                    {
//                        // Dummy Call
//                    }
                    
//                    RatingDetails ratingDetails = new RatingDetails()
//                    {
//                        Rating = ratingValue >= 5 || ratingValue <= 0 ? 5 : ratingValue,
//                        RatedByID = profileDetails.ID,
//                        ParentID = contentId
//                    };

//                    IRatingService ratingService = DependencyResolver.Current.GetService(typeof(IRatingService)) as IRatingService;
//                    ratingService.UpdateCommunityRating(ratingDetails);

//                    return true;
//                }
//                else
//                {
//                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
//                }
//            }

//            return false;
//        }

//        #endregion Mandatory profile check calls - Only logged in users

//        #region Optional profile check calls - Logged in & anonymous users

//        public Stream GetProfilePayload()
//        {
//            return GetMyPayload(_baseUri);
//        }

//        public Stream GetCommunity(string id)
//        {
//            // Parameter validation can be outside in cases where Live Id validation is optional
//            long communityId = ValidateEntityId(id);
//            Stream resultStream = null;
//            ProfileDetails profileDetails;
//            PayloadDetails payloadDetails;
//            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
//            if (ValidateAuthentication(false, out profileDetails))
//            {
//                //// TODO : Permissions check
//                payloadDetails = communityService.GetPayload(communityId, profileDetails.ID);
//                payloadDetails.CommunityType = CommunityTypes.Community;
//            }
//            else
//            {
//                //// TODO : In case of no user information, send only public content in payload
//                payloadDetails = communityService.GetPayload(communityId, null);
//                payloadDetails.CommunityType = CommunityTypes.Community;
//            }

//            //// This has to happen before Add Tour and Latest folder additions. Otherwise their Url will also be re-written
//            //// Rewrite URL with Folder in this case
//            RewritePayloadUrls(payloadDetails, false, _baseUri);

//            // TODO : Need to decide when to add 
//            AddTourFolders(payloadDetails, _baseUri);
//            resultStream = GetOutputStream(payloadDetails);
//            return resultStream;
//        }

//        public Stream GetFolder(string id)
//        {
//            // Parameter validation can be outside in cases where Live Id validation is optional
//            long communityId = ValidateEntityId(id);
//            Stream resultStream = null;
//            ProfileDetails profileDetails;
//            PayloadDetails payloadDetails;
//            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
//            if (ValidateAuthentication(false, out profileDetails))
//            {
//                // TODO : Permissions check
//                payloadDetails = communityService.GetPayload(communityId, profileDetails.ID);
//                payloadDetails.CommunityType = CommunityTypes.Folder;
//                payloadDetails.Searchable = false.ToString();
//            }
//            else
//            {
//                // TODO : In case of no user information, send only public content in payload
//                payloadDetails = communityService.GetPayload(communityId, null);
//                payloadDetails.CommunityType = CommunityTypes.Folder;
//                payloadDetails.Searchable = false.ToString();
//            }

//            RewritePayloadUrls(payloadDetails, false, _baseUri);
//            resultStream = GetOutputStream(payloadDetails);
//            return resultStream;
//        }

//        public Stream GetAllTours(string id)
//        {
//            // Parameter validation can be outside in cases where Live Id validation is optional
//            long communityId = ValidateEntityId(id);
//            Stream resultStream = null;
//            ProfileDetails profileDetails;
//            PayloadDetails payloadDetails;
//            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
//            if (ValidateAuthentication(false, out profileDetails))
//            {
//                // TODO: Get all tours for a given User.
//                payloadDetails = communityService.GetAllTours(communityId, profileDetails.ID);
//            }
//            else
//            {
//                payloadDetails = communityService.GetAllTours(communityId, null);
//            }

//            RewritePayloadUrls(payloadDetails, false, _baseUri);
//            resultStream = GetOutputStream(payloadDetails);
//            return resultStream;
//        }

//        public Stream GetLatest(string id)
//        {
//            // Parameter validation can be outside in cases where Live Id validation is optional
//            long communityId = ValidateEntityId(id);
//            Stream resultStream = null;
//            ProfileDetails profileDetails;
//            PayloadDetails payloadDetails;
//            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;

//            if (ValidateAuthentication(false, out profileDetails))
//            {
//                // TODO: Get all tours for a given User.
//                payloadDetails = communityService.GetLatestContent(communityId, profileDetails.ID);
//            }
//            {
//                payloadDetails = communityService.GetLatestContent(communityId, null);
//            }

//            RewritePayloadUrls(payloadDetails, false, _baseUri);
//            resultStream = GetOutputStream(payloadDetails);
//            return resultStream;
//        }

//        #endregion Optional profile check calls - Logged in & anonymous users

//        #region No profile check calls - Anybody

//        public Stream GetThumbnail(string id)
//        {
//            Guid thumbnailId = ValidateGuid(id);
//            //// TODO : Authenticate?
//            IBlobService blobService = DependencyResolver.Current.GetService(typeof(IBlobService)) as IBlobService;
//            var blobDetails = blobService.GetThumbnail(thumbnailId);
//            if (blobDetails != null && blobDetails.Data != null)
//            {
//                // Update the size of the thumbnail to 160 X 96 and upload it to temporary container in Azure.
//                blobDetails.Data = blobDetails.Data.GenerateThumbnail(Constants.DefaultClientThumbnailWidth, Constants.DefaultClientThumbnailHeight, Constants.DefaultThumbnailImageFormat);
//                blobDetails.MimeType = Constants.DefaultThumbnailMimeType;

//                return GetFileStream(blobDetails);
//            }

//            return null;
//        }

//        public Stream GetFile(string id)
//        {
//            Guid contentId = ValidateGuid(id);
//            //// TODO : Authenticate?
//            IBlobService blobService = DependencyResolver.Current.GetService(typeof(IBlobService)) as IBlobService;
//            var blobDetails = blobService.GetFile(contentId);
//            if (blobDetails != null && blobDetails.Data != null)
//            {
//                return GetFileStream(blobDetails);
//            }

//            return null;
//        }

//        public Stream DownloadFile(string id, string name, string wwtfull)
//        {
//            Guid contentId = ValidateGuid(id);
//            //// TODO : Authenticate?
//            IBlobService blobService = DependencyResolver.Current.GetService(typeof(IBlobService)) as IBlobService;
//            var blobDetails = blobService.GetFile(contentId);
//            if (blobDetails != null && blobDetails.Data != null)
//            {
//                //OutgoingWebResponseContext context = _context.OutgoingResponse;
//                //context.Headers.Add("Content-Encoding", blobDetails.MimeType);
//                //context.Headers.Add("content-disposition", "attachment;filename=" + name);

//                return GetFileStream(blobDetails);
//            }

//            return null;
//        }

//        public Stream GetBrowsePayload()
//        {
//            string baseUri = _baseUri;
//            PayloadDetails payloadDetails = PayloadDetailsExtensions.InitializePayload();
//            payloadDetails.Name = "Browse";
//            payloadDetails.Thumbnail = RewriteThumbnailUrl(payloadDetails.Thumbnail, "defaultfolderwwtthumbnail", baseUri);

//            // Get Latest Content.
//            var latestContent = PayloadDetailsExtensions.InitializePayload();
//            latestContent.Name = "Latest Content";
//            latestContent.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/LatestContent", baseUri);
//            latestContent.Thumbnail = RewriteThumbnailUrl(latestContent.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(latestContent);

//            // Get Top Rated Content.
//            var topRatedContent = PayloadDetailsExtensions.InitializePayload();
//            topRatedContent.Name = "Top Rated Content";
//            topRatedContent.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/TopRatedContent", baseUri);
//            topRatedContent.Thumbnail = RewriteThumbnailUrl(topRatedContent.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(topRatedContent);

//            // Get Top Downloaded Content
//            var topDownloadedContent = PayloadDetailsExtensions.InitializePayload();
//            topDownloadedContent.Name = "Most Downloaded Content";
//            topDownloadedContent.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/MostDownloadedContent", baseUri);
//            topDownloadedContent.Thumbnail = RewriteThumbnailUrl(topDownloadedContent.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(topDownloadedContent);

//            // Get Latest Community.
//            var latestCommunity = PayloadDetailsExtensions.InitializePayload();
//            latestCommunity.Name = "Latest Communities";
//            latestCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/LatestCommunity", baseUri);
//            latestCommunity.Thumbnail = RewriteThumbnailUrl(latestCommunity.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(latestCommunity);

//            // Get Top Rated Community.
//            var topRatedCommunity = PayloadDetailsExtensions.InitializePayload();
//            topRatedCommunity.Name = "Top Rated Communities";
//            topRatedCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/TopRatedCommunity", baseUri);
//            topRatedCommunity.Thumbnail = RewriteThumbnailUrl(topRatedCommunity.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(topRatedCommunity);

//            // Get Categories.
//            var categories = PayloadDetailsExtensions.InitializePayload();
//            categories.Name = "Categories";
//            categories.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/Categories", baseUri);
//            categories.Thumbnail = RewriteThumbnailUrl(categories.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(categories);

//            return GetOutputStream(payloadDetails);
//        }

//        public Stream GetTopDownloadedContent()
//        {
//            return GetTopContent(new EntityHighlightFilter(HighlightType.MostDownloaded, CategoryType.All, null), _baseUri);
//        }

//        public Stream GetTopRatedContent()
//        {
//            return GetTopContent(new EntityHighlightFilter(HighlightType.Popular, CategoryType.All, null), _baseUri);
//        }

//        public Stream GetLatestContent()
//        {
//            return GetTopContent(new EntityHighlightFilter(HighlightType.Latest, CategoryType.All, null), _baseUri);
//        }

//        public Stream GetTopRatedCommunity()
//        {
//            return GetTopCommunity(new EntityHighlightFilter(HighlightType.Popular, CategoryType.All, null), _baseUri);
//        }

//        public Stream GetLatestCommunity()
//        {
//            return GetTopCommunity(new EntityHighlightFilter(HighlightType.Latest, CategoryType.All, null), _baseUri);
//        }

//        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Not locale specific")]
//        public Stream GetCategories()
//        {
//            var categories = CategoryType.All.ToSelectList(CategoryType.All);
//            PayloadDetails payloadDetails = PayloadDetailsExtensions.InitializePayload();
//            payloadDetails.Name = "Categories";
//            payloadDetails.Permission = Permission.Reader;
//            payloadDetails.SetValuesFrom(categories);

//            string baseUri = _baseUri;
//            foreach (var childCategory in payloadDetails.Children)
//            {
//                childCategory.Thumbnail = RewriteThumbnailUrl(childCategory.Thumbnail, childCategory.Id.ToEnum<string, CategoryType>(CategoryType.GeneralInterest).ToString().ToLowerInvariant() + "wwtthumbnail", _baseUri);
//                childCategory.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse/Category/{1}", baseUri, childCategory.Id);
//            }

//            return GetOutputStream(payloadDetails);
//        }

//        public Stream GetCategory(string id)
//        {
//            long categoryId = ValidateEntityId(id);
//            return GetTopContent(new EntityHighlightFilter(HighlightType.MostDownloaded, ((int)categoryId).ToEnum<int, CategoryType>(CategoryType.All), null), _baseUri);
//        }

//        #endregion No profile check calls - Anybody

//        #endregion

//        #region Private static methods

//        /// <summary>
//        /// Creates default user community.
//        /// </summary>
//        /// <param name="userID">User identity.</param>
//        private static void CreateDefaultUserCommunity(long userID)
//        {
//            // This will used as the default community when user is uploading a new content.
//            // This community will need to have the following details:
//            CommunityDetails communityDetails = new CommunityDetails();

//            // 1. This community type should be User
//            communityDetails.CommunityType = CommunityTypes.User;

//            // 2. User/owner will be the new USER.
//            communityDetails.CreatedByID = userID;

//            // 3. This community is not featured.
//            communityDetails.IsFeatured = false;

//            // 4. Name should be NONE.
//            communityDetails.Name = Resources.UserCommunityName;

//            // 5. Access type should be private.
//            communityDetails.AccessTypeID = (int)AccessType.Private;

//            // 6. Set the category ID of general interest. We need to set the Category ID as it is a foreign key and cannot be null.
//            communityDetails.CategoryID = (int)CategoryType.GeneralInterest;

//            ////      TOOD: 7. While implementing roles we need to make sure we give the current user as the owner permissions.
//            ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
//            communityService.CreateCommunity(communityDetails);
//        }

//        /// <summary>
//        /// Rewrites the Thumbnail path and the URL of the Payload for the community.
//        /// </summary>
//        /// <param name="payloadDetails">PayloadDetails object</param>
//        /// <param name="hasChildCommunities"></param>
//        /// <param name="context"></param>
//        private static void RewritePayloadUrls(PayloadDetails payloadDetails, bool hasChildCommunities, string baseUri)
//        {
            

//            if (payloadDetails.CommunityType == CommunityTypes.Community)
//            {
//                payloadDetails.Thumbnail = RewriteThumbnailUrl(payloadDetails.Thumbnail, "defaultcommunitywwtthumbnail", baseUri);
//            }
//            else
//            {
//                payloadDetails.Thumbnail = RewriteThumbnailUrl(payloadDetails.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            }

//            // Update Place Urls
//            foreach (var place in payloadDetails.Links)
//            {
//                place.Thumbnail = RewriteThumbnailUrl(place.Thumbnail, place.FileType.GetContentTypeImage(), baseUri);
//                place.Url = place.FileType == ContentTypes.Link ? 
//                    place.ContentLink : 
//                    string.Format(CultureInfo.InvariantCulture, "{0}/Download/{1}/{2}{3}", baseUri, place.ContentAzureID, place.Url, "?wwtfull=true");
//            }

//            // Update Content Urls
//            foreach (var childCommunity in payloadDetails.Children)
//            {
//                if (childCommunity.IsCollection)
//                {
//                    // Only for collection set the default thumbnail, not for folders.
//                    childCommunity.Thumbnail = RewriteThumbnailUrl(childCommunity.Thumbnail, "defaultwtmlwwtthumbnail", baseUri);
//                    childCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/File/{1}", baseUri.ToString(), childCommunity.Id);
//                }
//                else
//                {
//                    if (hasChildCommunities)
//                    {
//                        childCommunity.Thumbnail = RewriteThumbnailUrl(childCommunity.Thumbnail, "defaultcommunitywwtthumbnail", baseUri);
//                        childCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Community/{1}", baseUri.ToString(), childCommunity.Id);
//                    }
//                    else
//                    {
//                        if (childCommunity.CommunityType == CommunityTypes.Community)
//                        {
//                            childCommunity.Thumbnail = RewriteThumbnailUrl(childCommunity.Thumbnail, "defaultcommunitywwtthumbnail", baseUri);
//                        }
//                        else
//                        {
//                            childCommunity.Thumbnail = RewriteThumbnailUrl(childCommunity.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//                        }

//                        childCommunity.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Folder/{1}", baseUri, childCommunity.Id);
//                    }
//                }
//            }

//            // Get profile details of all the user who have created the tours
//            IEnumerable<ProfileDetails> profileDetails = new List<ProfileDetails>();
//            if (payloadDetails.Tours.Any())
//            {
//                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;

//                // AuthorImageUrl has the created by ID of the user. This needs to be replaced with the URL for users picture
//                var userList = payloadDetails.Tours.Select(item => item.AuthorImageUrl).ToList().ConvertAll<long>(Convert.ToInt64).AsEnumerable();
//                profileDetails = profileService.GetProfiles(userList.Distinct());
//            }

//            // Update Tour Urls including thumbnail, author and author image
//            foreach (var tour in payloadDetails.Tours)
//            {
//                tour.ThumbnailUrl = RewriteThumbnailUrl(tour.ThumbnailUrl, "defaulttourswwtthumbnail", baseUri);
//                tour.TourUrl = string.Format(CultureInfo.InvariantCulture, "{0}/File/{1}", baseUri.ToString(), tour.TourUrl);
//                tour.AuthorURL = string.Format(CultureInfo.InvariantCulture, "{0}/Profile/Index/{1}", baseUri, tour.AuthorURL);

//                // Get profile picture ID using AuthorImageUrl which is CreatedByID value.
//                var firstOrDefault = profileDetails.FirstOrDefault(item => item.ID.ToString(CultureInfo.InvariantCulture) == tour.AuthorImageUrl);
//                if (firstOrDefault != null)
//                {
//                    var pictureID = firstOrDefault.PictureID;
//                    if (pictureID.HasValue)
//                    {
//                        tour.AuthorImageUrl = string.Format(CultureInfo.InvariantCulture, "{0}/File/Thumbnail/{1}", baseUri, pictureID.Value);
//                    }
//                    else 
//                    {
//                        // return default profile image
//                        tour.AuthorImageUrl = RewriteThumbnailUrl(string.Empty, "wwtprofile", baseUri);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Rewrites the thumbnail URL for the payload XML. In case if the thumbnail is provided, proper URL to access the thumbnail 
//        /// using the File controller and thumbnail action will be formed and returned.
//        /// In case the thumbnail is not provided and default image is provided, then default thumbnail URL will be formed and returned.
//        /// In other cases (both thumbnail and default image is not provided), same thumbnail will be returned.
//        /// </summary>
//        /// <param name="thumbnail">Thumbnail string to be verified</param>
//        /// <param name="defaultImage">Default image name</param>
//        /// <param name="context"></param>
//        /// <returns>Rewritten thumbnail URL</returns>
//        private static string RewriteThumbnailUrl(string thumbnail, string defaultImage, string baseUri)
//        {
//            if (!string.IsNullOrWhiteSpace(thumbnail))
//            {
//                thumbnail = string.Format(CultureInfo.InvariantCulture, "{0}/Thumbnail/{1}", baseUri, thumbnail);
//            }
//            else if (!string.IsNullOrWhiteSpace(defaultImage))
//            {
//                thumbnail = string.Format(CultureInfo.InvariantCulture, "{0}/Content/Images/{1}.png", baseUri, defaultImage);
//            }

//            return thumbnail;
//        }

//        /// <summary>
//        /// Adds all tours and latest tours to the Payload for the community.
//        /// </summary>
//        /// <param name="payloadDetails">PayloadDetails object</param>
//        private static void AddTourFolders(PayloadDetails payloadDetails, string baseUri)
//        {
//            var allTours = PayloadDetailsExtensions.InitializePayload();
//            allTours.Name = "All Tours";
//            allTours.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Community/{1}/Tours", baseUri, payloadDetails.Id);

//            var latestTours = PayloadDetailsExtensions.InitializePayload();
//            latestTours.Name = "Latest";
//            latestTours.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Community/{1}/Latest", baseUri, payloadDetails.Id);

//            // Insert tour folders at the start
//            payloadDetails.Children.Insert(0, latestTours);
//            payloadDetails.Children.Insert(0, allTours);
//        }

//        /// <summary>
//        /// Get File stream for all the tours in a folder
//        /// </summary>
//        /// <param name="payloadDetails">payload Details object</param>
//        /// <returns>Result Stream.</returns>
//        private static Stream GetOutputStream(PayloadDetails payloadDetails)
//        {
//            //WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
//            var stream = payloadDetails.GetXmlStream();
//            stream.Seek(0, SeekOrigin.Begin);
//            return stream;
//        }

//        /// <summary>
//        /// Gets the file stream based on the blobDetails
//        /// </summary>
//        /// <param name="blobDetails">Details of the blob</param>
//        /// <returns>File Stream Result </returns>
//        private static Stream GetFileStream(BlobDetails blobDetails)
//        {
//            // Update the response header.
//            //context.OutgoingResponse.ContentType = blobDetails.MimeType;

//            // Set the position to Begin.
//            blobDetails.Data.Seek(0, SeekOrigin.Begin);
//            return blobDetails.Data;
//        }

//        /// <summary>
//        /// This function is used to get the default payload XML for the user.
//        /// </summary>
//        /// <returns>Payload xml</returns>
//        private static Stream GetMyPayload(string baseUri)
//        {
            
//            // If we are not doing Re-writes of Url, we need to set the thumbnails explicitly at all places
//            PayloadDetails payloadDetails = PayloadDetailsExtensions.InitializePayload();
//            payloadDetails.Thumbnail = RewriteThumbnailUrl(payloadDetails.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Name = Resources.MyFolderLabel;

//            // Get My communities node.
//            var myCommunities = PayloadDetailsExtensions.InitializePayload();
//            myCommunities.Name = Resources.MyCommunitiesWWTLabel;
//            myCommunities.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Communities", baseUri);
//            myCommunities.Thumbnail = RewriteThumbnailUrl(myCommunities.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(myCommunities);

//            // Get My contents node.
//            var myContents = PayloadDetailsExtensions.InitializePayload();
//            myContents.Name = Resources.MyContentsWWTLabel;
//            myContents.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Contents", baseUri);
//            myContents.Thumbnail = RewriteThumbnailUrl(myContents.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(myContents);

//            // Get Browse EO node.
//            var browseDetails = PayloadDetailsExtensions.InitializePayload();
//            browseDetails.Name = Resources.BrowseWWTLabel;
//            browseDetails.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Browse", baseUri);
//            browseDetails.Thumbnail = RewriteThumbnailUrl(browseDetails.Thumbnail, "defaultfolderwwtthumbnail", baseUri);
//            payloadDetails.Children.Add(browseDetails);

//            // TODO: Get Search EO node.
//            var searchEOLink = new Place();
//            searchEOLink.Name = Resources.SearchWWTLabel;
//            searchEOLink.Url = string.Format(CultureInfo.InvariantCulture, "{0}{1}", baseUri, "?wwtfull=true");
//            searchEOLink.Permission = Permission.Reader;
//            searchEOLink.Thumbnail = RewriteThumbnailUrl(searchEOLink.Thumbnail, "searchwwtthumbnail", baseUri);

//            // This will make sure that the additional context menu options specific to folders are not shown in WWT.
//            searchEOLink.MSRComponentId = -1;

//            payloadDetails.Links.Add(searchEOLink);

//            // TODO: Get Help node.
//            var helpLink = new Place();
//            helpLink.Name = Resources.HelpWWTLabel;
//            helpLink.Url = string.Format(CultureInfo.InvariantCulture, "{0}/Home/FAQs{1}", baseUri, "?wwtfull=true");
//            helpLink.Permission = Permission.Reader;
//            helpLink.Thumbnail = RewriteThumbnailUrl(helpLink.Thumbnail, "helpwwtthumbnail", baseUri);

//            // This will make sure that the additional context menu options specific to folders are not shown in WWT.
//            helpLink.MSRComponentId = -1;

//            payloadDetails.Links.Add(helpLink);

//            return GetOutputStream(payloadDetails);
//        }

//        private static long CreateContent(string filename, Stream fileContent, ProfileDetails profileDetails, CommunityDetails parentCommunity, string baseUri)
//        {
//            ContentDetails content = null;

//            // No need to get into a memory stream as the stream is sent as is and need not be loaded
//            using (Stream writablefileContent = new MemoryStream())
//            {
//                fileContent.CopyTo(writablefileContent);
//                writablefileContent.Position = 0;
//                content = GetContentDetail(filename, writablefileContent, profileDetails, parentCommunity, baseUri);
//                content.Name = Path.GetFileNameWithoutExtension(filename);
//            }

//            IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
//            long contentID = content.ID = contentService.CreateContent(content);

//            if (contentID > 0)
//            {
//                INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
//                notificationService.NotifyNewEntityRequest(content, baseUri + "/");
//            }

//            return contentID;
//        }

//        private static long CreateTour(string filename, Stream fileContent, ProfileDetails profileDetails, CommunityDetails parentCommunity, string baseUri)
//        {
//            XmlDocument tourDoc = new XmlDocument();
//            ContentDetails content = null;

//            // NetworkStream is not Seek able. Need to load into memory stream so that it can be sought
//            using (Stream writablefileContent = new MemoryStream())
//            {
//                fileContent.CopyTo(writablefileContent);
//                writablefileContent.Position = 0;
//                content = GetContentDetail(filename, writablefileContent, profileDetails, parentCommunity, baseUri);
//                writablefileContent.Position = 0;
//                tourDoc = tourDoc.SetXmlFromTour(writablefileContent);
//            }

//            if (tourDoc != null)
//            {
//                // Note that the spelling of Description is wrong because that's how WWT generates the WTT file.
//                content.Name = tourDoc.GetAttributeValue("Tour", "Title");
//                content.Description = tourDoc.GetAttributeValue("Tour", "Descirption");
//                content.DistributedBy = tourDoc.GetAttributeValue("Tour", "Author");
//                content.TourLength = tourDoc.GetAttributeValue("Tour", "RunTime");
//            }

//            IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
//            long contentID = content.ID = contentService.CreateContent(content);

//            if (contentID > 0)
//            {
//                INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
//                notificationService.NotifyNewEntityRequest(content, baseUri + "/");
//            }

//            return contentID;
//        }

//        private static long CreateCollection(string filename, Stream fileContent, ProfileDetails profileDetails, CommunityDetails parentCommunity, string baseUri)
//        {
//            // Get name/ tags/ category from Tour.
//            XmlDocument collectionDoc = new XmlDocument();
//            ContentDetails content = null;

//            // NetworkStream is not Seek able. Need to load into memory stream so that it can be sought
//            using (Stream writablefileContent = new MemoryStream())
//            {
//                fileContent.CopyTo(writablefileContent);
//                writablefileContent.Position = 0;
//                content = GetContentDetail(filename, writablefileContent, profileDetails, parentCommunity, baseUri);
//                writablefileContent.Seek(0, SeekOrigin.Begin);
//                collectionDoc = collectionDoc.SetXmlFromWtml(writablefileContent);
//            }

//            if (collectionDoc != null)
//            {
//                content.Name = collectionDoc.GetAttributeValue("Folder", "Name");
//            }

//            IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
//            long contentID = content.ID = contentService.CreateContent(content);

//            if (contentID > 0)
//            {
//                INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
//                notificationService.NotifyNewEntityRequest(content, baseUri + "/");
//            }

//            return contentID;
//        }

//        private static ContentDetails GetContentDetail(string filename, Stream fileContent, ProfileDetails profileDetails, CommunityDetails parentCommunity, string baseUri)
//        {
//            ContentDetails content = new ContentDetails();

//            var fileDetail = UpdateFileDetails(filename, fileContent, baseUri);

//            content.ContentData = fileDetail;
//            content.CreatedByID = profileDetails.ID;

//            // By Default the access type will be private.
//            content.AccessTypeID = (int)AccessType.Public;

//            // Get the Category/Tags from Parent.
//            content.ParentID = parentCommunity.ID;
//            content.ParentType = parentCommunity.CommunityType;
//            content.CategoryID = parentCommunity.CategoryID;

//            return content;
//        }

//        private static FileDetail UpdateFileDetails(string filename, Stream fileContent, string baseUri)
//        {
//            IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;

//            var fileDetail = new FileDetail();
//            fileDetail.SetValuesFrom(filename, fileContent);

//            // TODO: Update mime type and size from the incoming request.
//            //fileDetail.MimeType = Path.GetExtension(filename).GetWwtMimeType(context.IncomingRequest.ContentType);
//            //fileDetail.Size = context.IncomingRequest.ContentLength;

//            contentService.UploadTemporaryFile(fileDetail);

//            return fileDetail;
//        }


//        private static Stream GetTopContent(EntityHighlightFilter entityHighlightFilter, string baseUri)
//        {
//            IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
//            PageDetails pageDetails = new PageDetails(1);
//            pageDetails.ItemsPerPage = 20;
//            var contents = entityService.GetContents(entityHighlightFilter, pageDetails);

//            var payloadDetails = PayloadDetailsExtensions.InitializePayload();
//            payloadDetails.SetValuesFrom(contents);

//            RewritePayloadUrls(payloadDetails, false, baseUri);
//            return GetOutputStream(payloadDetails);
//        }

//        private static Stream GetTopCommunity(EntityHighlightFilter entityHighlightFilter, string baseUri)
//        {
//            IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
//            PageDetails pageDetails = new PageDetails(1);
//            pageDetails.ItemsPerPage = 20;
//            var communities = entityService.GetCommunities(entityHighlightFilter, pageDetails);

//            var payloadDetails = PayloadDetailsExtensions.InitializePayload();
//            payloadDetails.SetValuesFrom(communities);

//            RewritePayloadUrls(payloadDetails, false, baseUri);
//            return GetOutputStream(payloadDetails);
//        }

//        #endregion
//    }
//}
