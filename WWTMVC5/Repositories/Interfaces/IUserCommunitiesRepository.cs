//-----------------------------------------------------------------------
// <copyright file="IUserCommunitiesRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the UserCommunities repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IUserCommunitiesRepository : IRepositoryBase<UserCommunities>
    {
        /// <summary>
        /// Approves or declines a permission request of a user for a community and adds the user role of the community
        /// to the user communities table.
        /// </summary>
        /// <param name="permissionItem">Permission item with details about the request</param>
        /// <param name="updatedByID">User who is updating the permission request</param>
        /// <returns>Operation status with details</returns>
        OperationStatus UpdateUserPermissionRequest(PermissionItem permissionItem, long updatedByID);

         /// <summary>
        /// Updates the user roles for the current community. Takes the user roles of the parent community and joins
        /// with the current community being edited.
        /// </summary>
        /// <param name="childCommunity">Child community being edited</param>
        /// <param name="parentID">Parent community specified</param>
        void InheritParentRoles(Community childCommunity, long parentID);

        /// <summary>
        /// Updates the user roles with the given permission item which will be having the role to be updated for the
        /// community and the user.
        /// </summary>
        /// <param name="permissionItem">Role update details</param>
        /// <returns>Operation status with details</returns>
        OperationStatus UpdateUserRoles(PermissionItem permissionItem);
    }
}