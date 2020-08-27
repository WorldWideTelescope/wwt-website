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
/// Summary description for AstroObjectServices
/// </summary>
/// 
namespace WWTWebservices
{
    internal static class AstroObjectServices
    {
        public class AstroObjectDataByRaDec
        {
            private AstroObjectsDataset ds;
            private string strErrorMsg;

            public AstroObjectDataByRaDec(float Ra, float Dec, float PlusMinusArcSecs)
            {
                strErrorMsg = "";
                SqlConnection myConnection2 = Database.GetConnectionAstroObjects();

                try
                {

                    myConnection2.Open();
                    SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetAstroObjects", myConnection2);

                    Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter CustParm = new SqlParameter("@pRa", SqlDbType.Float);
                    CustParm.Value = Ra;
                    Cmd2.SelectCommand.Parameters.Add(CustParm);

                    SqlParameter CustParm2 = new SqlParameter("@pDec", SqlDbType.Float);
                    CustParm2.Value = Dec;
                    Cmd2.SelectCommand.Parameters.Add(CustParm2);

                    SqlParameter CustParm3 = new SqlParameter("@pPlusMinusArcSecs", SqlDbType.Float);
                    CustParm3.Value = PlusMinusArcSecs;
                    Cmd2.SelectCommand.Parameters.Add(CustParm3);

                    ds = new AstroObjectsDataset();

                    Cmd2.Fill(ds, ds.Tables[0].TableName);
                }
                catch (Exception ex)
                {
                    throw
                        WWTWebService.RaiseException("GetAstroObjectByRADec", "http://WWTWebServices", ex.Message, "2000", "GetAstroObjectByRADec", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection2.State == ConnectionState.Open)
                        myConnection2.Close();
                }


            }



            public AstroObjectsDataset dsAstroObjectData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }


        public class AstroObjectByName
        {
            private AstroObjectsDataset ds;
            private string strErrorMsg;

            public AstroObjectByName(string AstroObjectName)
            {
                strErrorMsg = "";
                SqlConnection myConnection2 = Database.GetConnectionAstroObjects();

                try
                {

                    myConnection2.Open();
                    SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetAstroObjects", myConnection2);

                    Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter CustParm = new SqlParameter("@pAstroObjectName", SqlDbType.NVarChar);
                    CustParm.Value = AstroObjectName;
                    Cmd2.SelectCommand.Parameters.Add(CustParm);

                    ds = new AstroObjectsDataset();

                    Cmd2.Fill(ds, ds.Tables[0].TableName);
                }
                catch (Exception ex)
                {
                    throw
                        WWTWebService.RaiseException("GetAstroObjectsByName", "http://WWTWebServices", ex.Message, "2000", "GetAstroObjectsByName", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection2.State == ConnectionState.Open)
                        myConnection2.Close();
                }

            }

            public AstroObjectsDataset dsAstroObjectData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }


        public class AstroObjectDataInCatalog
        {
            private AstroObjectsDataset ds;
            private string strErrorMsg;

            public AstroObjectDataInCatalog(string CatalogName)
            {
                strErrorMsg = "";
                SqlConnection myConnection4 = Database.GetConnectionAstroObjects();

                try
                {

                    myConnection4.Open();
                    SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetCatalog", myConnection4);

                    Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter CustParm = new SqlParameter("@pCatalogName", SqlDbType.VarChar);
                    CustParm.Value = CatalogName;
                    Cmd2.SelectCommand.Parameters.Add(CustParm);

                    ds = new AstroObjectsDataset();

                    Cmd2.Fill(ds, ds.Tables[0].TableName);
                }
                catch (Exception ex)
                {
                    throw
                        WWTWebService.RaiseException("GetAstroObjectsInCatalog", "http://WWTWebServices", ex.Message, "2000", "GetAstroObjectsInCatalog", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection4.State == ConnectionState.Open)
                        myConnection4.Close();
                }


            }

            public AstroObjectsDataset dsAstroObjectData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }

    }
}
