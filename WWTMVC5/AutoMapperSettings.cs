//-----------------------------------------------------------------------
// <copyright file="AutoMapperSettings.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.ViewModels;
using AutoMapper;

namespace WWTMVC5
{
    /// <summary>
    /// Class having methods for setting the auto-mapper registrations.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "TODO: Error handling")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "This class is used for registering all auto mapper.")]
    public static class AutoMapperSettings
    {
        /// <summary>
        /// Registers the auto-mappers needed for the controllers which will be used for
        /// mapping the domain objects with the View Models.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This method is used for registering all auto mapper.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "This method is used for registering all auto mapper.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This method is used for registering all auto mapper.")]
        public static void RegisterControllerAutoMappers()
        {
            Mapper.CreateMap<CommunityInputViewModel, CommunityDetails>()
                    .ForMember(target => target.Thumbnail, options => options.Ignore());
            Mapper.CreateMap<EntityDetails, EntityViewModel>()
                    .ForMember(target => target.Category, options => options.MapFrom(source => source.CategoryID))
                    .ForMember(target => target.Rating, options => options.MapFrom(source => source.AverageRating));
            Mapper.CreateMap<ContentDetails, EntityViewModel>()
                    .ForMember(target => target.Category, options => options.MapFrom(source => source.CategoryID))
                    .ForMember(target => target.Rating, options => options.MapFrom(source => source.AverageRating));
            Mapper.CreateMap<ContentDetails, ContentViewModel>()
                    .ForMember(target => target.Category, options => options.MapFrom(source => source.CategoryID))
                    .ForMember(target => target.ContentType, options => options.MapFrom(source => source.TypeID))
                    .ForMember(target => target.IsLink, options => options.MapFrom(source => source.TypeID == (int)ContentTypes.Link))
                    .ForMember(target => target.Rating, options => options.MapFrom(source => source.AverageRating));
            Mapper.CreateMap<FileDetail, AssociatedFileViewModel>()
                    .ForMember(target => target.ID, options => options.MapFrom(source => source.AzureID));
            Mapper.CreateMap<LinkDetail, AssociatedFileViewModel>();

            // TODO: Need to get years and months from days.
            Mapper.CreateMap<CommentDetails, CommentItemViewModel>()
                    .ForMember(
                    target => target.PostedDate,
                    options => options.MapFrom(source => source.CommentedDatetime.GetFormattedDifference(DateTime.UtcNow)));

            // TODO: Need to update the Used, Total and PercentageUsedStorage Storage.
            Mapper.CreateMap<ProfileDetails, ProfileViewModel>()
                .ForMember(target => target.ProfileId, options => options.MapFrom(source => source.ID))
                .ForMember(target => target.UsedStorage, options => options.MapFrom(source => ((double)source.ConsumedSize).FormatBytes()))
                .ForMember(target => target.AvailableStorage, options => options.MapFrom(source => ((double)(source.TotalSize - source.ConsumedSize)).FormatBytes()))
                .ForMember(target => target.TotalStorage, options => options.MapFrom(source => ((double)source.TotalSize).FormatBytes()))
                .ForMember(target => target.PercentageUsedStorage, options => options.MapFrom(source => string.Format(CultureInfo.InvariantCulture, "{0:0}%", ((source.ConsumedSize / source.TotalSize)) * 100)))
                .ForMember(target => target.AboutProfile, options => options.MapFrom(source => source.AboutMe.DecodeAndReplace()))
                .ForMember(target => target.ProfilePhotoLink, options => options.MapFrom(source => source.PictureID));

            Mapper.CreateMap<CommunityDetails, SignUpDetails>()
                    .ForMember(target => target.Group, options => options.UseValue("Community"))
                    .ForMember(target => target.Searchable, options => options.UseValue("True"))
                    .ForMember(target => target.CommunityType, options => options.UseValue("Earth"))
                    .ForMember(
                             target => target.Thumbnail,
                             options => options.MapFrom(source => source.Thumbnail.AzureID != Guid.Empty ? source.Thumbnail.AzureID.ToString() : string.Empty));

            Mapper.CreateMap<CommunityDetails, CommunityViewModel>()
                    .ForMember(target => target.Category, options => options.MapFrom(source => source.CategoryID))
                    .ForMember(target => target.AccessType, options => options.MapFrom(source => source.AccessTypeID))
                    .ForMember(target => target.ProducerId, options => options.MapFrom(source => source.CreatedByID))
                    .ForMember(target => target.Producer, options => options.MapFrom(source => source.ProducedBy))
                    .ForMember(target => target.Rating, options => options.MapFrom(source => source.AverageRating))
                    .ForMember(target => target.ThumbnailID, options => options.MapFrom(source => source.Thumbnail.AzureID))
                    .ForMember(target => target.Entity, options => options.MapFrom(source => source.CommunityType))
                    .ForMember(
                            target => target.LastUpdated,
                            options => options.MapFrom(source => source.LastUpdatedDatetime.HasValue ? source.LastUpdatedDatetime.Value.GetFormattedDifference(DateTime.UtcNow) : string.Empty));

            Mapper.CreateMap<CommunityDetails, CommunityInputViewModel>()
                    .ForMember(target => target.ThumbnailID, options => options.MapFrom(source => source.Thumbnail.AzureID));

            Mapper.CreateMap<ContentsView, EntityViewModel>()
                   .ForMember(target => target.Id, options => options.MapFrom(source => source.ContentID))
                   .ForMember(target => target.Name, options => options.MapFrom(source => source.Title))
                   .ForMember(target => target.Category, options => options.MapFrom(source => source.CategoryID))
                   .ForMember(target => target.ParentId, options => options.MapFrom(source => source.CommunityID))
                   .ForMember(target => target.ParentName, options => options.MapFrom(source => source.CommunityName))
                   .ForMember(target => target.ParentType, options => options.MapFrom(source => source.CommunityTypeID))
                   .ForMember(target => target.Entity, options => options.UseValue(EntityType.Content))
                   .ForMember(target => target.Tags, options => options.MapFrom(source => string.IsNullOrWhiteSpace(source.Tags) ? string.Empty : source.Tags))
                   .ForMember(target => target.Rating, options => options.MapFrom(source => source.AverageRating))
                   .ForMember(target => target.Producer, options => options.MapFrom(source => source.ProducedBy))
                   .ForMember(target => target.ProducerId, options => options.MapFrom(source => source.CreatedByID))
                   .ForMember(target => target.ContentType, options => options.MapFrom(source => source.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic)));

            Mapper.CreateMap<CommunitiesView, EntityViewModel>()
                  .ForMember(target => target.Id, options => options.MapFrom(source => source.CommunityID))
                  .ForMember(target => target.Name, options => options.MapFrom(source => source.CommunityName))
                  .ForMember(target => target.Category, options => options.MapFrom(source => source.CategoryID))
                  .ForMember(target => target.ParentType, options => options.MapFrom(source => source.CommunityTypeID))
                  .ForMember(target => target.Entity, options => options.MapFrom(source => source.CommunityTypeID == 1 ? EntityType.Community : EntityType.Folder))
                  .ForMember(target => target.Tags, options => options.MapFrom(source => string.IsNullOrWhiteSpace(source.Tags) ? string.Empty : source.Tags))
                  .ForMember(target => target.Rating, options => options.MapFrom(source => source.AverageRating))
                  .ForMember(target => target.Producer, options => options.MapFrom(source => source.ProducedBy))
                  .ForMember(target => target.ProducerId, options => options.MapFrom(source => source.CreatedByID))
                  .ForMember(target => target.ContentType, options => options.UseValue(ContentTypes.None))
                  .ForMember(target => target.FileName, options => options.MapFrom(source => string.Format(CultureInfo.InvariantCulture, Constants.SignUpFileNameFormat, source.CommunityName)));
        }

        /// <summary>
        /// Register all auto-mappers needed for the services.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "This method is used for registering all auto mapper.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This method is used for registering all auto mapper.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This method is used for registering all auto mapper.")]
        public static void RegisterServiceAutoMappers()
        {
            // Used by CommentService
            Mapper.CreateMap<CommentDetails, CommunityComments>()
                .ForMember(target => target.CommunityID, options => options.MapFrom(source => source.ParentID))
                .ForMember(target => target.CommunityCommentsID, options => options.MapFrom(source => source.CommentID));

            Mapper.CreateMap<CommentDetails, ContentComments>()
                .ForMember(target => target.ContentID, options => options.MapFrom(source => source.ParentID))
                .ForMember(target => target.ContentCommentsID, options => options.MapFrom(source => source.CommentID));

            Mapper.CreateMap<CommunityComments, CommentDetails>()
                .ForMember(target => target.ParentID, options => options.MapFrom(source => source.CommunityID))
                .ForMember(target => target.CommentedBy, options => options.MapFrom(source => source.User.FirstName + " " + source.User.LastName))
                .ForMember(target => target.CommentedByPictureID, options => options.MapFrom(source => source.User.PictureID))
                .ForMember(target => target.CommentID, options => options.MapFrom(source => source.CommunityCommentsID));

            Mapper.CreateMap<ContentComments, CommentDetails>()
                .ForMember(target => target.ParentID, options => options.MapFrom(source => source.ContentID))
                .ForMember(target => target.CommentedDatetime, options => options.MapFrom(source => source.CommentDatetime))
                .ForMember(target => target.CommentedBy, options => options.MapFrom(source => source.User.FirstName + " " + source.User.LastName))
                .ForMember(target => target.CommentedByPictureID, options => options.MapFrom(source => source.User.PictureID))
                .ForMember(target => target.CommentID, options => options.MapFrom(source => source.ContentCommentsID));

            // Used by CommunityService
            Mapper.CreateMap<CommunityDetails, Community>()
                .ForMember(target => target.CommunityTypeID, options => options.MapFrom(source => (int)source.CommunityType))
                .ForMember(target => target.CommunityType, options => options.Ignore())
                .ForMember(target => target.ThumbnailID, options => options.Ignore());

            // Used by ContentService
            Mapper.CreateMap<ContentDetails, Content>()
                    .ForMember(target => target.Title, options => options.MapFrom(source => source.Name));

            Mapper.CreateMap<Content, DownloadDetails>();

            Mapper.CreateMap<StaticContent, StaticContentDetails>();

            // Used by EntityService
            Mapper.CreateMap<TopCategoryEntities, ContentDetails>()
                    .ForMember(target => target.Name, options => options.MapFrom(source => source.Title))
                    .ForMember(target => target.ID, options => options.MapFrom(source => source.ID))
                    .ForMember(target => target.ParentID, options => options.MapFrom(source => source.CommunityID))
                    .ForMember(target => target.ParentName, options => options.MapFrom(source => source.CommunityName))
                    .ForMember(target => target.Tags, options => options.Condition(source => !source.IsSourceValueNull))
                    .ForMember(target => target.ParentType, options => options.MapFrom(source => source.CommunityTypeID.ToEnum<int?, CommunityTypes>(CommunityTypes.None)));

            Mapper.CreateMap<TopCategoryEntities, EntityDetails>()
                    .ForMember(target => target.Tags, options => options.Condition(source => !source.IsSourceValueNull))
                    .ForMember(target => target.ID, options => options.MapFrom(source => source.ID))
                    .ForMember(target => target.Name, options => options.MapFrom(source => source.Title));

            Mapper.CreateMap<Community, CommunityDetails>()
                .ForMember(target => target.ID, options => options.MapFrom(source => (int)source.CommunityID))
                .ForMember(target => target.CategoryID, options => options.MapFrom(source => (int)source.Category.CategoryID))
                .ForMember(target => target.AccessTypeID, options => options.MapFrom(source => (int)source.AccessType.AccessTypeID))
                .ForMember(target => target.AccessTypeName, options => options.MapFrom(source => source.AccessType.Name))
                .ForMember(target => target.CommunityType, options => options.MapFrom(source => source.CommunityTypeID))
                .ForMember(target => target.LastUpdatedDatetime, options => options.MapFrom(source => source.ModifiedDatetime));

            // Used by ProfileService
            Mapper.CreateMap<ProfileDetails, User>()
                .ForMember(target => target.LiveID, options => options.MapFrom(source => source.PUID))
                .ForMember(target => target.UserID, options => options.MapFrom(source => source.ID))
                .ForMember(target => target.Email, options => options.MapFrom(source => source.Email.FixEmailAddress()))
                .ForMember(target => target.UserTypeID, options => options.MapFrom(source => (int?)source.UserType))
                .ForMember(target => target.UserType, options => options.Ignore());

            Mapper.CreateMap<User, ProfileDetails>()
                .ForMember(target => target.PUID, options => options.MapFrom(source => source.LiveID))
                .ForMember(target => target.ID, options => options.MapFrom(source => source.UserID))
                .ForMember(target => target.Email, options => options.MapFrom(source => source.Email.FixEmailAddress()))
                .ForMember(target => target.UserType, options => options.MapFrom(source => source.UserTypeID.HasValue ? source.UserTypeID.Value.ToEnum<int, UserTypes>(UserTypes.Regular) : UserTypes.Regular))
                .ForMember(target => target.TotalSize, options => options.MapFrom(source => source.UserType.MaxAllowedSize));

            Mapper.CreateMap<User, AdminReportProfileDetails>()
                .ForMember(target => target.PUID, options => options.MapFrom(source => source.LiveID))
                .ForMember(target => target.LastLoggedOn, options => options.MapFrom(source => source.LastLoginDatetime))
                .ForMember(target => target.UserName, options => options.MapFrom(source => source.GetFullName()))
                .ForMember(target => target.Email, options => options.MapFrom(source => source.Email.FixEmailAddress()));

            Mapper.CreateMap<UserCommunities, PermissionItem>()
                .ForMember(target => target.Name, options => options.MapFrom(source => source.User.FirstName + " " + source.User.LastName))
                .ForMember(target => target.Role, options => options.MapFrom(source => (UserRole)source.RoleID))
                .ForMember(target => target.Date, options => options.MapFrom(source => source.CreatedDatetime));

            Mapper.CreateMap<PermissionRequest, PermissionItem>()
                .ForMember(target => target.Name, options => options.MapFrom(source => source.User.FirstName + " " + source.User.LastName))
                .ForMember(target => target.CommunityName, options => options.MapFrom(source => source.Community.Name))
                .ForMember(target => target.Role, options => options.MapFrom(source => (UserRole)source.RoleID))
                .ForMember(target => target.Date, options => options.MapFrom(source => source.RequestedDate));

            Mapper.CreateMap<PermissionItem, PermissionRequest>()
                .ForMember(target => target.RoleID, options => options.MapFrom(source => (UserRole)source.Role))
                .ForMember(target => target.Role, options => options.Ignore());

            // Used by RatingService
            Mapper.CreateMap<RatingDetails, CommunityRatings>()
                .ForMember(target => target.CommunityID, options => options.MapFrom(source => source.ParentID));

            Mapper.CreateMap<RatingDetails, ContentRatings>()
                .ForMember(target => target.RatingByID, options => options.MapFrom(source => source.RatedByID))
                .ForMember(target => target.ContentID, options => options.MapFrom(source => source.ParentID));

            // Used by ReportEntityService
            Mapper.CreateMap<ReportEntityDetails, OffensiveCommunities>()
                .ForMember(target => target.CommunityID, options => options.MapFrom(source => source.ParentID))
                .ForMember(target => target.OffensiveTypeID, options => options.MapFrom(source => (int)source.ReportEntityType))
                .ForMember(target => target.OffensiveStatusID, options => options.MapFrom(source => (int)source.Status))
                .ForMember(target => target.Comments, options => options.MapFrom(source => source.Comment));

            Mapper.CreateMap<ReportEntityDetails, OffensiveContent>()
                .ForMember(target => target.ContentID, options => options.MapFrom(source => source.ParentID))
                .ForMember(target => target.OffensiveTypeID, options => options.MapFrom(source => (int)source.ReportEntityType))
                .ForMember(target => target.OffensiveStatusID, options => options.MapFrom(source => (int)source.Status))
                .ForMember(target => target.Comments, options => options.MapFrom(source => source.Comment));

            Mapper.CreateMap<ContentsView, ContentDetails>()
                .ForMember(target => target.ID, options => options.MapFrom(source => source.ContentID))
                .ForMember(target => target.Name, options => options.MapFrom(source => source.Title))
                .ForMember(target => target.ParentType, options => options.MapFrom(source => source.CommunityTypeID))
                .ForMember(target => target.ParentID, options => options.MapFrom(source => source.CommunityID))
                .ForMember(target => target.ParentName, options => options.MapFrom(source => source.CommunityName))
                .ForMember(target => target.Tags, options => options.MapFrom(source => (source.Tags ?? string.Empty)))
                .ForMember(target => target.SortOrder, options => options.Ignore())
                .ForMember(target => target.TourLength, options => options.MapFrom(source => source.TourRunLength))
                .ForMember(
                            target => target.Thumbnail,
                            options => options.MapFrom(source => new FileDetail()
                            {
                                AzureID = source.ThumbnailID ?? Guid.Empty
                            }))
                .ForMember(
                            target => target.ContentData,
                            options => options.ResolveUsing(source => 
                                    {
                                        ContentTypes type = source.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic);
                                        DataDetail dataDetail;

                                        if (type == ContentTypes.Link)
                                        {
                                            dataDetail = new LinkDetail(source.ContentUrl, source.ContentID);
                                        }
                                        else
                                        {
                                            var fileDetail = new FileDetail();
                                            fileDetail.ContentID = source.ContentID;
                                            fileDetail.AzureID = source.ContentAzureID;
                                            dataDetail = fileDetail;
                                        }

                                        dataDetail.Name = source.Filename;
                                        dataDetail.ContentType = type;
                                        return dataDetail;
                                    }));

            Mapper.CreateMap<AllContentsView, ContentDetails>()
               .ForMember(target => target.ID, options => options.MapFrom(source => source.ContentID))
               .ForMember(target => target.Name, options => options.MapFrom(source => source.Title))
               .ForMember(target => target.AccessTypeName, options => options.MapFrom(source => source.AccessType))
               .ForMember(target => target.ParentType, options => options.MapFrom(source => source.CommunityTypeID))
               .ForMember(target => target.ParentID, options => options.MapFrom(source => source.CommunityID))
               .ForMember(target => target.ParentName, options => options.MapFrom(source => source.CommunityName))
               .ForMember(target => target.Tags, options => options.MapFrom(source => (source.Tags ?? string.Empty)))
               .ForMember(target => target.SortOrder, options => options.Ignore())
               .ForMember(
                           target => target.Thumbnail,
                           options => options.ResolveUsing(source =>
                           {
                               var thumbnailDetail = new FileDetail();
                               thumbnailDetail.AzureID = source.ThumbnailID.HasValue ? source.ThumbnailID.Value : Guid.Empty;
                               return thumbnailDetail;
                           }))
               .ForMember(
                           target => target.ContentData,
                           options => options.ResolveUsing(source =>
                           {
                               ContentTypes type = source.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic);
                               DataDetail dataDetail;

                               if (type == ContentTypes.Link)
                               {
                                   dataDetail = new LinkDetail(source.ContentUrl, source.ContentID);
                               }
                               else
                               {
                                   var fileDetail = new FileDetail();
                                   fileDetail.ContentID = source.ContentID;
                                   fileDetail.AzureID = source.ContentAzureID;
                                   dataDetail = fileDetail;
                               }

                               dataDetail.Name = source.Filename;
                               dataDetail.ContentType = type;
                               return dataDetail;
                           }));

            Mapper.CreateMap<FeaturedContentsView, ContentDetails>()
                .ForMember(target => target.ID, options => options.MapFrom(source => source.ContentID))
                .ForMember(target => target.Name, options => options.MapFrom(source => source.Title))
                .ForMember(target => target.ParentType, options => options.MapFrom(source => source.CommunityTypeID))
                .ForMember(target => target.ParentID, options => options.MapFrom(source => source.CommunityID))
                .ForMember(target => target.ParentName, options => options.MapFrom(source => source.CommunityName))
                .ForMember(target => target.Tags, options => options.MapFrom(source => (source.Tags == null ? string.Empty : source.Tags)))
                .ForMember(
                            target => target.Thumbnail,
                            options => options.ResolveUsing(source =>
                            {
                                var thumbnailDetail = new FileDetail();
                                thumbnailDetail.AzureID = source.ThumbnailID.HasValue ? source.ThumbnailID.Value : Guid.Empty;
                                return thumbnailDetail;
                            }))
                .ForMember(
                            target => target.ContentData,
                            options => options.ResolveUsing(source =>
                            {
                                ContentTypes type = source.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic);
                                DataDetail dataDetail;

                                if (type == ContentTypes.Link)
                                {
                                    dataDetail = new LinkDetail(source.ContentUrl, source.ContentID);
                                }
                                else
                                {
                                    var fileDetail = new FileDetail();
                                    fileDetail.ContentID = source.ContentID;
                                    fileDetail.AzureID = source.ContentAzureID;
                                    dataDetail = fileDetail;
                                }

                                dataDetail.Name = source.Filename;
                                dataDetail.ContentType = type;
                                return dataDetail;
                            }));

            Mapper.CreateMap<CommunitiesView, CommunityDetails>()
                .ForMember(target => target.ID, options => options.MapFrom(source => source.CommunityID))
                .ForMember(target => target.Name, options => options.MapFrom(source => source.CommunityName))
                .ForMember(target => target.AccessTypeName, options => options.MapFrom(source => source.AccessType))
                .ForMember(target => target.AccessTypeID, options => options.MapFrom(source => (int)source.AccessType.ToEnum<string, AccessType>(AccessType.Private)))
                .ForMember(target => target.CommunityType, options => options.MapFrom(source => source.CommunityTypeID.ToEnum<int, CommunityTypes>(CommunityTypes.None)))
                .ForMember(target => target.SortOrder, options => options.Ignore())
                .ForMember(
                            target => target.Thumbnail,
                            options => options.ResolveUsing(source =>
                                    {
                                        var thumbnailDetail = new FileDetail();
                                        thumbnailDetail.AzureID = source.ThumbnailID.HasValue ? source.ThumbnailID.Value : Guid.Empty;
                                        return thumbnailDetail;
                                    }));

            Mapper.CreateMap<AllCommunitiesView, CommunityDetails>()
                .ForMember(target => target.ID, options => options.MapFrom(source => source.CommunityID))
                .ForMember(target => target.Name, options => options.MapFrom(source => source.CommunityName))
                .ForMember(target => target.AccessTypeName, options => options.MapFrom(source => source.AccessType))
                .ForMember(target => target.AccessTypeID, options => options.MapFrom(source => (int)source.AccessType.ToEnum<string, AccessType>(AccessType.Private)))
                .ForMember(target => target.CommunityType, options => options.MapFrom(source => source.CommunityTypeID.ToEnum<int, CommunityTypes>(CommunityTypes.None)))
                .ForMember(target => target.SortOrder, options => options.Ignore())
                .ForMember(
                            target => target.Thumbnail,
                            options => options.ResolveUsing(source =>
                            {
                                var thumbnailDetail = new FileDetail();
                                thumbnailDetail.AzureID = source.ThumbnailID.HasValue ? source.ThumbnailID.Value : Guid.Empty;
                                return thumbnailDetail;
                            }));

            Mapper.CreateMap<FeaturedCommunitiesView, CommunityDetails>()
                .ForMember(target => target.ID, options => options.MapFrom(source => source.CommunityID))
                .ForMember(target => target.Name, options => options.MapFrom(source => source.CommunityName))
                .ForMember(target => target.AccessTypeName, options => options.MapFrom(source => source.AccessType))
                .ForMember(target => target.AccessTypeID, options => options.MapFrom(source => (int)source.AccessType.ToEnum<string, AccessType>(AccessType.Private)))
                .ForMember(target => target.CommunityType, options => options.MapFrom(source => source.CommunityTypeID.ToEnum<int, CommunityTypes>(CommunityTypes.None)))
                .ForMember(
                            target => target.Thumbnail,
                            options => options.ResolveUsing(source =>
                            {
                                var thumbnailDetail = new FileDetail();
                                thumbnailDetail.AzureID = source.ThumbnailID.HasValue ? source.ThumbnailID.Value : Guid.Empty;
                                return thumbnailDetail;
                            }));

            // Used by AdministrationService
            Mapper.CreateMap<OffensiveCommunities, OffensiveEntityDetails>()
                .ForMember(target => target.EntityID, options => options.MapFrom(source => source.CommunityID))
                .ForMember(target => target.EntityName, options => options.MapFrom(source => source.Community.Name))
                .ForMember(target => target.EntryID, options => options.MapFrom(source => source.OffensiveCommunitiesID))
                .ForMember(target => target.ReportedByID, options => options.MapFrom(source => source.ReportedByID))
                .ForMember(target => target.ReportedBy, options => options.MapFrom(source => source.User.FirstName + " " + source.User.LastName))
                .ForMember(target => target.ReportedDatetime, options => options.MapFrom(source => source.ReportedDatetime))
                .ForMember(target => target.Reason, options => options.MapFrom(source => Resources.ResourceManager.GetString(source.OffensiveTypeID.ToEnum<int, ReportEntityType>(ReportEntityType.None).ToString())))
                .ForMember(target => target.Comment, options => options.MapFrom(source => source.Comments));

            Mapper.CreateMap<OffensiveContent, OffensiveEntityDetails>()
                .ForMember(target => target.EntityID, options => options.MapFrom(source => source.ContentID))
                .ForMember(target => target.EntityName, options => options.MapFrom(source => source.Content.Title))
                .ForMember(target => target.EntryID, options => options.MapFrom(source => source.OffensiveContentID))
                .ForMember(target => target.ReportedByID, options => options.MapFrom(source => source.ReportedByID))
                .ForMember(target => target.ReportedBy, options => options.MapFrom(source => source.User.FirstName + " " + source.User.LastName))
                .ForMember(target => target.ReportedDatetime, options => options.MapFrom(source => source.ReportedDatetime))
                .ForMember(target => target.Reason, options => options.MapFrom(source => Resources.ResourceManager.GetString(source.OffensiveTypeID.ToEnum<int, ReportEntityType>(ReportEntityType.None).ToString())))
                .ForMember(target => target.Comment, options => options.MapFrom(source => source.Comments));

            Mapper.CreateMap<InviteRequestItem, InviteRequestContent>();
            Mapper.CreateMap<InviteRequestItem, InviteRequestItem>()
                .ForMember(target => target.EmailIdList, options => options.Ignore());
            Mapper.CreateMap<InviteRequestsView, InviteRequestItem>()
                .ForMember(target => target.EmailIdList, options => options.MapFrom(source => new Collection<string>() { source.EmailID }));

            Mapper.CreateMap<CommunityDetails, AdminEntityDetails>()
                .ForMember(target => target.EntityID, options => options.MapFrom(source => source.ID))
                .ForMember(target => target.EntityName, options => options.MapFrom(source => source.Name))
                .ForMember(target => target.Visibility, options => options.MapFrom(source => source.AccessTypeName))
                .ForMember(target => target.ModifiedDatetime, options => options.MapFrom(source => source.LastUpdatedDatetime))
                .ForMember(target => target.EntityType, options => options.UseValue(EntityType.Community))
                .ForMember(target => target.Category, options => options.MapFrom(source => source.CategoryID.ToEnum<int, CategoryType>(CategoryType.All)))
                .ForMember(target => target.CreatedBy, options => options.MapFrom(source => source.ProducedBy))
                .ForMember(target => target.CreatedByID, options => options.MapFrom(source => source.CreatedByID))
                .ForMember(target => target.DistributedBy, options => options.MapFrom(source => HttpContext.Current.Server.HtmlDecode(source.DistributedBy).GetTextFromHtmlString()));
            
            Mapper.CreateMap<ContentDetails, AdminEntityDetails>()
                .ForMember(target => target.EntityID, options => options.MapFrom(source => source.ID))
                .ForMember(target => target.EntityName, options => options.MapFrom(source => source.Name))
                .ForMember(target => target.Visibility, options => options.MapFrom(source => source.AccessTypeName))
                .ForMember(target => target.ModifiedDatetime, options => options.MapFrom(source => source.LastUpdatedDatetime))
                .ForMember(target => target.EntityType, options => options.UseValue(EntityType.Content))
                .ForMember(target => target.Category, options => options.MapFrom(source => source.CategoryID.ToEnum<int, CategoryType>(CategoryType.All)))
                .ForMember(target => target.CreatedBy, options => options.MapFrom(source => source.ProducedBy))
                .ForMember(target => target.CreatedByID, options => options.MapFrom(source => source.CreatedByID))
                .ForMember(target => target.DistributedBy, options => options.MapFrom(source => HttpContext.Current.Server.HtmlDecode(source.DistributedBy).GetTextFromHtmlString()));

            Mapper.CreateMap<ProfileDetails, AdminUserDetails>()
                .ForMember(target => target.UserID, options => options.MapFrom(source => source.ID))
                .ForMember(target => target.UserName, options => options.MapFrom(source => source.FirstName + " " + source.LastName))
                .ForMember(target => target.UserImageID, options => options.MapFrom(source => source.PictureID))
                .ForMember(target => target.Email, options => options.MapFrom(source => source.Email));

            Mapper.CreateMap<SearchView, EntityViewModel>()
                .ForMember(target => target.Tags, options => options.MapFrom(source => string.IsNullOrWhiteSpace(source.Tags) ? string.Empty : source.Tags))
                .ForMember(target => target.ParentType, options => options.Condition(source => !source.IsSourceValueNull));
        }
    }
}
