//-----------------------------------------------------------------------
// <copyright file="Enum.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace WWTMVC5
{
    /// <summary>
    /// Enum representing the Highlight Type for the communities or contents.
    /// </summary>
    public enum HighlightType
    {
        /// <summary>
        /// Visitor of the highlights
        /// </summary>
        None,

        /// <summary>
        /// Featured entities
        /// </summary>
        Featured,

        /// <summary>
        /// Latest entities
        /// </summary>
        Latest,

        /// <summary>
        /// Popular entities
        /// </summary>
        Popular,

        /// <summary>
        /// Related entities
        /// </summary>
        Related,

        /// <summary>
        /// Most downloaded entities.
        /// </summary>
        MostDownloaded
    }

    /// <summary>
    /// Enum representing the Order Type for the comments.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Newest First
        /// </summary>
        NewestFirst,

        /// <summary>
        /// Oldest First
        /// </summary>
        OldestFirst,
    }

    /// <summary>
    /// Enum representing the Category Type for the communities or contents.
    /// </summary>
    public enum CategoryType
    {
        /// <summary>
        /// All categories
        /// </summary>
        All = 0,

        /// <summary>
        /// Solid Earth category
        /// </summary>
        SolidEarth = 1,

        /// <summary>
        /// Ancient Earth category
        /// </summary>
        AncientEarth = 2,

        /// <summary>
        /// Atmosphere category
        /// </summary>
        Atmosphere = 3,

        /// <summary>
        /// Climate category
        /// </summary>
        Climate = 4,

        /// <summary>
        /// Astronomy category
        /// </summary>
        Astronomy = 5,

        /// <summary>
        /// Oceans and Rivers category
        /// </summary>
        OceansRivers = 6,

        /// <summary>
        /// Cold Regions category
        /// </summary>
        ColdRegions = 7,

        /// <summary>
        /// Planetary Science category
        /// </summary>
        Planets = 8,

        /// <summary>
        /// General category
        /// </summary>
        GeneralInterest = 9,

        /// <summary>
        /// How To category
        /// </summary>
        HowTo = 10,

        /// <summary>
        /// Life Science category
        /// </summary>
        LifeScience = 11,

        /// <summary>
        /// Earth Surface category
        /// </summary>
        EarthSurface = 12,

        //New Categories (June 2015)
        LearningWWT = 13,
        Nebula=14,
        Galaxies=15,
        Surveys = 16,
        BlackHoles=17,
        Supernova=18,
        StarClusters=19,
        Kiosk=20,
        Mars=21,
        Other=22,
        WWT=23,
        CosmicEvents=24,
        WWTAmbassadors=25,
        Educators=26,
        NewTours=27

    }

    /// <summary>
    /// Enum representing the Entity Type for the communities/contents/folders.
    /// </summary>
    public enum EntityType
    {
        /// <summary>
        /// All entities
        /// </summary>
        All,

        /// <summary>
        /// Community Entity
        /// </summary>
        Community,

        /// <summary>
        /// Folder entity
        /// </summary>
        Folder,

        /// <summary>
        /// Content Entity
        /// </summary>
        Content,

        /// <summary>
        /// User Entity
        /// </summary>
        User
    }

    /// <summary>
    /// Enum representing the Content Relationship Type for the contents.
    /// </summary>
    public enum AssociatedContentRelationshipType
    {
        /// <summary>
        /// Visitor relationship type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Associated content
        /// </summary>
        Associated = 1,

        /// <summary>
        /// Video content
        /// </summary>
        Video = 2,
    }

    /// <summary>
    /// Enum representing the Content Types for the contents.
    /// </summary>
    public enum ContentTypes
    {
         All = -1,
        /// <summary>
        /// Visitor type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Tour content.
        /// </summary>
        Tours = 1,

        /// <summary>
        /// Collection (WTML) content.
        /// </summary>
        Wtml = 2,

        /// <summary>
        /// Excel spreadsheet
        /// </summary>
        Excel = 3,

        /// <summary>
        /// Word document
        /// </summary>
        Doc = 4,

        /// <summary>
        /// PowerPoint presentation.
        /// </summary>
        Ppt = 5,

        /// <summary>
        /// Link content.
        /// </summary>
        Link = 6,

        /// <summary>
        /// Any other contents.
        /// </summary>
        Generic = 7,

        /// <summary>
        /// Layer file.
        /// </summary>
        Wwtl = 8,

        /// <summary>
        /// Video file.
        /// </summary>
        Video = 9,
       
    }

    /// <summary>
    /// Enum representing the Offence Type for the communities or contents.
    /// </summary>
    public enum ReportEntityType
    {
        /// <summary>
        /// Visitor type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Abusive offence.
        /// </summary>
        Abusive = 1,

        /// <summary>
        /// Profane offence.
        /// </summary>
        Profane = 2,

        /// <summary>
        /// Copyright Violation offence.
        /// </summary>
        CopyrightViolation = 3,

        /// <summary>
        /// Incorrect Information offence.
        /// </summary>
        IncorrectInformation = 4,

        /// <summary>
        /// Any Other offence.
        /// </summary>
        Other = 5
    }

    /// <summary>
    ///  Enum representing the Offensive status for the communities or contents.
    /// </summary>
    public enum OffensiveStatusType
    {
        /// <summary>
        /// Visitor status
        /// </summary>
        None = 0,

        /// <summary>
        /// Flagged status
        /// </summary>
        Flagged = 1,

        /// <summary>
        /// Reviewed status
        /// </summary>
        Reviewed = 2,

        /// <summary>
        /// Offensive status
        /// </summary>
        Offensive = 3,

        /// <summary>
        /// Deleted Status
        /// </summary>
        Deleted = 4
    }

    /// <summary>
    /// Enum representing the access types of the entities.
    /// </summary>
    public enum AccessType
    {
        /// <summary>
        /// None access type
        /// </summary>
        None = 0,

        /// <summary>
        /// Private access type
        /// </summary>
        Private = 1,

        /// <summary>
        /// Public access type
        /// </summary>
        Public = 2
    }

    /// <summary>
    /// Enum representing the type of user.
    /// The primary use of this type is used to identify the storage limits of user.
    /// Also this can be used for other purpose.
    /// </summary>
    public enum UserTypes
    {
        /// <summary>
        /// None user type
        /// </summary>
        None = 0,

        /// <summary>
        /// Site administrator
        ///     Storage limit = 10 GB
        /// </summary>
        SiteAdmin = 1,

        /// <summary>
        /// Premium user type
        ///     Storage limit = 10 GB
        /// </summary>
        Premium = 2,

        /// <summary>
        /// Regular user type. 
        ///     Storage limit = 5 GB
        /// </summary>
        Regular = 3
    }

    /// <summary>
    /// Enum representing the type of community.
    /// </summary>
    public enum CommunityTypes
    {
        /// <summary>
        /// Visitor entity type
        /// </summary>
        None = 0,

        /// <summary>
        /// Community type
        /// </summary>
        Community = 1,

        /// <summary>
        /// Folder Type
        /// </summary>
        Folder = 2,

        /// <summary>
        /// User entity type 
        /// </summary>
        User = 3
    }

    /// <summary>
    /// Enum representing the type of permission.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// No permission given on any community or content. DO NOT Change the number order since the master database
        /// will be using 1 for Reader, 2 for Contributor, and so on.
        /// </summary>
        None = -1,

        /// <summary>
        /// Visitor, the least permission type who can access only public Community/Content
        /// </summary>
        Visitor = 0,

        /// <summary>
        /// Reader, who have read access to private Community/Content
        /// </summary>
        Reader = 1,

        /// <summary>
        /// Contributor, who have read access to private Community/Content and also can create/edit his own content
        /// </summary>
        Contributor = 2,

        /// <summary>
        /// Moderator, who have read access to private Community/Content, can create/edit his own Community/Content
        /// </summary>
        Moderator = 3,

        /// <summary>
        /// Moderator, who have read access to private Community/Content, can create/edit his own Community/Content
        ///     Moderator role is inherited from parent Community/Content. Also DO NOT change the number order.
        /// </summary>
        ModeratorInheritted = 4,

        /// <summary>
        /// Owner, who is having all access to the community owned by him
        /// </summary>
        Owner = 5,

        /// <summary>
        /// Site Admin, who is having access to all the communities and contents
        /// </summary>
        SiteAdmin = 6
    }

    /// <summary>
    /// Enum representing the tabs to be shown in Permissions.
    /// </summary>
    public enum PermissionsTab
    {
        /// <summary>
        /// Request is not from any tab, like Leave community or join community
        /// </summary>
        None = 0,

        /// <summary>
        /// User permissions tab
        /// </summary>
        Users = 1,

        /// <summary>
        /// Permissions Requests tab
        /// </summary>
        Requests = 2,

        /// <summary>
        /// This is for the my request page in profile
        /// </summary>
        ProfileRequests = 3
    }

    /// <summary>
    /// Enum/Flag representing the Permission of user.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "TODO")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Cannot rename visitor to None as it is the required behavior.")]
    [Flags]
    public enum Permission
    {
        /// <summary>
        /// User requested for permission, pending approval.
        ///     NOTE: Negative and positive values is flags are equal, added negative so that it will not affect the 
        ///     existing values shared for WWT client. In case if +1024 is added, this values has to be changed.
        /// </summary>
        [XmlEnum("-1024")]
        PendingApproval = -1024,

        /// <summary>
        /// Visitor permissions on any Community/Folder/Content
        /// </summary>
        [XmlEnum("0")]
        Visitor = 0,

        /// <summary>
        /// View permissions on Community/Folder/Content
        /// </summary>
        [XmlEnum("1")]
        Read = 1,

        /// <summary>
        /// Add/delete/update only contents of the current Community/Folder
        /// </summary>
        [XmlEnum("2")]
        WriteChildContent = 2,

        /// <summary>
        /// Add/delete/update community/content/folder of the current Community/Folder
        /// </summary>
        [XmlEnum("4")]
        WriteChildContainer = 4,

        /// <summary>
        /// Update the current community/folder/content
        /// </summary>
        [XmlEnum("8")]
        Write = 8,

        /// <summary>
        /// Update the permissions for other users (not owners).
        /// </summary>
        [XmlEnum("16")]
        Permit = 16,

        /// <summary>
        /// Update the permissions for other users including owners.
        /// </summary>
        [XmlEnum("32")]
        OwnerPermit = 32,

        /// <summary>
        /// Reader permissions
        /// </summary>
        [XmlEnum("1")]
        Reader = Read,

        /// <summary>
        /// Contributor permissions.
        /// </summary>
        [XmlEnum("3")]
        Contributor = Read | WriteChildContent,

        /// <summary>
        /// Moderator permissions.
        /// </summary>
        [XmlEnum("23")]
        Moderator = Read | WriteChildContent | WriteChildContainer | Permit,

        /// <summary>
        /// Inherited Moderator permissions.
        /// </summary>
        [XmlEnum("31")]
        ModeratorInheritted = Read | WriteChildContent | WriteChildContainer | Permit | Write,

        /// <summary>
        /// Owner permissions.
        /// </summary>
        [XmlEnum("63")]
        Owner = Read | WriteChildContent | WriteChildContainer | Write | Permit | OwnerPermit
    }

    /// <summary>
    /// Enum representing the static content types.
    /// </summary>
    public enum StaticContentType
    {
        /// <summary>
        /// None static content type
        /// </summary>
        None = 0,

        /// <summary>
        /// Home page help static content type
        /// </summary>
        HomePageHelpText = 1,

        /// <summary>
        /// FAQ Page static content type
        /// </summary>
        FAQ = 2,

        /// <summary>
        /// WWT Install page static content type
        /// </summary>
        WWTInstall = 3,

        /// <summary>
        /// Excel Install page
        /// </summary>
        ExcelInstall = 4,

        /// <summary>
        /// Excel Help page
        /// </summary>
        ExcelHelp = 5,

        /// <summary>
        /// Learn More page
        /// </summary>
        LearnMore = 6,

        /// <summary>
        /// Get Started Page
        /// </summary>
        GetStarted = 7,

        /// <summary>
        /// Visualizing Content in WWT page
        /// </summary>
        VisualizingContentinWWT = 8,

        /// <summary>
        /// Narwhal page
        /// </summary>
        Narwhal = 9,

        /// <summary>
        /// WWT Add-in for Excel page
        /// </summary>
        WWTAddinForExcel = 10,
    }

    /// <summary>
    /// Enum representing the number of items in search results page.
    /// </summary>
    public enum SearchResultsPerPage
    {
        /// <summary>
        /// Ten Items, default
        /// </summary>
        Ten = 10,

        /// <summary>
        /// Twenty Five Items
        /// </summary>
        TwentyFive = 25,

        /// <summary>
        /// Fifty Items
        /// </summary>
        Fifty = 50,

        /// <summary>
        /// Hundred Items
        /// </summary>
        Hundred = 100
    }

    /// <summary>
    /// Enum representing the sorting for search.
    /// </summary>
    public enum SearchSortBy
    {
        /// <summary>
        /// Default Sorting
        /// </summary>
        None = 0,

        /// <summary>
        /// Sort By Rating
        /// </summary>
        Rating = 1,

        /// <summary>
        /// Sort By Categories
        /// </summary>
        Categories = 2,

        /// <summary>
        /// Sort By DistributedBy
        /// </summary>
        DistributedBy = 3,

        /// <summary>
        /// Sort By ContentType
        /// </summary>
        ContentType = 4,
    }

    /// <summary>
    /// Enum representing the Actions performed by Admin.
    /// </summary>
    public enum AdminActions
    {
        /// <summary>
        /// Delete Action
        /// </summary>
        Delete = 0,

        /// <summary>
        /// Mark As Private action
        /// </summary>
        MarkAsPrivate = 1,

        /// <summary>
        /// Mark As public action
        /// </summary>
        MarkAsPublic = 2,
    }
}
