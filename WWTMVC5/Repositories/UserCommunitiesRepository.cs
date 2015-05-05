//-----------------------------------------------------------------------
// <copyright file="UserCommunitiesRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the UserCommunities repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public class UserCommunitiesRepository : RepositoryBase<UserCommunities>, IUserCommunitiesRepository
    {
        /// <summary>
        /// Initializes a new instance of the UserCommunitiesRepository class.
        /// </summary>
        /// <param name="earthOnlineDbContext">
        /// Instance of Layerscape db context
        /// </param>
        public UserCommunitiesRepository(EarthOnlineEntities earthOnlineDbContext)
            : base(earthOnlineDbContext)
        {
        }

        /// <summary>
        /// Approves or declines a permission request of a user for a community and adds the user role of the community
        /// to the user communities table.
        /// </summary>
        /// <param name="permissionItem">Permission item with details about the request</param>
        /// <param name="updatedByID">User who is updating the permission request</param>
        public OperationStatus UpdateUserPermissionRequest(PermissionItem permissionItem, long updatedByID)
        {
            OperationStatus operationStatus = OperationStatus.CreateSuccessStatus();

            PermissionRequest permissionRequest = Queryable.Where(this.EarthOnlineDbContext.PermissionRequest, (PermissionRequest pr) => pr.UserID == permissionItem.UserID && 
                                                    pr.CommunityID == permissionItem.CommunityID && 
                                                    pr.Approved == null).FirstOrDefault();

            // Make sure permissionRequest is not null
            this.CheckNotNull(() => new { permissionRequest });

            // Update the status of the permission request as approved or rejected.
            permissionRequest.Approved = permissionItem.Approved;
            permissionRequest.RespondedByID = updatedByID;
            permissionRequest.RespondedDate = DateTime.UtcNow;

            // Check if any existing user community role is already there for the user for the same community.
            UserCommunities existingUserCommunityRole = Queryable.Where(this.DbSet, (UserCommunities uc) => uc.UserID == permissionItem.UserID && uc.CommunityId == permissionItem.CommunityID).FirstOrDefault();

            // If the request is approved and also there are no roles for the same community and same user (not approved by anyone else) or 
            // the new role is higher than the existing role,
            // only then add the user and his role for the community in user communities list.
            if (permissionItem.Approved == true)
            {
                if (existingUserCommunityRole != null && existingUserCommunityRole.Role.RoleID >= (int)permissionItem.Role)
                {
                    operationStatus.Succeeded = false;
                    operationStatus.CustomErrorMessage = true;
                    operationStatus.ErrorMessage = Resources.MembershipExistsErrorMessage;

                    // Note that changes to permission request is not saved to the DB.
                    return operationStatus;
                }
                else
                {
                    permissionItem.IsInherited = false;

                    // Take the request which is being approved as only User Role which needs to be updated for the community
                    // whose request is getting approved and also for their children recursively.
                    UpdateCommunityPermission(permissionRequest.Community, permissionItem, false);
                }
            }
            else
            {
                // To update the permission request.
                this.Update(Enumerable.FirstOrDefault<UserCommunities>(permissionRequest.Community.UserCommunities));
            }

            this.SaveChanges();
            return operationStatus;
        }

        /// <summary>
        /// Updates the user roles for the current community. Takes the user roles of the parent community and joins
        /// with the current community being edited.
        /// </summary>
        /// <param name="childCommunity">Child community being edited</param>
        /// <param name="parentID">Parent community specified</param>
        public void InheritParentRoles(Community childCommunity, long parentID)
        {
            // Make sure childCommunity is not null
            this.CheckNotNull(() => new { childCommunity });

            if (parentID > 0)
            {
                Community parent = Queryable.Where(this.EarthOnlineDbContext.Community, (Community c) => c.CommunityID == parentID).FirstOrDefault();

                // Make sure parent community is not null
                this.CheckNotNull(() => new { parent });

                // Take all the user roles of parent community which needs to be updated for the current community
                // and also for their children recursively.
                foreach (var parentUserCommunities in parent.UserCommunities)
                {
                    PermissionItem permissionItem = new PermissionItem();
                    permissionItem.UserID = parentUserCommunities.UserID;
                    permissionItem.Role = (UserRole)parentUserCommunities.RoleID;
                    permissionItem.IsInherited = true;
                    UpdateCommunityPermission(childCommunity, permissionItem, false);
                }
            }
            else
            {
                foreach (var currentUserCommunities in childCommunity.UserCommunities)
                {
                    currentUserCommunities.IsInherited = false;
                }
            }
        }

        /// <summary>
        /// Updates the user roles with the given permission item which will be having the role to be updated for the
        /// community and the user. This is also used for deleting the user role and leaving community.
        /// </summary>
        /// <param name="permissionItem">Role update details</param>
        /// <returns>Operation status with details</returns>
        public OperationStatus UpdateUserRoles(PermissionItem permissionItem)
        {
            OperationStatus operationStatus = new OperationStatus();

            permissionItem.IsInherited = false;
            Community currentCommunity = Queryable.Where<Community>(this.EarthOnlineDbContext.Community, community => community.CommunityID == permissionItem.CommunityID).FirstOrDefault();

            UserCommunities userCommunityRole = Queryable.Where(this.DbSet, (UserCommunities uc) => uc.UserID == permissionItem.UserID && uc.CommunityId == permissionItem.CommunityID).FirstOrDefault();

            if (userCommunityRole != null)
            {
                // Get the parent community of the current community whose role is being updated.
                var parentCommunity = Enumerable.FirstOrDefault<CommunityRelation>(userCommunityRole.Community.CommunityRelation1);

                // 1. Role will be update the current community and also for all his children recursively.
                // 2. If this is the root community, mark inherited as false and update the permission for the community and its children.
                if (parentCommunity != null)
                {
                    // 3. Check whether user is having a role for the parent community.
                    UserCommunities userCommunity = Enumerable.Where(parentCommunity.Community.UserCommunities, (UserCommunities uc) => uc.UserID == permissionItem.UserID).FirstOrDefault();

                    if (userCommunity != null)
                    {
                        // 4. If the user's parent community role is higher than the current permission, it cannot be set and return immediately.
                        if (userCommunity.RoleID > (int)permissionItem.Role)
                        {
                            // TODO: Need to throw error message.
                            operationStatus.Succeeded = false;
                            operationStatus.CustomErrorMessage = true;
                            operationStatus.ErrorMessage = Resources.UserExistsInParentErrorMessage;
                            return operationStatus;
                        }
                        else if (userCommunity.RoleID == (int)permissionItem.Role)
                        {
                            // 5. If the roles are same, make it as inherited.
                            permissionItem.IsInherited = true;
                        }
                    }
                }
            }

            // Get the owners list for the community.
            IEnumerable<long> communityOwners = Queryable.Select<UserCommunities, long>(Queryable.Where(this.DbSet, (UserCommunities uc) => uc.CommunityId == permissionItem.CommunityID && 
                                                                                                                               uc.RoleID == (int)UserRole.Owner), (UserCommunities uc) => uc.UserID);

            // If there is only one owner and user role is being updated for him, then block the update (both edit and delete).
            if (communityOwners.Count() <= 1 && communityOwners.Contains(permissionItem.UserID))
            {
                // TODO: Need to throw error message.
                operationStatus.Succeeded = false;
                operationStatus.CustomErrorMessage = true;
                operationStatus.ErrorMessage = Resources.OnlyOwnerErrorMessage;
                return operationStatus;
            }

            UpdateCommunityPermission(currentCommunity, permissionItem, true);

            this.SaveChanges();

            operationStatus.Succeeded = true;
            return operationStatus;
        }

        /// <summary>
        /// Updates the user role for all the child communities recursively.
        /// </summary>
        /// <param name="community">Current community whose children need to be updated</param>
        /// <param name="permissionItem">Permission to be set</param>
        /// <param name="forceUpdate">Update the role even if they are higher, needed for update</param>
        private void UpdateCommunityPermission(Community community, PermissionItem permissionItem, bool forceUpdate)
        {
            // Check if any existing user community role is already there for the user for the same community.
            UserCommunities existingUserCommunityRole = Enumerable.Where(community.UserCommunities, (UserCommunities uc) => uc.UserID == permissionItem.UserID).FirstOrDefault();

            if (permissionItem.Role == UserRole.None && existingUserCommunityRole != null)
            {
                // If the role is none, then delete the user role for the community.
                this.Delete(existingUserCommunityRole);
            }
            else if (permissionItem.Role != UserRole.None)
            {
                // Add new user roles only for update roles not for delete roles (UserRole.None).
                if (existingUserCommunityRole == null)
                {
                    // If there are no roles for child community, then add the user role mapping for the child community.
                    var userCommunityRole = new UserCommunities();

                    userCommunityRole.CommunityId = community.CommunityID;
                    userCommunityRole.UserID = permissionItem.UserID;
                    userCommunityRole.RoleID = (int)permissionItem.Role;
                    userCommunityRole.IsInherited = permissionItem.IsInherited;
                    userCommunityRole.CreatedDatetime = DateTime.UtcNow;

                    community.UserCommunities.Add(userCommunityRole);
                }
                else if (existingUserCommunityRole.RoleID < (int)permissionItem.Role || forceUpdate)
                {
                    // If already there is a role assigned to the user and it is lesser than the role being assigned, then
                    // update the role with current higher role.
                    existingUserCommunityRole.RoleID = (int)permissionItem.Role;
                    existingUserCommunityRole.IsInherited = permissionItem.IsInherited;
                }
                else if (existingUserCommunityRole.RoleID == (int)permissionItem.Role)
                {
                    // If already there is a role assigned to the user and it is equal to the role being assigned, then
                    // just set the role as inherited from parent.
                    existingUserCommunityRole.IsInherited = permissionItem.IsInherited;
                    return;
                }
                else
                {
                    // If already there is a role assigned to the user and it is higher than the role being assigned, then
                    // no need to update the role being assigned.
                    return;
                }
            }

            // For each children, update the role.
            foreach (var childCommunityRelation in community.CommunityRelation)
            {
                permissionItem.IsInherited = true;

                // Continue with other child communities in case if the role is not higher.
                UpdateCommunityPermission(childCommunityRelation.Community1, permissionItem, forceUpdate);
            }
        }
    }
}