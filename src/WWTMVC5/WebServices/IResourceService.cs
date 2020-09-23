//-----------------------------------------------------------------------
// <copyright file="IResourceService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WWTMVC5.WebServices
{
    [ServiceContract]
    public interface IResourceService
    {
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "User")]
        bool CheckIfUserExists();

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "User", Method = "POST")]
        bool RegisterUser();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Communities")]
        Stream GetMyCommunities();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Contents")]
        Stream GetMyContents();

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Community/{id}", Method = "DELETE")]
        bool DeleteCommunity(string id);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Content/{id}", Method = "DELETE")]
        bool DeleteContent(string id);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Content/Publish/{filename}", Method = "POST")]
        long PublishContent(string filename, Stream fileContent);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Payload")]
        Stream GetProfilePayload();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Community/{id}")]
        Stream GetCommunity(string id);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Community/{id}/Tours")]
        Stream GetAllTours(string id);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Community/{id}/Latest")]
        Stream GetLatest(string id);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Folder/{id}")]
        Stream GetFolder(string id);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Thumbnail/{id}")]
        Stream GetThumbnail(string id);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "File/{id}")]
        Stream GetFile(string id);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Download/{id}/{name}?wwtfull={wwtfull}")]
        Stream DownloadFile(string id, string name, string wwtfull);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Browse")]
        Stream GetBrowsePayload();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Browse/MostDownloadedContent")]
        Stream GetTopDownloadedContent();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Browse/TopRatedContent")]
        Stream GetTopRatedContent();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "WCF API"), OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Browse/LatestContent")]
        Stream GetLatestContent();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "WCF API"), OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Browse/TopRatedCommunity")]
        Stream GetTopRatedCommunity();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "WCF API"), OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Browse/LatestCommunity")]
        Stream GetLatestCommunity();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Browse/Categories")]
        Stream GetCategories();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Browse/Category/{id}")]
        Stream GetCategory(string id);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Content/Rate/{id}/{rating}", Method = "POST")]
        bool RateContent(string id, string rating);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "Community/Rate/{id}/{rating}", Method = "POST")]
        bool RateCommunity(string id, string rating);
    }
}
