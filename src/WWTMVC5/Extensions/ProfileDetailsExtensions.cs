//-----------------------------------------------------------------------
// <copyright file="ProfileDetailsExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for ProfileDetails class.
    /// </summary>
    public static class ProfileDetailsExtensions
    {
        /// <summary>
        /// Gets the profile name from the profile details object. Concatenates the first name and last name and returns the profile name.
        /// </summary>
        /// <param name="profileDetails">Profile details object.</param>
        /// <returns>Profile name</returns>
        public static string GetProfileName(this ProfileDetails profileDetails)
        {
            string profileName = string.Empty;

            if (profileDetails != null)
            {
                profileName = string.Join(" ", new string[] { profileDetails.FirstName, profileDetails.LastName });
                profileName = profileName.Trim();
            }

            return profileName;
        }

        /// <summary>
        /// Set the profile details like User ID, profile name and a Boolean indicating whether the user is site admin or not
        /// in to the session variables.
        /// </summary>
        /// <param name="profileDetails">Profile details object.</param>
        public static void SetProfileSessionValues(this ProfileDetails profileDetails)
        {
            if (profileDetails != null)
            {
                SessionWrapper.Set<long>("CurrentUserID", profileDetails.ID);
                SessionWrapper.Set<bool>("IsSiteAdmin", profileDetails.UserType == UserTypes.SiteAdmin ? true : false);
                SessionWrapper.Set<string>("CurrentUserProfileName", profileDetails.GetProfileName());
            }
        }
    }
}