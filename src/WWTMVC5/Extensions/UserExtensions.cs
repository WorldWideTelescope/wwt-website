//-----------------------------------------------------------------------
// <copyright file="UserExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Properties;
using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for User.
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Gets the user name from DB. If First name or Last name is not provided, then None Provided will be returned. 
        /// </summary>
        /// <param name="thisObject">Current User instance on which the extension method is called</param>
        public static string GetFullName(this User thisObject)
        {
            string fullName = Resources.DefaultProfileName;
            if (thisObject != null)
            {
                return thisObject.FirstName + " " + thisObject.LastName;
                //if (!string.IsNullOrWhiteSpace(thisObject.FirstName) || !string.IsNullOrWhiteSpace(thisObject.LastName))
                //{
                //    fullName = thisObject.FirstName + " " + thisObject.LastName;
                //}
            }

            return fullName;
        }
    }
}