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
        public OffensiveEntityDetailsList GetOffensiveCommunities()
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
                    entities = new Collection<OffensiveEntityDetails>(entityService.GetOffensiveCommunities(profileDetails.ID).ToList());
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return new OffensiveEntityDetailsList() { Entities = entities };
        }

        public OffensiveEntityDetailsList GetOffensiveContents()
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
                    entities = new Collection<OffensiveEntityDetails>(entityService.GetOffensiveContents(profileDetails.ID).ToList());
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

        public AdminEntityDetailsList GetCommunities(string categoryID)
        {
            AdminEntityDetailsList result = new AdminEntityDetailsList();

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    result.FeaturedEntities = GetCommunities(HighlightType.Featured, categoryID, new List<long>());
                    result.Entities = GetCommunities(HighlightType.None, categoryID, result.FeaturedEntities.Select(c => c.EntityID).ToList());
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }
            return result;
        }

        public AdminEntityDetailsList GetContents(string categoryID)
        {
            AdminEntityDetailsList result = new AdminEntityDetailsList();

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    result.FeaturedEntities = GetContents(HighlightType.Featured, categoryID, new List<long>());
                    result.Entities = GetContents(HighlightType.None, categoryID, result.FeaturedEntities.Select(c => c.EntityID).ToList());
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }
            return result;
        }

        public bool UpdateFeaturedCommunities(string categoryID, Stream featuredCommunities)
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
                            var status = featuredEntityService.UpdateFeaturedCommunities(communities.FeaturedEntities, profileDetails.ID, GetCategoryID(categoryID));
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

        public bool UpdateFeaturedContents(string categoryID, Stream featuredContents)
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
                            var status = featuredEntityService.UpdateFeaturedContents(contents.FeaturedEntities, profileDetails.ID, GetCategoryID(categoryID));
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

        public AdminUserDetailsList GetUsers()
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

                    foreach (var profile in profileService.GetAllProfiles(profileDetails.ID))
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

        public AdminReportProfileDetailsList GetUsersForReport()
        {
            AdminReportProfileDetailsList usersList = new AdminReportProfileDetailsList();

            ProfileDetails profileDetails;

            if (ValidateAuthentication(true, out profileDetails))
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                profileDetails = profileService.GetProfile(profileDetails.PUID);
                if (profileDetails != null)
                {
                    usersList.Users = new Collection<AdminReportProfileDetails>();

                    foreach (var profile in profileService.GetAllProfilesForReport(profileDetails.ID))
                    {
                        usersList.Users.Add(profile);
                    }
                }
                else
                {
                    throw new WebFaultException<string>(Resources.UserNotRegisteredMessage, HttpStatusCode.Unauthorized);
                }
            }

            return usersList;
        }

        public bool UpdateAdminUsers(Stream adminUsers)
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
                            var status = profileService.PromoteAsSiteAdmin(usersList.AdminUsers.Select(u => u.UserID), profileDetails.ID);
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

        public AdminEntityDetailsList GetAllCommunities(string categoryID)
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
                    foreach (var item in entityService.GetAllCommunities(profileDetails.ID, GetCategoryID(categoryID)))
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

        public AdminEntityDetailsList GetAllContents(string categoryID)
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
                    foreach (var item in entityService.GetAllContents(profileDetails.ID, GetCategoryID(categoryID)))
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

        public bool UnDeleteOffensiveCommunity(string id)
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
                    status = communityService.UnDeleteOffensiveCommunity(long.Parse(id, CultureInfo.InvariantCulture), profileDetails.ID).Succeeded;
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

        private static Collection<AdminEntityDetails> GetCommunities(HighlightType highlightType, string categoryID, List<long> doNotInclude)
        {
            Collection<AdminEntityDetails> entities = new Collection<AdminEntityDetails>();
            IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
            var results = entityService.GetCommunities(new EntityHighlightFilter(highlightType, int.Parse(categoryID, CultureInfo.InvariantCulture).ToEnum<int, CategoryType>(CategoryType.All), null));
            foreach (var item in results.Where(fc => !doNotInclude.Contains(fc.ID)))
            {
                AdminEntityDetails entity = new AdminEntityDetails();
                Mapper.Map(item, entity);
                entity.CategoryName = Resources.ResourceManager.GetString(item.CategoryID.ToEnum<int, CategoryType>(CategoryType.All).ToString(), Resources.Culture);

                entities.Add(entity);
            }

            return entities;
        }

        private static Collection<AdminEntityDetails> GetContents(HighlightType highlightType, string categoryID, List<long> doNotInclude)
        {
            Collection<AdminEntityDetails> entities = new Collection<AdminEntityDetails>();
            IEntityService entityService = DependencyResolver.Current.GetService(typeof(IEntityService)) as IEntityService;
            var results = entityService.GetContents(new EntityHighlightFilter(highlightType, int.Parse(categoryID, CultureInfo.InvariantCulture).ToEnum<int, CategoryType>(CategoryType.All), null));

            foreach (var item in results.Where(fc => !doNotInclude.Contains(fc.ID)))
            {
                AdminEntityDetails entity = new AdminEntityDetails();
                Mapper.Map(item, entity);
                entity.CategoryName = Resources.ResourceManager.GetString(item.CategoryID.ToEnum<int, CategoryType>(CategoryType.All).ToString(), Resources.Culture);

                entities.Add(entity);
            }

            return entities;
        }

        private static int? GetCategoryID(string id)
        {
            int? categoryID = null;
            int catID;
            if (int.TryParse(id, out catID))
            {
                if (catID > 0)
                {
                    categoryID = catID;
                }
            }

            return categoryID;
        }
    }
}