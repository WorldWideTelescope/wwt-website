//-----------------------------------------------------------------------
// <copyright file="HttpRequestBaseExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;
using System.Security.Claims;
using System.Web;
using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for HttpRequestBase
    /// </summary>
    public static class HttpRequestBaseExtensions
    {
        /// <summary>
        /// Gets the identity PUID for the user
        /// </summary>
        /// <param name="thisObject">HttpContextBase object</param>
        /// <returns>user identity PUID</returns>
        public static string GetIdentityName(this HttpContextBase thisObject) 
        {
            var profileDetails = SessionWrapper.Get<ProfileDetails>("ProfileDetails");
            return profileDetails != null ? profileDetails.PUID : "";
        }

        /// <summary>
        /// Gets the Auth state token for the user
        /// </summary>
        /// <param name="thisObject">HttpContextBase object</param>
        /// <returns>AuthState Token</returns>
        public static string GetAuthStateToken(this HttpContextBase thisObject)
        {
            return SessionWrapper.Get<string>("AuthStateToken") ?? "";
        }

        /// <summary>
        /// Gets the Auth state token type for the user
        /// </summary>
        /// <param name="thisObject">HttpContextBase object</param>
        /// <returns>AuthState Token Type ID</returns>
        public static int GetAuthStateTokenType(this HttpContextBase thisObject)
        {
            int authStateTokenType = 2;

            //TODO: What is a ticketType? - how to get from LiveID
            //if (thisObject != null && thisObject.User.Identity != null && thisObject.User.Identity.IsAuthenticated)
            //{
            //    authStateTokenType = (int)((LiveIdentity)thisObject.User.Identity).TicketType;
            //}

            return authStateTokenType;
        }

        /// <summary>
        /// Gets the identity name for the user
        /// </summary>
        /// <param name="thisObject">HttpContextBase object</param>
        /// <returns>user identity name</returns>
        public static string GetIdentityProfileName(this HttpContextBase thisObject)
        {
            return SessionWrapper.Get<string>("CurrentUserProfileName") ?? "";
        }
    }
}