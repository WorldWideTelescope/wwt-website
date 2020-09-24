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
        /// Config setting name for getting the azure connection string.
        /// </summary>
        public const string EarthOnlineStorageSettingName = "EarthOnlineStorage";

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
        /// Timeout for azure retry policy.
        /// </summary>
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

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
        /// Name of the index which contains the Live ID token.
        /// </summary>
        public const string LiveTokenHeaderName = "LiveUserToken";

        /// <summary>
        /// Name of the index which contains the Live ID token.
        /// </summary>
        public const string LiveTokenTypeHeaderName = "LiveUserTokenType";

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
        /// Gets a value indicating whether we need to enable emailing or not.
        /// </summary>
        public static bool EnableEmailing
        {
            get
            {
                bool result = false;
                if (bool.TryParse(ConfigurationManager.AppSettings["EnableEmailing"], out result))
                {
                    // Dummy call
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the minimum duration in milliseconds for one iteration of the service loop.
        /// </summary>
        public static int MinMillisecondsPerIteration
        {
            get
            {
                int minMillisecondsPerIteration = 30;
                if (int.TryParse(ConfigurationManager.AppSettings["MinMillisecondsPerIteration"], out minMillisecondsPerIteration))
                {
                    // Dummy call
                }

                return minMillisecondsPerIteration;
            }
        }

        /// <summary>
        /// Gets the maximum duration in milliseconds for one iteration of the service loop.
        /// </summary>
        public static int MaxMillisecondsPerIteration
        {
            get
            {
                int maxMillisecondsPerIteration = 30;
                if (int.TryParse(ConfigurationManager.AppSettings["MaxMillisecondsPerIteration"], out maxMillisecondsPerIteration))
                {
                    // Dummy call
                }

                return maxMillisecondsPerIteration;
            }
        }

        /// <summary>
        /// Gets services's safe shutdown down.
        /// </summary>
        public static int ShutdownWaitTime
        {
            get
            {
                int maxMillisecondsPerIteration = 30;
                if (int.TryParse(ConfigurationManager.AppSettings["MaxMillisecondsPerIteration"], out maxMillisecondsPerIteration))
                {
                    // Dummy call
                }

                return maxMillisecondsPerIteration;
            }
        }

        /// <summary>
        /// Gets sender mail id.
        /// </summary>
        public static string SenderEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["SenderEmail"];
            }
        }

        /// <summary>
        /// Gets Microsoft Email id.
        /// </summary>
        public static string MicrosoftEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["MicrosoftEmail"];
            }
        }

        /// <summary>
        /// Gets Archival Email Path.
        /// </summary>
        public static string EmailRequestsArchivePath
        {
            get
            {
                string arhicvalPath = ConfigurationManager.AppSettings["EmailRequestsArchivePath"];
                if (!string.IsNullOrWhiteSpace(arhicvalPath))
                {
                    arhicvalPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EmailArchive");
                }

                return arhicvalPath;
            }
        }

        /// <summary>
        /// Gets sender display name.
        /// </summary>
        public static string SenderDisplayName
        {
            get
            {
                return ConfigurationManager.AppSettings["SenderDisplayName"];
            }
        }

        /// <summary>
        /// Gets ReplyTo mail id.
        /// </summary>
        public static string ReplyToEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["ReplyToEmail"];
            }
        }

        /// <summary>
        /// Gets Bcc mail id.
        /// </summary>
        public static string BccEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["BccEmail"];
            }
        }

        /// <summary>
        /// Gets ReplyTo display name.
        /// </summary>
        public static string ReplyToDisplayName
        {
            get
            {
                return ConfigurationManager.AppSettings["ReplyToDisplayName"];
            }
        }

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
                return ConfigReader<string>.GetSetting("ThumbnailContainer");
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
        /// Gets Retry policy for azure connections.
        /// </summary>
        public static IRetryPolicy DefaultRetryPolicy
        {
            get
            {
                return new LinearRetry(TimeSpan.FromSeconds(1), 3);
            }
        }

        /// <summary>
        /// Gets the number of comments to be fetched per page.
        /// </summary>
        public static int CommentsPerPage
        {
            get
            {
                return ConfigReader<int>.GetSetting("CommentsToBeFetched", 10);
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
        /// Gets the mail to URL.
        /// </summary>
        public static string MailToLinkFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["MailToUri"];
            }
        }

        /// <summary>
        /// Gets the twitter URL.
        /// </summary>
        public static string TwitterShareLinkFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["TwitterShareUri"];
            }
        }

        /// <summary>
        /// Gets the facebook URL.
        /// </summary>
        public static string FacebookShareLinkFormat
        {
            get
            {
                return ConfigurationManager.AppSettings["FacebookShareUri"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether authorization is required or not.
        /// </summary>
        public static bool IsAuthorizationRequired
        {
            get
            {
                bool result = false;
                if (bool.TryParse(ConfigurationManager.AppSettings["IsAuthorizationRequired"], out result))
                {
                    // Dummy call
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Multiple Mails should be sent or not.
        /// </summary>
        public static bool SendMultipleMails
        {
            get
            {
                return ConfigReader<bool>.GetSetting("SendMultipleMails", false);
            }
        }

        /// <summary>
        /// Gets the number of Entities to be fetched per page.
        /// </summary>
        public static int HighlightEntitiesToBeFetched
        {
            get
            {
                // Default value is 20 if the value is not specified or wrong in the configuration file.
                return ConfigReader<int>.GetSetting("HighlightEntitiesToBeFetched", 20);
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
        /// Gets the number of total pages shown
        /// </summary>
        public static int TotalPagesShown
        {
            get
            {
                return ConfigReader<int>.GetSetting("TotalPagesShown", 5);
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
        /// Gets the number of times we need to retry for sending mails.
        /// </summary>
        public static int RetryCount
        {
            get
            {
                int retryCount = 3;
                if (int.TryParse(ConfigurationManager.AppSettings["RetryCount"], out retryCount))
                {
                    // Dummy call
                }

                return retryCount;
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

        /// <summary>
        /// Gets the HomePageVideoGuid.
        /// </summary>
        public static string HomePageVideoGuid
        {
            get
            {
                return ConfigurationManager.AppSettings["HomePageVideoGuid"];
            }
        }

        /// <summary>
        /// Gets the WWT forum URL.
        /// </summary>
        public static string WWTForumUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["VisitOurForumUrl"];
            }
        }
    }
}