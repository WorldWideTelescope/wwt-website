//-----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using WWTMVC5.Models;
using WWTMVC5.Properties;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts an string or an integer value to respective Enum.
        /// </summary>
        /// <typeparam name="TInput">
        /// Value Type.
        /// </typeparam>
        /// <typeparam name="TEnum">
        /// Enum Type.
        /// </typeparam>
        /// <param name="value">
        /// Value of the enum.
        /// </param>
        /// <param name="defaultValue">
        /// Default enum value.
        /// </param>
        /// <returns>
        /// Enum value corresponding to the value.
        /// </returns>
        public static TEnum ToEnum<TInput, TEnum>(this TInput value, TEnum defaultValue)
            where TEnum : struct
        {
            TEnum result = defaultValue;

            Type type = typeof(TEnum);

            if (value != null && type.IsEnum)
            {
                if (Enum.TryParse<TEnum>(value.ToString(), true, out result))
                {
                    // Dummy Call.
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the Enum to SelectList by adding all the Values in the Enum as SelectListItems. SelectList is needed for
        /// showing the dropdown when those Enums are shown in Edit forms of entities.
        /// </summary>
        /// <typeparam name="TEnum">Enum Type on which extension method is called</typeparam>
        /// <param name="value">Enum value on which extension method is called</param>
        /// <param name="ignoreItem">Enum value which should not be there in the Select list</param>
        /// <returns>SelectList instance of Enum</returns>
        public static SelectList ToSelectList<TEnum>(this TEnum value, TEnum ignoreItem)
            where TEnum : struct
        {
            return value.ToSelectList(new Collection<TEnum>() { ignoreItem });
        }

        /// <summary>
        /// Converts the Enum to SelectList by adding all the Values in the Enum as SelectListItems. SelectList is needed for
        /// showing the dropdown when those Enums are shown in Edit forms of entities.
        /// </summary>
        /// <typeparam name="TEnum">Enum Type on which extension method is called</typeparam>
        /// <param name="value">Enum value on which extension method is called</param>
        /// <param name="ignoreItems">Enum values which should not be there in the Select list</param>
        /// <returns>SelectList instance of Enum</returns>
        public static SelectList ToSelectList<TEnum>(this TEnum value, Collection<TEnum> ignoreItems)
            where TEnum : struct
        {
            if (typeof(TEnum).IsEnum)
            {
                var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                             where !ignoreItems.Contains(enumValue)
                             select new { ID = (int)Enum.Parse(typeof(TEnum), enumValue.ToString()), Name = Resources.ResourceManager.GetString(enumValue.ToString()) };

                return new SelectList(values.ToList(), "ID", "Name", ((int)Enum.Parse(typeof(TEnum), value.ToString())).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the Tool tip for the ContentType of current content type.
        /// </summary>
        /// <param name="thisObject">Current content type</param>
        /// <returns>ContentType Tool tip of the string</returns>
        public static string GetContentTypeToolTip(this ContentTypes thisObject)
        {
            string toolTip = Resources.FileToolTip;

            switch (thisObject)
            {
                case ContentTypes.Tours:
                    toolTip = Resources.WttToolTip;
                    break;

                case ContentTypes.Wtml:
                    toolTip = Resources.WtmlToolTip;
                    break;

                case ContentTypes.Excel:
                    toolTip = Resources.ExcelToolTip;
                    break;

                case ContentTypes.Link:
                    toolTip = Resources.LinkToolTip;
                    break;

                case ContentTypes.Wwtl:
                    toolTip = Resources.LayerFileToolTip;
                    break;

                case ContentTypes.Video:
                    toolTip = Resources.VideoToolTip;
                    break;

                case ContentTypes.Doc:
                case ContentTypes.Ppt:
                case ContentTypes.Generic:
                case ContentTypes.None:
                default:
                    toolTip = Resources.FileToolTip;
                    break;
            }

            return toolTip;
        }

        /// <summary>
        /// Gets the Image Name for the ContentType of current content type.
        /// </summary>
        /// <param name="thisObject">Current content type</param>
        /// <returns>ContentType Image as a string</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Not locale specific")]
        public static string GetContentTypeImage(this ContentTypes thisObject)
        {
            string imageName = "default" + thisObject.ToString().ToLowerInvariant() + "wwtthumbnail";

            if (thisObject == ContentTypes.None)
            {
                imageName = "default" + ContentTypes.Generic.ToString().ToLowerInvariant() + "wwtthumbnail";
            }

            return imageName;
        }

        /// <summary>
        /// Gets the users permission based on the UserRole.
        /// </summary>
        /// <param name="thisObject">User role for which permission to be obtained</param>
        /// <returns>Permission for the user role</returns>
        public static Permission GetPermission(this UserRole thisObject)
        {
            Permission permission = Permission.Visitor;

            if (thisObject == UserRole.Owner || UserRole.SiteAdmin == thisObject)
            {
                permission = Permission.Owner;
            }
            else if (thisObject == UserRole.Moderator)
            {
                permission = Permission.Moderator;
            }
            else if (thisObject == UserRole.ModeratorInheritted)
            {
                permission = Permission.ModeratorInheritted;
            }
            else if (thisObject == UserRole.Contributor)
            {
                permission = Permission.Contributor;
            }
            else if (thisObject == UserRole.Reader)
            {
                permission = Permission.Reader;
            }

            return permission;
        }

        /// <summary>
        /// Checks whether the current permission can do write (edit/delete) operations on the entities or not.
        /// </summary>
        /// <param name="thisObject">Permission instance</param>
        /// <returns>True if write operations are allowed, false otherwise.</returns>
        public static bool CanWrite(this Permission thisObject)
        {
            return (thisObject & Permission.Write) == Permission.Write;
        }

        /// <summary>
        /// Checks whether the current permission can do write (edit/delete) operations on the children content of the current entity or not.
        /// </summary>
        /// <param name="thisObject">Permission instance</param>
        /// <returns>True if write operations are allowed, false otherwise.</returns>
        public static bool CanWriteChildContent(this Permission thisObject)
        {
            return (thisObject & Permission.WriteChildContent) == Permission.WriteChildContent;
        }

        /// <summary>
        /// Checks whether the current permission can do write (edit/delete) operations on the children container of the current entity or not.
        /// </summary>
        /// <param name="thisObject">Permission instance</param>
        /// <returns>True if write operations are allowed, false otherwise.</returns>
        public static bool CanWriteChildContainer(this Permission thisObject)
        {
            return (thisObject & Permission.WriteChildContainer) == Permission.WriteChildContainer;
        }

        /// <summary>
        /// Checks whether the current permission can set the permissions (excluding owner permissions) for other users on current entity or not.
        /// </summary>
        /// <param name="thisObject">Permission instance</param>
        /// <returns>True if permit operations are allowed, false otherwise.</returns>
        public static bool CanSetPermits(this Permission thisObject)
        {
            return (thisObject & Permission.Permit) == Permission.Permit;
        }

        /// <summary>
        /// Checks whether the current permission can set the permissions (including owner permissions) for other users on current entity or not.
        /// </summary>
        /// <param name="thisObject">Permission instance</param>
        /// <returns>True if permit operations are allowed, false otherwise.</returns>
        public static bool CanSetOwnerPermits(this Permission thisObject)
        {
            return (thisObject & Permission.OwnerPermit) == Permission.OwnerPermit;
        }

        /// <summary>
        /// Checks the content's content type whether tours / Wwtl / Wtml
        /// </summary>
        /// <param name="thisObject">Content type instance</param>
        /// <returns> True or False</returns>
        public static bool IsWWWTContentType(this ContentTypes thisObject)
        {
            return (thisObject == ContentTypes.Tours || thisObject == ContentTypes.Wwtl || thisObject == ContentTypes.Wtml);
        }

        /// <summary>
        /// Get the label for download button based on the content type.
        /// </summary>
        /// <param name="thisObject">Content type instance</param>
        /// <returns>Label as string.</returns>
        public static string GetDownloadLinkLabel(this ContentTypes thisObject)
        {
            string buttonLabel = string.Empty;

            switch (thisObject)
            {
                case ContentTypes.Tours:
                    buttonLabel = Resources.ViewTourButtonLabel;
                    break;
                case ContentTypes.Wtml:
                    buttonLabel = Resources.ViewCollectionButtonLabel;
                    break;
                case ContentTypes.Excel:
                    buttonLabel = Resources.OpenExcelFileButtonLabel;
                    break;
                case ContentTypes.Wwtl:
                    buttonLabel = Resources.LoadDataLayerButtonLabel;
                    break;
                case ContentTypes.Video:
                    buttonLabel = Resources.ViewVideoLink;
                    break;
                default:
                    buttonLabel = Resources.DownloadContentLink;
                    break;
            }

            return buttonLabel;
        }

        /// <summary>
        /// Gets the Localized name for the ContentType.
        /// </summary>
        /// <param name="thisObject">Current content type</param>
        /// <returns>ContentType name string</returns>
        public static string GetName(this ContentTypes thisObject)
        {
            string contentTypeName = string.Empty;

            switch (thisObject)
            {
                case ContentTypes.Tours:
                    contentTypeName = Resources.ContentTypeTourName;
                    break;

                case ContentTypes.Wtml:
                    contentTypeName = Resources.ContentTypeWtmlName;
                    break;

                case ContentTypes.Excel:
                    contentTypeName = Resources.ContentTypeExcelName;
                    break;

                case ContentTypes.Link:
                    contentTypeName = Resources.ContentTypeLinkName;
                    break;

                case ContentTypes.Wwtl:
                    contentTypeName = Resources.ContentTypeWwtlName;
                    break;

                case ContentTypes.Video:
                    contentTypeName = Resources.ContentTypeVideoName;
                    break;

                case ContentTypes.Doc:
                    contentTypeName = Resources.ContentTypeDocName;
                    break;

                case ContentTypes.Ppt:
                    contentTypeName = Resources.ContentTypePptName;
                    break;

                case ContentTypes.Generic:
                    contentTypeName = Resources.ContentTypeGenericName;
                    break;

                case ContentTypes.None:
                default:
                    contentTypeName = Resources.CommunityTitle;
                    break;
            }

            return contentTypeName;
        }

        /// <summary>
        /// Gets the Localized name for the SearchSortBy options.
        /// </summary>
        /// <param name="thisObject">Current Sort By type</param>
        /// <returns>Search Sort By name string</returns>
        public static string GetName(this SearchSortBy thisObject)
        {
            string searchSortByName = string.Empty;

            switch (thisObject)
            {
                case SearchSortBy.Rating:
                    searchSortByName = Resources.SearchRatingFilter;
                    break;

                case SearchSortBy.Categories:
                    searchSortByName = Resources.SearchCategoriesFilter;
                    break;

                case SearchSortBy.DistributedBy:
                    searchSortByName = Resources.SearchDistributedByFilter;
                    break;

                case SearchSortBy.ContentType:
                    searchSortByName = Resources.SearchContentTypeFilter;
                    break;

                case SearchSortBy.None:
                default:
                    break;
            }

            return searchSortByName;
        }
    }
}