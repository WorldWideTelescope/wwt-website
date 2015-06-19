//-----------------------------------------------------------------------
// <copyright file="AdministrationService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.WebServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AdministrationService : ServiceBase, IAdministrationService
    {
        public async Task<OffensiveEntityDetailsList> GetOffensiveCommunities()
        {
            Collection<OffensiveEntityDetails> entities = new Collection<OffensiveEntityDetails>();
            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IReportEntityService entityService = DependencyResolver.Current.GetService(typeof(IReportEntityService)) as IReportEntityService;
                    var communities = await entityService.GetOffensiveCommunities(profileDetails.ID);
                    entities = new Collection<OffensiveEntityDetails>(communities.ToList());
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return new OffensiveEntityDetailsList() { Entities = entities };
        }

        public async Task<OffensiveEntityDetailsList> GetOffensiveContents()
        {
            Collection<OffensiveEntityDetails> entities = new Collection<OffensiveEntityDetails>();

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IReportEntityService entityService = DependencyResolver.Current.GetService(typeof(IReportEntityService)) as IReportEntityService;
                    var contents = await entityService.GetOffensiveContents(profileDetails.ID);
                    entities = new Collection<OffensiveEntityDetails>(contents.ToList());
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return new OffensiveEntityDetailsList() { Entities = entities };
        }

        public bool DeleteOffensiveCommunityEntry(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IReportEntityService entityService = DependencyResolver.Current.GetService(typeof(IReportEntityService)) as IReportEntityService;
                    var details = new OffensiveEntry()
                    {
                        EntryID = long.Parse(id, CultureInfo.InvariantCulture),
                        ReviewerID = profileDetails.ID,
                        Status = OffensiveStatusType.Reviewed
                    };
                    status = entityService.UpdateOffensiveCommunityEntry(details).Succeeded;
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public bool DeleteOffensiveContentEntry(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IReportEntityService entityService = DependencyResolver.Current.GetService(typeof(IReportEntityService)) as IReportEntityService;
                    var details = new OffensiveEntry()
                    {
                        EntryID = long.Parse(id, CultureInfo.InvariantCulture),
                        ReviewerID = profileDetails.ID,
                        Status = OffensiveStatusType.Reviewed
                    };
                    status = entityService.UpdateOffensiveContentEntry(details).Succeeded;
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public bool DeleteOffensiveCommunity(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    var details = new OffensiveEntry()
                    {
                        EntityID = long.Parse(id, CultureInfo.InvariantCulture),
                        ReviewerID = profileDetails.ID,
                        Status = OffensiveStatusType.Offensive
                    };

                    ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
                    status = communityService.DeleteCommunity(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID, true, details).Succeeded;

                    // Notify the Moderators and Owners about the Deleted Community.
                    INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
                    EntityAdminActionRequest notification = new EntityAdminActionRequest()
                    {
                        AdminID = profileDetails.ID,
                        EntityID = long.Parse(id, CultureInfo.InvariantCulture),
                        EntityType = EntityType.Community,
                        EntityLink = string.Format(CultureInfo.InvariantCulture, "{0}/Community/Index/{1}", HttpContext.Current.Request.UrlReferrer.GetApplicationPath(), id),
                        Action = AdminActions.Delete
                    };

                    notificationService.NotifyEntityDeleteRequest(notification);
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public bool DeleteOffensiveContent(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    var details = new OffensiveEntry()
                    {
                        EntityID = long.Parse(id, CultureInfo.InvariantCulture),
                        ReviewerID = profileDetails.ID,
                        Status = OffensiveStatusType.Offensive
                    };

                    IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
                    status = contentService.DeleteContent(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID, true, details).Succeeded;

                    // Notify the Moderators and Owners about the Deleted Content.
                    INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
                    EntityAdminActionRequest notification = new EntityAdminActionRequest()
                    {
                        AdminID = profileDetails.ID,
                        EntityID = long.Parse(id, CultureInfo.InvariantCulture),
                        EntityType = EntityType.Content,
                        EntityLink = string.Format(CultureInfo.InvariantCulture, "{0}/Content/Index/{1}", HttpContext.Current.Request.UrlReferrer.GetApplicationPath(), id),
                        Action = AdminActions.Delete
                    };

                    notificationService.NotifyEntityDeleteRequest(notification);
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public async Task<AdminEntityDetailsList> GetCommunities(string categoryId)
        {
            AdminEntityDetailsList result = new AdminEntityDetailsList();

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    result.FeaturedEntities = await GetCommunities(HighlightType.Featured, categoryId, new List<long>());
                    result.Entities = await GetCommunities(HighlightType.None, categoryId, result.FeaturedEntities.Select(c => c.EntityID).ToList());
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }
            return result;
        }

        public async Task<AdminEntityDetailsList> GetContents(string categoryId)
        {
            AdminEntityDetailsList result = new AdminEntityDetailsList();

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    result.FeaturedEntities = await GetContents(HighlightType.Featured, categoryId, new List<long>());
                    result.Entities = await GetContents(HighlightType.None, categoryId, result.FeaturedEntities.Select(c => c.EntityID).ToList());
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }
            return result;
        }

        public async Task<bool> UpdateFeaturedCommunities(string categoryId, Stream featuredCommunities)
        {
            bool operationStatus = false;
            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    string input;
                    using (StreamReader sr = new StreamReader(featuredCommunities))
                    {
                        input = sr.ReadToEnd();
                    }

                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        AdminEntityDetailsList communities = input.DeserializeXML<AdminEntityDetailsList>();
                        if (communities != null && communities.FeaturedEntities != null)
                        {
                            IFeaturedEntityService featuredEntityService = DependencyResolver.Current.GetService(typeof(IFeaturedEntityService)) as IFeaturedEntityService;
                            var status = await featuredEntityService.UpdateFeaturedCommunities(communities.FeaturedEntities, profileDetails.ID, GetCategoryId(categoryId));
                            operationStatus = status.Succeeded;
                        }
                    }
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return operationStatus;
        }

        public async Task<bool> UpdateFeaturedContents(string categoryId, Stream featuredContents)
        {
            bool operationStatus = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    string input;
                    using (StreamReader sr = new StreamReader(featuredContents))
                    {
                        input = sr.ReadToEnd();
                    }

                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        AdminEntityDetailsList contents = input.DeserializeXML<AdminEntityDetailsList>();
                        if (contents != null && contents.FeaturedEntities != null)
                        {
                            IFeaturedEntityService featuredEntityService = DependencyResolver.Current.GetService(typeof(IFeaturedEntityService)) as IFeaturedEntityService;
                            var status = await featuredEntityService.UpdateFeaturedContents(contents.FeaturedEntities, profileDetails.ID, GetCategoryId(categoryId));
                            operationStatus = status.Succeeded;
                        }
                    }
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return operationStatus;
        }

        public async Task<AdminUserDetailsList> GetUsers()
        {
            AdminUserDetailsList usersList = new AdminUserDetailsList();

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    usersList.AdminUsers = new Collection<AdminUserDetails>();
                    usersList.Users = new Collection<AdminUserDetails>();

                    foreach (var profile in await profileService.GetAllProfiles(profileDetails.ID))
                    {
                        AdminUserDetails user = new AdminUserDetails();
                        Mapper.Map(profile, user);

                        if (profile.UserType == UserTypes.SiteAdmin)
                        {
                            usersList.AdminUsers.Add(user);
                        }
                        else
                        {
                            usersList.Users.Add(user);
                        }
                    }
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return usersList;
        }

        //public AdminReportProfileDetailsList GetUsersForReport()
        //{
        //    AdminReportProfileDetailsList usersList = new AdminReportProfileDetailsList();

        //    ProfileDetails profileDetails;

        //    if (ValidateAuthentication(true, out profileDetails))
        //    {
        //        IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
        //        profileDetails = profileService.GetProfile(profileDetails.PUID);
        //        if (profileDetails != null)
        //        {
        //            usersList.Users = new Collection<AdminReportProfileDetails>();

        //            foreach (var profile in profileService.GetAllProfilesForReport(profileDetails.ID))
        //            {
        //                usersList.Users.Add(profile);
        //            }
        //        }
        //        else
        //        {
        //            throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
        //        }
        //    }

        //    return usersList;
        //}

        public async Task<bool> UpdateAdminUsers(Stream adminUsers)
        {
            bool operationStatus = false;

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    string input;
                    using (StreamReader sr = new StreamReader(adminUsers))
                    {
                        input = sr.ReadToEnd();
                    }

                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        AdminUserDetailsList usersList = input.DeserializeXML<AdminUserDetailsList>();
                        if (usersList != null)
                        {
                            var status = await profileService.PromoteAsSiteAdmin(usersList.AdminUsers.Select(u => u.UserID), profileDetails.ID);
                            operationStatus = status.Succeeded;
                        }
                    }
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return operationStatus;
        }

        public async Task<AdminEntityDetailsList> GetAllCommunities(string categoryId)
        {
            AdminEntityDetailsList adminEntityDetailsList = new AdminEntityDetailsList();
            adminEntityDetailsList.Entities = new Collection<AdminEntityDetails>();

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
                    foreach (var item in await entityService.GetAllCommunities(profileDetails.ID, GetCategoryId(categoryId)))
                    {
                        AdminEntityDetails entity = new AdminEntityDetails();
                        Mapper.Map(item, entity);
                        entity.CategoryName = Resources.ResourceManager.GetString(item.CategoryID.ToEnum<int, CategoryType>(CategoryType.All).ToString(), Resources.Culture);

                        adminEntityDetailsList.Entities.Add(entity);
                    }
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return adminEntityDetailsList;
        }

        public async Task<AdminEntityDetailsList> GetAllContents(string categoryId)
        {
            AdminEntityDetailsList adminEntityDetailsList = new AdminEntityDetailsList();
            adminEntityDetailsList.Entities = new Collection<AdminEntityDetails>();

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
                    foreach (var item in await entityService.GetAllContents(profileDetails.ID, GetCategoryId(categoryId)))
                    {
                        AdminEntityDetails entity = new AdminEntityDetails();
                        Mapper.Map(item, entity);
                        entity.CategoryName = Resources.ResourceManager.GetString(item.CategoryID.ToEnum<int, CategoryType>(CategoryType.All).ToString(), Resources.Culture);

                        adminEntityDetailsList.Entities.Add(entity);
                    }
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return adminEntityDetailsList;
        }

        public async Task<bool> UnDeleteOffensiveCommunity(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = await profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
                    var undeleteResult = await communityService.UnDeleteOffensiveCommunity(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID)
                    status = undeleteResult.Succeeded;
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public bool UnDeleteOffensiveContent(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
                    status = contentService.UnDeleteOffensiveContent(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID).Succeeded;
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public bool MarkAsPrivateCommunity(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
                    status = communityService.SetCommunityAccessType(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID, AccessType.Private).Succeeded;

                    // Notify the Moderators and Owners about the Community which has been marked as private.
                    INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
                    EntityAdminActionRequest notification = new EntityAdminActionRequest()
                    {
                        AdminID = profileDetails.ID,
                        EntityID = long.Parse(id, CultureInfo.InvariantCulture),
                        EntityType = EntityType.Community,
                        EntityLink = string.Format(CultureInfo.InvariantCulture, "{0}/Community/Index/{1}", HttpContext.Current.Request.UrlReferrer.GetApplicationPath(), id),
                        Action = AdminActions.MarkAsPrivate
                    };

                    notificationService.NotifyEntityDeleteRequest(notification);
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public bool MarkAsPrivateContent(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
                    status = contentService.SetContentAccessType(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID, AccessType.Private).Succeeded;

                    // Notify the Moderators and Owners about the Content which has been marked as private.
                    INotificationService notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;
                    EntityAdminActionRequest notification = new EntityAdminActionRequest()
                    {
                        AdminID = profileDetails.ID,
                        EntityID = long.Parse(id, CultureInfo.InvariantCulture),
                        EntityType = EntityType.Content,
                        EntityLink = string.Format(CultureInfo.InvariantCulture, "{0}/Content/Index/{1}", HttpContext.Current.Request.UrlReferrer.GetApplicationPath(), id),
                        Action = AdminActions.MarkAsPrivate
                    };

                    notificationService.NotifyEntityDeleteRequest(notification);
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public bool MarkAsPublicCommunity(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    ICommunityService communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
                    status = communityService.SetCommunityAccessType(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID, AccessType.Public).Succeeded;
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        public bool MarkAsPublicContent(string id)
        {
            bool status = false;

            ProfileDetails profileDetails;
            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    IContentService contentService = DependencyResolver.Current.GetService(typeof(IContentService)) as IContentService;
                    status = contentService.SetContentAccessType(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID, AccessType.Public).Succeeded;
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return status;
        }

        private static async Task<Collection<AdminEntityDetails>> GetCommunities(HighlightType highlightType, string categoryId, List<long> doNotInclude)
        {
            Collection<AdminEntityDetails> entities = new Collection<AdminEntityDetails>();
            IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
            var results = await entityService.GetCommunities(new EntityHighlightFilter(highlightType, int.Parse(categoryId, CultureInfo.InvariantCulture).ToEnum<int, CategoryType>(CategoryType.All), null));
            foreach (var item in results.Where(fc => !doNotInclude.Contains(fc.ID)))
            {
                AdminEntityDetails entity = new AdminEntityDetails();
                Mapper.Map(item, entity);
                entity.CategoryName = Resources.ResourceManager.GetString(item.CategoryID.ToEnum<int, CategoryType>(CategoryType.All).ToString(), Resources.Culture);

                entities.Add(entity);
            }

            return entities;
        }

        private static async Task<Collection<AdminEntityDetails>> GetContents(HighlightType highlightType, string categoryId, List<long> doNotInclude)
        {
            Collection<AdminEntityDetails> entities = new Collection<AdminEntityDetails>();
            IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
            var results = await entityService.GetContents(new EntityHighlightFilter(highlightType, int.Parse(categoryId, CultureInfo.InvariantCulture).ToEnum<int, CategoryType>(CategoryType.All), null));

            foreach (var item in results.Where(fc => !doNotInclude.Contains(fc.ID)))
            {
                AdminEntityDetails entity = new AdminEntityDetails();
                Mapper.Map(item, entity);
                entity.CategoryName = Resources.ResourceManager.GetString(item.CategoryID.ToEnum<int, CategoryType>(CategoryType.All).ToString(), Resources.Culture);

                entities.Add(entity);
            }

            return entities;
        }

        private static int? GetCategoryId(string id)
        {
            int? categoryId = null;
            int catId;
            if (int.TryParse(id, out catId))
            {
                if (catId > 0)
                {
                    categoryId = catId;
                }
            }

            return categoryId;
        }
    }
}