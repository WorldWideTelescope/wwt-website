using System;
using System.Collections.Generic;
////using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Microsoft.Research.WWTLib
{
    public class SForT
    {
        SqlConnections oSqlConnections;
        internal static string dbName;

		public DataTable FindTours(string sTKeyword, int NumReturn)
		{
			try
			{
				dbName = "WWTTours";
				oSqlConnections = new SqlConnections();
				SqlConnection sc = new SqlConnection(oSqlConnections.GetSqlConnection(dbName));
				SqlCommand c = new SqlCommand("spSearchTours", sc);
				c.CommandType = CommandType.StoredProcedure;
				c.Parameters.Add("@pSearchString", SqlDbType.NVarChar).Value = sTKeyword;
				c.Parameters.Add("@pNumReturnedTours", SqlDbType.Int).Value = NumReturn;

				DataTable dt = new DataTable();
				using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
				{
					dataAdapter.SelectCommand = (SqlCommand)c;
					dataAdapter.Fill(dt);
					return dt;
				}
			}
			catch
			{
				throw;
			}
		}

		public DataTable GetToursByCategory(int categoryId)
		{
			try
			{
				dbName = "WWTTours";
				oSqlConnections = new SqlConnections();
				SqlConnection sc = new SqlConnection(oSqlConnections.GetSqlConnection(dbName));
				SqlCommand c = new SqlCommand("spGetTourDetailsByCategoryId", sc);
				c.CommandType = CommandType.StoredProcedure;
				c.Parameters.Add("@CatId", SqlDbType.Int).Value = categoryId;

				DataTable dt = new DataTable();
				using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
				{
					dataAdapter.SelectCommand = (SqlCommand)c;
					dataAdapter.Fill(dt);
					return dt;
				}
			}
			catch
			{
				throw;
			}
		}

		public SqlDataReader GetTourCategories()
		{
			try
			{
				dbName = "WWTTours";
				oSqlConnections = new SqlConnections();

				return SqlHelper.ExecuteReader(oSqlConnections.GetSqlConnection(dbName), "spGetCategories");
			}
			catch
			{
				throw;
			}
		}

		public DataTable GetRandomTour()
		{
			string guid;
			dbName = "WWTTours";
			oSqlConnections = new SqlConnections();
			SqlConnection sc = new SqlConnection(oSqlConnections.GetSqlConnection(dbName));
			SqlCommand c = new SqlCommand("spGetActiveTourGUIDs", sc);
			c.CommandType = CommandType.StoredProcedure;

			DataSet ds = new DataSet();
			int rnd;
			using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
			{

				dataAdapter.SelectCommand = (SqlCommand)c;
				dataAdapter.Fill(ds);
				System.Random RandNum = new System.Random();
				rnd = RandNum.Next(0, ds.Tables[0].Rows.Count);
			}


			guid = (string)ds.Tables[0].Rows[rnd].ItemArray[0].ToString();
			return GetFeaturedTour(guid);
		}

		public DataTable GetFeaturedTour(string guid)
		{
			dbName = "WWTTours";
			oSqlConnections = new SqlConnections();
			SqlConnection sc = new SqlConnection(oSqlConnections.GetSqlConnection(dbName));
			SqlCommand c = new SqlCommand("spGetTourByGuid", sc);
			c.CommandType = CommandType.StoredProcedure;
			c.Parameters.Add("@Guid", SqlDbType.VarChar).Value = guid;

			DataSet ds = new DataSet();
			using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
			{
				dataAdapter.SelectCommand = (SqlCommand)c;
				dataAdapter.Fill(ds);
				return ds.Tables[0];
			}
		}

		public int GetRelatedToursCount(string guid)
		{
			dbName = "WWTTours";
			oSqlConnections = new SqlConnections();
			SqlConnection sc = new SqlConnection(oSqlConnections.GetSqlConnection(dbName));
			SqlCommand c = new SqlCommand("spGetRelatedTours", sc);
			c.CommandType = CommandType.StoredProcedure;
			c.Parameters.Add("@pTourGUID", SqlDbType.VarChar).Value = guid;

			DataSet ds = new DataSet();

			SqlDataAdapter dataAdapter = new SqlDataAdapter();

			dataAdapter.SelectCommand = (SqlCommand)c;
			dataAdapter.Fill(ds);
			return ds.Tables[0].Rows.Count;

		}

		public DataTable FindRelatedTours(string guid)
		{
			dbName = "WWTTours";
			oSqlConnections = new SqlConnections();
			SqlConnection sc = new SqlConnection(oSqlConnections.GetSqlConnection(dbName));
			SqlCommand c = new SqlCommand("spRelatedTourSearch", sc);
			c.CommandType = CommandType.StoredProcedure;
			c.Parameters.Add("@Guid", SqlDbType.VarChar).Value = guid;

			DataSet ds = new DataSet();
			using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
			{

				dataAdapter.SelectCommand = (SqlCommand)c;
				dataAdapter.Fill(ds);
				return ds.Tables[0];
			}
		}
    }
}
