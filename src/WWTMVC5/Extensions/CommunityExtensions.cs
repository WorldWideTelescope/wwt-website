//-----------------------------------------------------------------------
// <copyright file="CommunityExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Community.
    /// </summary>
    public static class CommunityExtensions
    {
        /// <summary>
        /// Delete all existing tags which are not part of the new tags list.
        /// </summary>
        /// <param name="thisObject">Current Community instance on which the extension method is called</param>
        /// <param name="tags">Comma separated tags string</param>
        public static void RemoveTags(this Community thisObject, string tags)
        {
            if (thisObject != null)
            {
                if (!string.IsNullOrWhiteSpace(tags))
                {
                    IEnumerable<string> tagsArray = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
                    if (tagsArray != null && tagsArray.Count() > 0)
                    {
                        var removeTags = from ct in thisObject.CommunityTags
                                         where !tagsArray.Contains(ct.Tag.Name)
                                         select ct;

                        foreach (var item in removeTags.ToList())
                        {
                            thisObject.CommunityTags.Remove(item);
                        }
                    }
                    else if (thisObject.CommunityTags.Count > 0)
                    {
                        thisObject.CommunityTags.Clear();
                    }
                }
                else if (thisObject.CommunityTags.Count > 0)
                {
                    thisObject.CommunityTags.Clear();
                }
            }
        }
    }
}