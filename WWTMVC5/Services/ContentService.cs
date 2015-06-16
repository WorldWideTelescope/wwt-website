//-----------------------------------------------------------------------
// <copyright file="ContentService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Content Service having methods for retrieving Content
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class ContentService : PermissionService, IContentService
    {
        //// TODO: Need to move all Private static method to either extensions or Use auto mapper.

        #region Private member variables

        /// <summary>
        /// Instance of Contents repository
        /// </summary>
        private IContentRepository contentRepository;

        /// <summary>
        /// Instance of Blob data repository
        /// </summary>
        private IBlobDataRepository blobDataRepository;

        /// <summary>
        /// Instance of Tag repository
        /// </summary>
        private IRepositoryBase<Tag> tagRepository;

        /// <summary>
        /// Instance of Community repository
        /// </summary>
        private ICommunityRepository communityRepository;

        /// <summary>
        /// Instance of User repository
        /// </summary>
        private IUserRepository userRepository;

        /// <summary>
        /// Instance of OffensiveContent repository
        /// </summary>
        private IRepositoryBase<OffensiveContent> offensiveContentRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ContentService class.
        /// </summary>
        /// <param name="contentRepository">Instance of content repository</param>
        /// <param name="blobDataRepository">Instance of Blob data repository</param>
        /// <param name="tagRepository">Instance of tagRepository</param>
        /// <param name="communityRepository">Instance of community repository</param>
        /// <param name="userRepository">Instance of User repository</param>
        public ContentService(
            IContentRepository contentRepository,
            IBlobDataRepository blobDataRepository,
            IRepositoryBase<Tag> tagRepository,
            ICommunityRepository communityRepository,
            IUserRepository userRepository,
            IRepositoryBase<OffensiveContent> offensiveContentRepository)
            : base(communityRepository, userRepository)
        {
            this.contentRepository = contentRepository;
            this.blobDataRepository = blobDataRepository;
            this.tagRepository = tagRepository;
            this.communityRepository = communityRepository;
            this.userRepository = userRepository;
            this.offensiveContentRepository = offensiveContentRepository;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the contents from the Layerscape database.
        /// </summary>
        /// <param name="contentID">Content for which details to be fetched</param>
        /// <param name="userID">Id of the user who is accessing</param>
        /// <returns>Details about the content</returns>
        public ContentDetails GetContentDetails(long contentID, long userID)
        {
            ContentDetails contentDetails = null;
            var content = this.contentRepository.GetContent(contentID);
            if (content != null)
            {
                Permission userPermission;
                UserRole userRole = GetContentUserRole(content, userID);

                // For private contents, user's who have not assigned explicit permission will not have access.
                if (CanReadContent(userRole))
                {
                    userPermission = userRole.GetPermission();

                    contentDetails = new ContentDetails(userPermission);
                    contentDetails.SetValuesFrom(content);
                }
            }

            return contentDetails;
        }

        public ContentDetails GetContentDetails(Guid azureId)
        {
            ContentDetails contentDetails = null;
            var content = contentRepository.GetContent(azureId);
            if (content != null)
            {
                var userRole = UserRole.SiteAdmin;
                var userPermission = userRole.GetPermission();
                contentDetails = new ContentDetails(userPermission);
                contentDetails.SetValuesFrom(content);
            }

            return contentDetails;
        }


        /// <summary>
        /// Gets the contents from the Layerscape database.
        /// </summary>
        /// <param name="contentID">Content for which details to be fetched</param>
        /// <param name="userID">Id of the user who is accessing</param>
        /// <returns>Details about the content</returns>
        public ContentDetails GetContentDetailsForEdit(long contentID, long userID)
        {
            ContentDetails contentDetails = null;
            var content = this.contentRepository.GetContent(contentID);
            if (content != null)
            {
                Permission userPermission;
                UserRole userRole = GetContentUserRole(content, userID);

                // In case of Edit, user should have role assigned for editing the content.
                if (CanEditDeleteContent(content, userID, userRole))
                {
                    userPermission = userRole.GetPermission();

                    contentDetails = new ContentDetails(userPermission);
                    contentDetails.SetValuesFrom(content);
                }
            }

            return contentDetails;
        }

        /// <summary>
        /// Deletes the specified content from the Earth database.
        /// </summary>
        /// <param name="contentID">Content Id</param>
        /// <param name="profileID">User Identity</param>
        /// <param name="isOffensive">Whether community is offensive or not.</param>
        /// <param name="offensiveDetails">Offensive Details.</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        public OperationStatus DeleteContent(long contentID, long profileID, bool isOffensive, OffensiveEntry offensiveDetails)
        {
            return DeleteContentRecursively(contentID, profileID, isOffensive, offensiveDetails);
        }

        /// <summary>
        /// Deletes the specified content from the Earth database.
        /// </summary>
        /// <param name="contentID">Content Id</param>
        /// <param name="profileID">User Identity</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        public OperationStatus DeleteContent(long contentID, long profileID)
        {
            var details = new OffensiveEntry()
            {
                EntityID = contentID,
                ReviewerID = profileID,
                Status = OffensiveStatusType.Deleted,
                Justification = "Deleted while deleting the Content."
            };
            return DeleteContentRecursively(contentID, profileID, false, details);
        }

        /// <summary>
        /// Un-deletes the specified content from the Earth database so that it is again accessible in the site.
        /// </summary>
        /// <param name="contentID">Content Id</param>
        /// <param name="userID">User Identity</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        public OperationStatus UnDeleteOffensiveContent(long contentID, long userID)
        {
            OperationStatus status = null;
            try
            {
                if (this.userRepository.IsSiteAdmin(userID))
                {
                    // Get the current content from DB.
                    Content content = this.contentRepository.GetItem((c) => c.ContentID == contentID);

                    if (content != null)
                    {
                        content.IsDeleted = false;

                        // We need to mark the community as not offensive as the Admin is marking the content as undeleted.
                        content.IsOffensive = false;

                        // We need to mark the DeletedBy as null as we are Undoing the delete operation. 
                        //  Also DeleteBy filed will be used to check if the user has explicitly deleted the content.
                        content.User2 = null;

                        Community parentCommunity = Enumerable.ElementAt<CommunityContents>(content.CommunityContents, 0).Community;

                        if ((parentCommunity.CommunityTypeID == (int)CommunityTypes.Community || parentCommunity.CommunityTypeID == (int)CommunityTypes.Folder)
                                && parentCommunity.IsDeleted == true)
                        {
                            // Get the "None" (User) community of the user who uploaded the Content.
                            Community noneCommunity = this.communityRepository.GetItem(
                                                                    c => c.CreatedByID == content.CreatedByID && c.CommunityTypeID == (int)CommunityTypes.User);

                            this.CheckNotNull(() => new { noneCommunity });

                            // Remove the existing relation
                            content.CommunityContents.Remove(Enumerable.ElementAt<CommunityContents>(content.CommunityContents, 0));

                            // Set the None community as parent community.
                            CommunityContents noneCommunityContent = new CommunityContents()
                            {
                                CommunityID = noneCommunity.CommunityID,
                                Content = content
                            };

                            content.CommunityContents.Add(noneCommunityContent);
                        }

                        foreach (var contentRelation in content.ContentRelation)
                        {
                            contentRelation.Content1.IsDeleted = false;
                            contentRelation.Content1.User2 = null;
                        }

                        this.contentRepository.Update(content);

                        // Save changes to the database
                        this.contentRepository.SaveChanges();

                        status = OperationStatus.CreateSuccessStatus();
                    }
                    else
                    {
                        status = OperationStatus.CreateFailureStatus(string.Format(CultureInfo.CurrentCulture, "Content with ID '{0}' was not found", contentID));
                    }
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus(Resources.UserNotSiteAdminError);
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            return status;
        }

        /// <summary>
        /// Sets the given access type for the specified Content.
        /// </summary>
        /// <param name="contentID">Content Id</param>
        /// <param name="userID">User Identity</param>
        /// <param name="accessType">Access type of the Content.</param>
        /// <returns>Status of the operation. Success, if succeeded. Failure message and exception details in case of exception.</returns>
        public OperationStatus SetContentAccessType(long contentID, long userID, AccessType accessType)
        {
            OperationStatus status = null;

            try
            {
                if (this.userRepository.IsSiteAdmin(userID))
                {
                    // Get the current content from DB.
                    Content content = this.contentRepository.GetItem((c) => c.ContentID == contentID);

                    // Make sure content exists
                    this.CheckNotNull(() => new { content });

                    content.AccessTypeID = (int)accessType;

                    content.IsOffensive = (accessType == AccessType.Private);

                    var offensiveDetails = new OffensiveEntry()
                    {
                        EntityID = contentID,
                        ReviewerID = userID,
                        Status = OffensiveStatusType.Offensive
                    };

                    UpdateAllOffensiveContentEntry(contentID, offensiveDetails);

                    this.contentRepository.Update(content);
                    this.contentRepository.SaveChanges();

                    // Create Success message if set access type is successful.
                    status = OperationStatus.CreateSuccessStatus();
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            return status;
        }

        /// <summary>
        /// Creates the new content in Layerscape with the given details passed in ContentsView instance.
        /// </summary>
        /// <param name="contentDetails">Details of the content</param>
        /// <returns>Id of the content created. Returns -1 is creation is failed.</returns>
        public long CreateContent(ContentDetails contentDetails)
        {
            // Make sure content is not null
            this.CheckNotNull(() => new { contentDetails });

            long contentID = -1;

            UserRole userRole = GetCommunityUserRole(contentDetails.ParentID, contentDetails.CreatedByID);
            if (!CanCreateContent(userRole))
            {
                // TODO: Throw custom permissions exception which will be shown to user in error page.
                throw new HttpException(401, Resources.NoPermissionCreateContentMessage);
            }

            // Upload content data, thumbnail, video and associated file to azure.
            // Since the associated files are already stored in the Temporary container.
            // We need to move the files to actual container.
            if (UploadFilesToAzure(contentDetails))
            {
                if (MoveAssociatedFiles(contentDetails))
                {
                    try
                    {
                        // Create a new instance of content details
                        Content content = new Content();

                        // Set values from content details.
                        content.SetValuesFrom(contentDetails);

                        // Set the thumbnail ID.
                        content.ThumbnailID = MoveThumbnail(contentDetails.Thumbnail);

                        // Set Created and modified time.
                        content.CreatedDatetime = content.ModifiedDatetime = DateTime.UtcNow;

                        // Set Created and modified IDs.
                        content.ModifiedByID = contentDetails.CreatedByID;
                        content.CreatedByID = contentDetails.CreatedByID;

                        // Set Default Values.
                        content.IsDeleted = false;
                        content.IsSearchable = true;
                        content.IsOffensive = false;
                        content.DownloadCount = 0;

                        Community parentCommunity = this.communityRepository.GetItem(c => c.CommunityID == contentDetails.ParentID);
                        if (parentCommunity != null)
                        {
                            parentCommunity.ModifiedByID = contentDetails.CreatedByID;
                            parentCommunity.ModifiedDatetime = DateTime.UtcNow;

                            // Updated Parent child relationship
                            CommunityContents comCont = new CommunityContents()
                            {
                                Community = parentCommunity,
                                Content = content
                            };

                            // Add the relationship
                            content.CommunityContents.Add(comCont);
                        }

                        // Update video file details in database.
                        CreateVideo(content, contentDetails);

                        // Update associated files details in database.
                        CreateAssociateContents(content, contentDetails);

                        // Update Tags.
                        this.SetContentTags(contentDetails.Tags, content);

                        //// TODO: Update Permissions Details.
                        //// TODO: Update Consumed size of the user. If the size is more than allowed throw error.

                        this.contentRepository.Add(content);

                        // Save changes to the database
                        this.contentRepository.SaveChanges();

                        // Get the content ID from database. We need to retrieve it from the database as it is a identity column.
                        contentID = content.ContentID;
                    }
                    catch (Exception)
                    {
                        // Delete Uploaded Content/Video/Thumbnail
                        // Note:- Deleting associated files as will be handled by a worker role 
                        //  which will delete temporary file from container.
                        DeleteUploadedFiles(contentDetails);
                        throw;
                    }
                }
                else
                {
                    // Delete Uploaded Content/Video/Thumbnail
                    // Note:- Deleting associated files as will be handled by a worker role 
                    //  which will delete temporary file from container.
                    DeleteUploadedFiles(contentDetails);
                }
            }
            return contentID;
        }

        /// <summary>
        /// Updates the content in Layerscape with the given details passed in contentDetails instance.
        /// </summary>
        /// <param name="contentDetails">Details of the content</param>
        /// <param name="userID">User identification</param>
        /// <returns>True if content is updated; otherwise false.</returns>
        public bool UpdateContent(ContentDetails contentDetails, long userID)
        {
            // Make sure content is not null
            this.CheckNotNull(() => new { contentDetails });

            // Get the current content from DB.
            Content content = this.contentRepository.GetContent(contentDetails.ID);
            if (content != null)
            {
                UserRole userRole = GetContentUserRole(content, userID);

                if (!CanEditDeleteContent(content, userID, userRole))
                {
                    // In case if user is reader or visitor, he should not be allowed to edit the content.
                    // TODO: Throw item not exists or no permissions exception which will be shown to user in error page.
                    contentDetails = null;
                    this.CheckNotNull(() => new { contentDetails });
                }

                // Upload edited/removed content data, thumbnail, video and associated file to azure.
                if (UploadFilesToAzure(contentDetails, content))
                {
                    if (MoveAssociatedFiles(contentDetails))
                    {
                        try
                        {
                            // Do not let the user to change the Content as public in case if it is offensive.
                            // This scenario might happen when the edit page is left open or cached and meantime content is marked as offensive 
                            // by the Site Admin.
                            if (content.IsOffensive.HasValue && (bool)content.IsOffensive && contentDetails.AccessTypeID == (int)AccessType.Public)
                            {
                                contentDetails.AccessTypeID = (int)AccessType.Private;
                            }

                            // Set values from content details.
                            content.SetValuesFrom(contentDetails);

                            // Set the thumbnail ID.
                            if (contentDetails.Thumbnail.AzureID != content.ThumbnailID)
                            {
                                // Move Thumbnail.
                                content.ThumbnailID = MoveThumbnail(contentDetails.Thumbnail);
                            }

                            // Update Modified by and date time.
                            content.ModifiedByID = userID;
                            content.ModifiedDatetime = DateTime.UtcNow;

                            // Updated Parent child relationship
                            UpdateParent(contentDetails, content);

                            // Update video file details in database.
                            UpdateVideo(content, contentDetails);

                            // Update associated files details in database.
                            UpdateAssociateContents(contentDetails, content);

                            //// TODO: Update Consumed size of the user. If the size is more than allowed throw error.

                            // Update Tags.
                            this.SetContentTags(contentDetails.Tags, content);

                            this.contentRepository.Update(content);

                            // Save changes to the database
                            this.contentRepository.SaveChanges();

                            return true;
                        }
                        catch (Exception)
                        {
                            // Delete Uploaded Content/Video/Thumbnail
                            // Note:- Deleting associated files as will be handled by a worker role 
                            //  which will delete temporary file from container.
                            DeleteUploadedFiles(contentDetails);
                            throw;
                        }
                    }
                    else
                    {
                        // Delete Uploaded Content/Video/Thumbnail
                        // Note:- Deleting associated files as will be handled by a worker role 
                        //  which will delete temporary file from container.
                        DeleteUploadedFiles(contentDetails);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Uploads the associated file to temporary container.
        /// </summary>
        /// <param name="fileDetail">Details of the associated file.</param>
        /// <returns>True if content is uploaded; otherwise false.</returns>
        public bool UploadTemporaryFile(FileDetail fileDetail)
        {
            // Make sure file detail is not null
            this.CheckNotNull(() => new { fileDetail });

            BlobDetails fileBlob = new BlobDetails()
            {
                BlobID = fileDetail.AzureID.ToString(),
                Data = fileDetail.DataStream,
                MimeType = fileDetail.MimeType
            };

            return this.blobDataRepository.UploadTemporaryFile(fileBlob);
        }

        /// <summary>
        /// Gets the communities and folders which can be used as parent while creating a new 
        /// community/folder/content by the specified user.
        /// </summary>
        /// <param name="userID">User for which the parent communities/folders are being fetched</param>
        /// <returns>List of communities folders</returns>
        public IEnumerable<Community> GetParentCommunities(long userID)
        {
            // Need to check if the user is site admin or not.
            UserRole currentUserRole = this.userRepository.GetUserRole(userID, null);
            return this.communityRepository.GetParentCommunities(userID, -1, CommunityTypes.None, UserRole.Contributor, currentUserRole);
        }

        /// <summary>
        /// Increments the download count of the content identified by the ContentId.
        /// </summary>
        /// <param name="contentID">Content ID</param>
        /// <param name="userID">User Identification.</param>
        public void IncrementDownloadCount(long contentID, long userID)
        {
            Expression<Func<Content, bool>> condition = ((Content c) => c.ContentID == contentID);

            this.IncrementDownloadCount(condition, userID);
        }

        /// <summary>
        /// This function retrieves the contents uploaded by the user.
        /// </summary>
        /// <param name="userID">User identity.</param>
        /// <returns>Payload details.</returns>
        public PayloadDetails GetUserContents(long userID)
        {
            PayloadDetails payloadDetails = null;

            Expression<Func<Content, bool>> condition = (Content c) => c.CreatedByID == userID
                && c.IsDeleted == false
                && Enumerable.FirstOrDefault<CommunityContents>(c.CommunityContents) != null
                && !(bool)Enumerable.FirstOrDefault<CommunityContents>(c.CommunityContents).Community.IsDeleted;

            Func<Content, object> orderBy = (Content c) => c.ModifiedDatetime;

            var contents = this.contentRepository.GetItems(condition, orderBy, true);

            // Get Content Details object from Contents so that it has permission details
            List<ContentDetails> contentDetailsList = new List<ContentDetails>();
            foreach (Content content in contents)
            {
                UserRole userRole = GetContentUserRole(content, userID);

                // For private contents, user's who have not assigned explicit permission will not have access.
                if (userRole != UserRole.None)
                {
                    var contentDetails = new ContentDetails(userRole.GetPermission());
                    contentDetails.SetValuesFrom(content);
                    contentDetailsList.Add(contentDetails);
                }
            }

            payloadDetails = PayloadDetailsExtensions.InitializePayload();
            payloadDetails.Name = "My Contents";

            payloadDetails.SetValuesFrom(contentDetailsList);
            return payloadDetails;
        }

        /// <summary>
        /// Gets the role of the user on the given Content.
        /// </summary>
        /// <param name="content">Content on which user role has to be found</param>
        /// <param name="userID">Current user id</param>
        /// <returns>UserRole on the content</returns>
        public UserRole GetContentUserRole(Content content, long? userID)
        {
            UserRole userRole = UserRole.Visitor;

            if (content != null)
            {
                if (userID.HasValue && content.CreatedByID == userID.Value)
                {
                    userRole = UserRole.Owner;
                }
                else
                {
                    string accessType = this.contentRepository.GetContentAccessType(content.ContentID);
                    var parent = Enumerable.FirstOrDefault<CommunityContents>(content.CommunityContents);
                    if (parent != null)
                    {
                        if (userID.HasValue)
                        {
                            userRole = this.userRepository.GetUserRole(userID.Value, parent.Community.CommunityID);
                        }

                        if (userRole == UserRole.Moderator)
                        {
                            // In case of user is Moderator for the parent community, he should be considered as moderator inherited so 
                            // that he will be having permissions to edit/delete this content.
                            userRole = UserRole.ModeratorInheritted;
                        }
                        else if (userRole == UserRole.Contributor)
                        {
                            // In case of user is Contributor for the parent community, he should be considered as Owner if the content
                            // is created by him. If the content is not created by him, he should be considered as Reader.
                            if (content.CreatedByID == userID)
                            {
                                userRole = UserRole.Owner;
                            }
                            else
                            {
                                userRole = UserRole.Reader;
                            }
                        }
                    }

                    // 1. In case if Private content, only users who are given access (Reader/Contributor/Moderator/Owner) can access them.
                    // 2. If the required user role is not higher or equal to the user role, then return null.
                    if (accessType == AccessType.Private.ToString() && userRole < UserRole.Reader)
                    {
                        // No permissions for the user on the given community
                        return UserRole.None;
                    }
                }
            }

            return userRole;
        }

        /// <summary>
        /// Retrieves the latest content IDs for sitemap.
        /// </summary>
        /// <param name="count">Total Ids required</param>
        /// <returns>
        /// Collection of IDs.
        /// </returns>
        public IEnumerable<long> GetLatestContentIDs(int count)
        {
            return this.contentRepository.GetLatestContentIDs(count);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Updates the relationship for parent and content.
        /// </summary>
        /// <param name="contentDetails">Details of the content.</param>
        /// <param name="content">Content which has to be updated.</param>
        private static void UpdateParent(ContentDetails contentDetails, Content content)
        {
            // Few things to be noted:
            // a) Obviously the count to be 1 always.
            // b) A content can be child of only once parent community or folder
            if (Enumerable.ElementAt<CommunityContents>(content.CommunityContents, 0).CommunityID != contentDetails.ParentID)
            {
                if (content.CommunityContents.Count > 0)
                {
                    content.CommunityContents.Clear();
                }

                CommunityContents comCont = new CommunityContents()
                {
                    CommunityID = contentDetails.ParentID,
                    Content = content
                };

                // Add the relationship
                content.CommunityContents.Add(comCont);
            }
        }

        /// <summary>
        /// Updates the relationship for video and content.
        /// </summary>
        /// <param name="content">Content which has to be updated.</param>
        /// <param name="contentDetails">Details of the content.</param>
        private static void UpdateVideo(Content content, ContentDetails contentDetails)
        {
            var videoDetail = contentDetails.Video as FileDetail;
            if (videoDetail != null)
            {
                long deletedByID = content.ModifiedByID.Value;

                var existingVideo = Enumerable.Where<ContentRelation>(content.ContentRelation, cr => cr.ContentRelationshipTypeID == (int)AssociatedContentRelationshipType.Video
                                    && cr.Content1.IsDeleted == false).FirstOrDefault();

                if (existingVideo != null)
                {
                    // Remove the previous videos from azure.
                    existingVideo.Content1.IsDeleted = true;
                    existingVideo.Content1.DeletedByID = deletedByID;
                    existingVideo.Content1.DeletedDatetime = DateTime.UtcNow;

                    content.ContentRelation.Remove(existingVideo);
                }

                CreateVideo(content, contentDetails);
            }
        }

        /// <summary>
        /// Updates the relationship for video and content.
        /// </summary>
        /// <param name="content">Content which has to be updated.</param>
        /// <param name="contentDetails">Details of the content.</param>
        private static void CreateVideo(Content content, ContentDetails contentDetails)
        {
            if (contentDetails.Video != null)
            {
                Content videoContent = new Content();
                videoContent.SetValuesFrom(contentDetails, contentDetails.Video);

                // Note that Modifying user is the one who is creating the video.
                videoContent.CreatedByID = content.ModifiedByID.Value;

                var contentRelation = new ContentRelation()
                {
                    Content = content,
                    Content1 = videoContent,
                    ContentRelationshipTypeID = (int)AssociatedContentRelationshipType.Video
                };

                content.ContentRelation.Add(contentRelation);
            }
        }

        /// <summary>
        /// Updates the relationship for associated contents.
        /// </summary>
        /// <param name="contentDetails">Details of the content.</param>
        /// <param name="content">Content which has to be updated.</param>
        private static void UpdateAssociateContents(ContentDetails contentDetails, Content content)
        {
            long deletedByID = content.ModifiedByID.Value;

            // Get all Existing files list.
            var newFilesIDs = contentDetails.AssociatedFiles
                .Where(af => (af.ContentID.HasValue) && (af.ContentID.Value > 0))
                .Select(af => af.ContentID.Value);

            // Delete all existing associated files which are not part of the new associated file list.
            var removeAssociatedFiles = from cr in content.ContentRelation
                                        where cr.ContentRelationshipTypeID == (int)AssociatedContentRelationshipType.Associated
                                            && !newFilesIDs.Contains(cr.Content1.ContentID)
                                        select cr;

            foreach (var item in Enumerable.ToList<ContentRelation>(removeAssociatedFiles))
            {
                item.Content1.IsDeleted = true;
                item.Content1.DeletedByID = deletedByID;
                item.Content1.DeletedDatetime = DateTime.UtcNow;

                content.ContentRelation.Remove(item);
            }

            // Create new associated files.
            CreateAssociateContents(content, contentDetails);
        }

        /// <summary>
        /// Creates the relationship for associated contents.
        /// </summary>
        /// <param name="content">Content which has to be updated.</param>
        /// <param name="contentDetails">Details of the content.</param>
        private static void CreateAssociateContents(Content content, ContentDetails contentDetails)
        {
            if (contentDetails.AssociatedFiles != null && contentDetails.AssociatedFiles.Count() > 0)
            {
                foreach (var dataDetails in contentDetails.AssociatedFiles)
                {
                    // Create new associated file only if contentId is not set
                    if (!dataDetails.ContentID.HasValue || dataDetails.ContentID.Value <= 0)
                    {
                        Content associatedContent = new Content();
                        associatedContent.SetValuesFrom(contentDetails, dataDetails);

                        // Note that Modifying user is the one who is creating the associated contents.
                        associatedContent.CreatedByID = content.ModifiedByID.Value;

                        var associatedRelation = new ContentRelation()
                        {
                            Content = content,
                            Content1 = associatedContent,
                            ContentRelationshipTypeID = (int)AssociatedContentRelationshipType.Associated
                        };
                        content.ContentRelation.Add(associatedRelation);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the specified content from the Earth database recursively.
        /// </summary>
        /// <param name="contentID">Content Id</param>
        /// <param name="profileID">User Identity</param>
        /// <param name="isOffensive">Whether the Content is offensive or not?</param>
        /// <param name="offensiveDetails">Offensive Details.</param>
        /// <returns>True of the content is deleted. False otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore all exceptions")]
        private OperationStatus DeleteContentRecursively(long contentID, long profileID, bool isOffensive, OffensiveEntry offensiveDetails)
        {
            OperationStatus status = null;
            try
            {
                // Get the current content from DB.
                Content content = this.contentRepository.GetItem((c) => c.ContentID == contentID && c.IsDeleted == false);

                if (content != null)
                {
                    UserRole userRole = GetContentUserRole(content, profileID);

                    if (CanEditDeleteContent(content, profileID, userRole))
                    {
                        content.IsDeleted = true;
                        content.IsOffensive = isOffensive;
                        content.DeletedByID = profileID;
                        content.DeletedDatetime = DateTime.UtcNow;

                        // Update all the offensive entity entries if the content is being deleted.
                        UpdateAllOffensiveContentEntry(content.ContentID, offensiveDetails);

                        foreach (var contentRelation in content.ContentRelation)
                        {
                            contentRelation.Content1.IsDeleted = true;
                            contentRelation.Content1.IsOffensive = isOffensive;
                            contentRelation.Content1.DeletedByID = profileID;
                            contentRelation.Content1.DeletedDatetime = DateTime.UtcNow;
                        }

                        this.contentRepository.Update(content);

                        // Save changes to the database
                        this.contentRepository.SaveChanges();

                        status = OperationStatus.CreateSuccessStatus();
                    }
                    else
                    {
                        // In case if user is reader or visitor, he should not be allowed to delete the content.
                        status = OperationStatus.CreateFailureStatus("User does not have permission for deleting the content.");
                    }
                }
                else
                {
                    status = OperationStatus.CreateFailureStatus(string.Format(CultureInfo.CurrentCulture, "Content with ID '{0}' was not found", contentID));
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            return status;
        }

        /// <summary>
        /// Sets the Content-Tags relations for given Tags. If the tags are not there in the DB, they will be added.
        /// </summary>
        /// <param name="tagsString">Comma separated tags string</param>
        /// <param name="content">Content to which tags to be related</param>
        private void SetContentTags(string tagsString, Content content)
        {
            // Delete all existing tags which are not part of the new tags list.
            content.RemoveTags(tagsString);

            // Create Tags and relationships.
            if (!string.IsNullOrWhiteSpace(tagsString))
            {
                IEnumerable<string> tagsArray = tagsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
                if (tagsArray != null && tagsArray.Count() > 0)
                {
                    var notExistingTags = from tag in tagsArray
                                          where Enumerable.FirstOrDefault<ContentTags>(content.ContentTags, t => t.Tag.Name == tag) == null
                                          select tag;

                    foreach (string tag in notExistingTags)
                    {
                        Tag objTag = tagRepository.GetItem((Tag t) => t.Name == tag);

                        if (objTag == null)
                        {
                            objTag = new Tag();
                            objTag.Name = tag;
                        }

                        ContentTags contentTag = new ContentTags();
                        contentTag.Content = content;
                        contentTag.Tag = objTag;

                        content.ContentTags.Add(contentTag);
                    }
                }
            }
        }

        /// <summary>
        /// Uploads all files to azure.
        /// </summary>
        /// <param name="contentDetails">Details of the content.</param>
        private bool UploadFilesToAzure(ContentDetails contentDetails)
        {
            bool uploadStatus = true;

            // Upload content data.
            var fileDetail = contentDetails.ContentData as FileDetail;
            if (fileDetail != null)
            {
                // Move the content file from temporary container to file container.
                uploadStatus = MoveFile(fileDetail);
            }

            // Upload video.
            if (contentDetails.Video != null && uploadStatus)
            {
                var videoDetail = contentDetails.Video as FileDetail;
                if (videoDetail != null)
                {
                    // Move the video file from temporary container to file container.
                    uploadStatus = MoveFile(videoDetail);
                }
            }

            return uploadStatus;
        }

        /// <summary>
        /// Uploads all files to azure.
        /// </summary>
        /// <param name="contentDetails">Details of the content.</param>
        private bool UploadFilesToAzure(ContentDetails contentDetails, Content content)
        {
            bool uploadStatus = true;

            // Upload content data.
            var fileDetail = contentDetails.ContentData as FileDetail;
            if (fileDetail != null && content.ContentAzureID != fileDetail.AzureID)
            {
                // Move the content file from temporary container to file container.
                uploadStatus = MoveFile(fileDetail);
            }

            // Upload video.
            if (contentDetails.Video != null && uploadStatus)
            {
                var videoDetail = contentDetails.Video as FileDetail;
                if (videoDetail != null)
                {
                    // Move the video file from temporary container to file container.
                    uploadStatus = MoveFile(videoDetail);
                }
            }

            return uploadStatus;
        }

        /// <summary>
        /// Uploads associated files to azure.
        /// </summary>
        /// <param name="contentDetails">Details of the content.</param>
        private bool MoveAssociatedFiles(ContentDetails contentDetails)
        {
            bool status = true;
            if (contentDetails.AssociatedFiles != null && contentDetails.AssociatedFiles.Count() > 0)
            {
                foreach (var dataDetails in contentDetails.AssociatedFiles)
                {
                    var fileDetails = dataDetails as FileDetail;
                    if (fileDetails != null && (!fileDetails.ContentID.HasValue || fileDetails.ContentID.Value <= 0))
                    {
                        // Move files from temporary container to file container.
                        if (!MoveFile(fileDetails))
                        {
                            status = false;
                            break;
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Moves thumbnail from temporary storage to thumbnail storage in azure.
        /// </summary>
        /// <param name="fileDetails">Details of the thumbnail.</param>
        private Guid MoveThumbnail(FileDetail fileDetails)
        {
            Guid thumbnailId = Guid.Empty;
            if (fileDetails != null && fileDetails.AzureID != Guid.Empty)
            {
                BlobDetails thumbnailBlob = new BlobDetails()
                {
                    BlobID = fileDetails.AzureID.ToString()
                };

                thumbnailId = this.blobDataRepository.MoveThumbnail(thumbnailBlob) ? fileDetails.AzureID : Guid.Empty;
            }

            return thumbnailId;
        }

        /// <summary>
        /// Move temporary file to actual container in azure.
        /// </summary>
        /// <param name="fileDetails">Details of the file.</param>
        private bool MoveFile(FileDetail fileDetails)
        {
            BlobDetails fileBlob = new BlobDetails()
            {
                BlobID = fileDetails.AzureID.ToString(),
                MimeType = fileDetails.MimeType
            };

            return this.blobDataRepository.MoveFile(fileBlob);
        }

        /// <summary>
        /// Deletes already uploaded contents from azure.
        /// </summary>
        /// <param name="contentDetails">Details of the content.</param>
        private void DeleteUploadedFiles(ContentDetails contentDetails)
        {
            // Delete content data.
            var fileDetail = contentDetails.ContentData as FileDetail;
            if (fileDetail != null)
            {
                DeleteFile(fileDetail);
            }
            
            // Delete video.
            var videoDetail = contentDetails.Video as FileDetail;
            if (contentDetails.Video != null)
            {
                DeleteFile(videoDetail);
            }
        }

        /// <summary>
        /// Deletes thumbnail from azure.
        /// </summary>
        /// <param name="contentDetails">Details of the content.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "TODO: This method will be used later")]
        private void DeleteTemporaryThumbnail(ContentDetails contentDetails)
        {
            BlobDetails thumbnailBlob = new BlobDetails()
            {
                BlobID = contentDetails.Thumbnail.AzureID.ToString()
            };

            // Delete thumbnail from azure.
            this.blobDataRepository.DeleteThumbnail(thumbnailBlob);
        }

        /// <summary>
        /// Deletes file from azure.
        /// </summary>
        /// <param name="fileDetail">Details of the file.</param>
        private void DeleteFile(FileDetail fileDetail)
        {
            BlobDetails fileBlob = new BlobDetails()
            {
                BlobID = fileDetail.AzureID.ToString(),
                MimeType = fileDetail.MimeType
            };

            // Delete file from azure.
            this.blobDataRepository.DeleteFile(fileBlob);
        }

        /// <summary>
        /// Increments the download count of the content identified by the Condition.
        /// </summary>
        /// <param name="condition">Content Condition</param>
        /// <param name="userID">User Identification.</param>
        private void IncrementDownloadCount(Expression<Func<Content, bool>> condition, long userID)
        {
            var content = this.contentRepository.GetItem(condition);
            if (content != null)
            {
                UserRole userRole = GetContentUserRole(content, userID);
                if (CanReadContent(userRole))
                {
                    // Update download Count.
                    content.DownloadCount = content.DownloadCount.HasValue ? content.DownloadCount.Value + 1 : 1;
                    this.contentRepository.Update(content);

                    // Save changes to the database
                    this.contentRepository.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Updates the all the entries for the given Content with all the details.
        /// </summary>
        /// <param name="contentID">Content ID.</param>
        /// <param name="details">Details provided.</param>
        /// <returns>True if content was updated; otherwise false.</returns>
        private OperationStatus UpdateAllOffensiveContentEntry(long contentID, OffensiveEntry details)
        {
            OperationStatus status = null;
            try
            {
                var offensiveContents = this.offensiveContentRepository.GetItems(oc => oc.ContentID == contentID && oc.OffensiveStatusID == (int)OffensiveStatusType.Flagged, null, false);
                if (offensiveContents != null && offensiveContents.Count() > 0)
                {
                    foreach (var item in offensiveContents)
                    {
                        item.OffensiveStatusID = (int)details.Status;
                        item.Justification = details.Justification;

                        item.ReviewerID = details.ReviewerID;
                        item.ReviewerDatetime = DateTime.UtcNow;

                        this.offensiveContentRepository.Update(item);
                    }
                }
            }
            catch (Exception exception)
            {
                status = OperationStatus.CreateFailureStatus(exception);
            }

            // Status will be null if all sub communities and contents have been deleted. 
            // If one them is not deleted then the status will have the exception details.
            status = status ?? OperationStatus.CreateSuccessStatus();

            return status;
        }

        #endregion
    }
}
