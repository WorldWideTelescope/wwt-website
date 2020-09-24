using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


/// <summary>
/// Summary description for WWTWebService
/// </summary>

namespace WWTWebservices
{

[WebService(Namespace = "http://research.microsoft.com/WWT/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class WWTWebService : System.Web.Services.WebService {

    public enum FaultCode
    {
        Client = 0,
        Server = 1
    }


    public WWTWebService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 

    }

 
    public static SoapException RaiseException(string uri,
                                string webServiceNamespace,
                                string errorMessage,
                                string errorNumber,
                                string errorSource,
                                FaultCode code)
    {
        XmlQualifiedName faultCodeLocation = null;
        //Identify the location of the FaultCode
        switch (code)
        {
            case FaultCode.Client:
                faultCodeLocation = SoapException.ClientFaultCode;
                break;
            case FaultCode.Server:
                faultCodeLocation = SoapException.ServerFaultCode;
                break;
        }
        XmlDocument xmlDoc = new XmlDocument();
        //Create the Detail node
        XmlNode rootNode = xmlDoc.CreateNode(XmlNodeType.Element,
                           SoapException.DetailElementName.Name,
                           SoapException.DetailElementName.Namespace);
        //Build specific details for the SoapException
        //Add first child of detail XML element.
        XmlNode errorNode = xmlDoc.CreateNode(XmlNodeType.Element, "Error",
                                              webServiceNamespace);
        //Create and set the value for the ErrorNumber node
        XmlNode errorNumberNode =
          xmlDoc.CreateNode(XmlNodeType.Element, "ErrorNumber",
                            webServiceNamespace);
        errorNumberNode.InnerText = errorNumber;
        //Create and set the value for the ErrorMessage node
        XmlNode errorMessageNode = xmlDoc.CreateNode(XmlNodeType.Element,
                                                    "ErrorMessage",
                                                    webServiceNamespace);
        errorMessageNode.InnerText = errorMessage;
        //Create and set the value for the ErrorSource node
        XmlNode errorSourceNode =
          xmlDoc.CreateNode(XmlNodeType.Element, "ErrorSource",
                            webServiceNamespace);
        errorSourceNode.InnerText = errorSource;
        //Append the Error child element nodes to the root detail node.
        errorNode.AppendChild(errorNumberNode);
        errorNode.AppendChild(errorMessageNode);
        errorNode.AppendChild(errorSourceNode);
        //Append the Detail node to the root node
        rootNode.AppendChild(errorNode);
        //Construct the exception
        SoapException soapEx = new SoapException(errorMessage,
                                                 faultCodeLocation, uri,
                                                 rootNode);
        //Raise the exception  back to the caller
        return soapEx;
    }




    [WebMethod (CacheDuration = 120)]
    public AstroObjectsDataset GetAstroObjectsByRaDec(float Ra, float Dec, float PlusMinusArcSecs)
    {

        AstroObjectServices.AstroObjectDataByRaDec x2 = new AstroObjectServices.AstroObjectDataByRaDec(Ra, Dec, PlusMinusArcSecs);

        AstroObjectsDataset dsAstroObjectData = x2.dsAstroObjectData;

        return dsAstroObjectData;   
    }




    [WebMethod(CacheDuration = 120)]
    public AstroObjectsDataset GetAstroObjectsByName(string AstroObjectName)
    {
        AstroObjectServices.AstroObjectByName x1 = new AstroObjectServices.AstroObjectByName(AstroObjectName);

        AstroObjectsDataset dsAstroObjectData = x1.dsAstroObjectData;

        return dsAstroObjectData;   
    }




    [WebMethod(CacheDuration = 120)]
    public AstroObjectsDataset GetAstroObjectsInCatalog(string CatalogName)
    {

        AstroObjectServices.AstroObjectDataInCatalog x3 = new AstroObjectServices.AstroObjectDataInCatalog(CatalogName);

        AstroObjectsDataset dsAstroObjectData = x3.dsAstroObjectData;

        return dsAstroObjectData;   
    }


    [WebMethod]
    [System.Xml.Serialization.XmlInclude(typeof(Tour))]
    public List<Tour> ImportTour(string TourXML, byte[] TourBlob, byte[] TourThumbnail, byte[] AuthorThumbnail)
    {

        TourServices.WWTTour x4 = new TourServices.WWTTour(TourXML, TourBlob, TourThumbnail, AuthorThumbnail);

        List<Tour> dsWWTTourData = x4.dsTourFromCache;

        return dsWWTTourData;   
    }


    [WebMethod]
    [System.Xml.Serialization.XmlInclude(typeof(Tour))]
    public List<Tour> UpdateTourWorkFlowStatus(string TourGUID, char WorkFlowStatusCode, string ApprovedRejectedByName)
    {

        TourServices.WWTTourUpdtStatus x4 = new TourServices.WWTTourUpdtStatus(TourGUID, WorkFlowStatusCode, ApprovedRejectedByName);

        List<Tour> dsWWTTourData = x4.dsTourFromCache;

        return dsWWTTourData;   
    }

    [WebMethod]
    [System.Xml.Serialization.XmlInclude(typeof(Tour))]
    public List<Tour> InsertTourRatingOrComment(string UserGUID, string TourGUID, string Rating, string Comment, string UserSelfRatingID, string UserContactInfo, string ObjectionTypeID, string ObjectionComment)
    {
        int PassRating;
        int PassUserSelfRatingID;
        int PassObjectionTypeID; 

        try
        {
            PassRating = Convert.ToInt32(Rating);
        }
        catch
        {
            PassRating = -999;
        }
        try
        {
            PassUserSelfRatingID = Convert.ToInt32(UserSelfRatingID);
        }
        catch
        {
            PassUserSelfRatingID = -999;
        }
        try
        {
            PassObjectionTypeID = Convert.ToInt32(ObjectionTypeID);
        }
        catch
        {
            PassObjectionTypeID = -999;
        }

        TourServices.WWTTourInsertRatingOrComment x4 = new TourServices.WWTTourInsertRatingOrComment(UserGUID, TourGUID.ToUpper(), PassRating, Comment, PassUserSelfRatingID, UserContactInfo, PassObjectionTypeID, ObjectionComment);

        List<Tour> dsWWTTourData = x4.dsTourFromCache;

        return dsWWTTourData;
    }

    [WebMethod(CacheDuration = 120)]
    [System.Xml.Serialization.XmlInclude(typeof(Tour))]
    public List<Tour> GetToursForGUID(string TourGUID)
    {

        TourServices.WWTTourForGUID x4 = new TourServices.WWTTourForGUID(TourGUID);

        List<Tour> dsWWTTourData = x4.dsTourFromCache;

        return dsWWTTourData;   
    }




    [WebMethod(CacheDuration = 120)]
    [System.Xml.Serialization.XmlInclude(typeof(Tour))]
    public List<Tour> GetToursForAuthor(string AuthorName)
    {

        TourServices.WWTTourForAuthor x4 = new  TourServices.WWTTourForAuthor(AuthorName);

        List<Tour> dsWWTTourData = x4.dsTourFromCache;

        return dsWWTTourData;   
    }


    [WebMethod(CacheDuration = 120)]
    [System.Xml.Serialization.XmlInclude(typeof(Tour))]
    public List<Tour> GetToursForKeyword(string Keyword)
    {
        TourServices.WWTTourForKeyword x4 = new TourServices.WWTTourForKeyword(Keyword);

        List<Tour> dsWWTTourData = x4.dsTourFromCache;

        return dsWWTTourData;   
    }


    [WebMethod(CacheDuration = 120)]
    [System.Xml.Serialization.XmlInclude(typeof(Tour))]
    public List<Tour> GetToursForDateRange(string BeginDateTime, string EndDateTime)
    {
        TourServices.WWTTourForDateRange x4 = new TourServices.WWTTourForDateRange(BeginDateTime, EndDateTime);

        List<Tour> dsWWTTourData = x4.dsTourFromCache;

        return dsWWTTourData;  
    }


    }

}



