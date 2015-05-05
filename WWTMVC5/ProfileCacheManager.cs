using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using WWTMVC5.Models;

namespace WWTMVC5
{
    public static class ProfileCacheManager
    {
        public static ProfileDetails GetProfileDetails(string accessToken)
        {
            try
            {
                ProfileDetails profileDetails = (ProfileDetails)HttpRuntime.Cache[accessToken];
                return profileDetails;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public static void CacheProfile(string accessToken, ProfileDetails profileDetails)
        {
            HttpRuntime.Cache.Add(
                accessToken,
                profileDetails,
                null,
                DateTime.Now.AddHours(1),
                Cache.NoSlidingExpiration,
                CacheItemPriority.Default,
                null);
        }
    }
}