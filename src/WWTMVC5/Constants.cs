//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace WWTMVC5
{
    /// <summary>
    /// Class representing all the constants used across the applications..
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Path separator for azure folders.
        /// </summary>
        public const string PathSeparator = "/";

        /// <summary>
        /// Default Mime Type for blobs.
        /// </summary>
        public const string DefaultMimeType = "application/octet-stream";

        /// <summary>
        /// WTML Mime Type for blobs.
        /// </summary>
        public const string WtmlMimeType = "application/x-wtml";

        /// <summary>
        /// WTT Mime Type for blobs.
        /// </summary>
        public const string WttMimeType = "application/x-wtt";

        /// <summary>
        /// WWTFIG Mime Type for blobs.
        /// </summary>
        public const string WwtfigMimeType = "application/x-wwtfig";

        /// <summary>
        /// WWTL Mime Type for blobs.
        /// </summary>
        public const string WwtlMimeType = "application/x-wwtl";

        /// <summary>
        /// Sign up file name format.
        /// </summary>
        public const string SignUpFileNameFormat = "{0}_Signup.wtml";

        /// <summary>
        /// File extension for collection file
        /// </summary>
        public const string CollectionFileExtension = ".WTML";

        /// <summary>
        /// File extension for tour file
        /// </summary>
        public const string TourFileExtension = ".WTT";

        /// <summary>
        /// File extension for layer file
        /// </summary>
        public const string LayerFileExtension = ".WWTL";

        /// <summary>
        /// Mime type for link
        /// </summary>
        public const string LinkMimeType = "link";

        /// <summary>
        /// Default Selected Search Type
        /// </summary>
        public const string BasicSearchType = "basic";

        /// <summary>
        /// Default Thumbnail Width.
        /// </summary>
        public const int DefaultThumbnailWidth = 160;

        /// <summary>
        /// Default Thumbnail Height
        /// </summary>
        public const int DefaultThumbnailHeight = 75;

        /// <summary>
        /// Default Profile Picture Width.
        /// </summary>
        public const int DefaultProfilePictureWidth = 111;

        /// <summary>
        /// Default Profile Picture Height
        /// </summary>
        public const int DefaultProfilePictureHeight = 111;

        /// <summary>
        /// Default Thumbnail Client Width.
        /// </summary>
        public const int DefaultClientThumbnailWidth = 96;

        /// <summary>
        /// Default Thumbnail Client Height
        /// </summary>
        public const int DefaultClientThumbnailHeight = 45;

        /// <summary>
        /// Default Thumbnail MimeType
        /// </summary>
        public const string DefaultThumbnailMimeType = "image/jpeg";

        /// <summary>
        /// Size threshold in bytes. If the actual message content is less than this limit
        /// the message content should be embedded, otherwise the content should be passed
        /// using a Blob.
        /// </summary>
        public const int MessageSizeThreshold = 6000;

        /// <summary>
        /// Gets Container name for files in azure.
        /// </summary>
        public static string ContainerName
        {
            get
            {
                return ConfigReader<string>.GetSetting("PrimaryContainer");
            }
        }

        /// <summary>
        /// Gets Container name for files in azure.
        /// </summary>
        public static string AssetContainerName
        {
            get
            {
                return ConfigReader<string>.GetSetting("AssetContainer");
            }
        }

        /// <summary>
        /// Gets Container name for thumbnail files in azure.
        /// </summary>
        public static string ThumbnailContainerName
        {
            get
            {
                return ConfigReader<string>.GetSetting("CommunitiesThumbnailContainer");
            }
        }

        /// <summary>
        /// Gets Temporary Container name.
        /// </summary>
        public static string TemporaryContainerName
        {
            get
            {
                return ConfigReader<string>.GetSetting("TemporaryContainer");
            }
        }

        /// <summary>
        /// Gets Name of the queue that receives messages processed by the Azure services.
        /// </summary>
        public static string AzureBackendQueueName
        {
            get
            {
                return ConfigReader<string>.GetSetting("QueueName");
            }
        }

        /// <summary>
        /// Gets Container name for notification message in azure.
        /// </summary>
        public static string NotificationContainerName
        {
            get
            {
                return ConfigReader<string>.GetSetting("NotificationContainer");
            }
        }

        /// <summary>
        /// Gets the Default Thumbnail ImageFormat
        /// </summary>
        public static ImageFormat DefaultThumbnailImageFormat
        {
            get
            {
                return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// Gets the number of comments to be fetched per page.
        /// </summary>
        public static int EntitiesPerUser
        {
            get
            {
                return ConfigReader<int>.GetSetting("EntitiesPerUser", 8);
            }
        }

        /// <summary>
        /// Gets the number of results to be fetched per entity type for Pivot Viewer.
        /// </summary>
        public static int PivotResultsCount
        {
            get
            {
                return ConfigReader<int>.GetSetting("PivotResultsCount", 50);
            }
        }

        /// <summary>
        /// Gets the number of Entities to be displayed per page.
        /// </summary>
        public static int HighlightEntitiesPerPage
        {
            get
            {
                // Default value is 4 if the value is not specified or wrong in the configuration file.
                return ConfigReader<int>.GetSetting("HighlightEntitiesPerPage", 4);
            }
        }

        /// <summary>
        /// Gets the number of days.
        /// </summary>
        public static int CommunityTourLatestFileDays
        {
            get
            {
                return ConfigReader<int>.GetSetting("LatestTourFileDays", 30);
            }
        }

        /// <summary>
        /// Gets the number permissions to be displayed per page.
        /// </summary>
        public static int PermissionsPerPage
        {
            get
            {
                return ConfigReader<int>.GetSetting("PermissionsPerPage", 30);
            }
        }

        /// <summary>
        /// Gets the number minimum rated people to be considered while fetching the top rated communities/contents.
        /// </summary>
        public static int MinRatedPeopleCount
        {
            get
            {
                return ConfigReader<int>.GetSetting("MinRatedPeopleCount", 2);
            }
        }

        /// <summary>
        /// Gets the number of total URLs to be added to sitemap
        /// </summary>
        public static int SitemapCount
        {
            get
            {
                return ConfigReader<int>.GetSetting("SitemapCount", 1000);
            }
        }

        /// <summary>
        /// Gets a value indicating whether mails for new entity should be sent or not.
        /// </summary>
        public static bool CanSendNewEntityMail
        {
            get
            {
                return ConfigReader<bool>.GetSetting("SendNewEntityMail", false);
            }
        }
    }
}