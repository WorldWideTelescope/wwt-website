using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class PostRatingFeedbackProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public PostRatingFeedbackProvider(WwtOptions options)
        {
            _options = options;
        }

        public override string ContentType => ContentTypes.Xml;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            try
            {
                string query = context.Request.Params["Q"];
                string[] values = query.Split(',');
                string tour = values[0];
                string user = values[1];
                int rating = Convert.ToInt32(values[2]);
                if (rating > -1 && rating < 6)
                {
                    PostFeedback(tour, user, rating);
                }
            }
            catch
            {
            }
            context.Response.End();
            return Task.CompletedTask;
        }

        private SqlConnection GetConnectionWWTTours()
        {
            string connStr = null;
            connStr = _options.WwtToursDBConnectionString;
            SqlConnection myConnection = null;
            myConnection = new SqlConnection(connStr);
            return myConnection;
        }

        private void PostFeedback(string tour, string User, int rating)
        {
            string strErrorMsg;
            SqlConnection myConnection5 = GetConnectionWWTTours();

            try
            {
                myConnection5.Open();

                SqlCommand Cmd = null;
                Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandTimeout = 20;
                Cmd.Connection = myConnection5;

                Cmd.CommandText = "spInsertTourRatingOrComment";

                SqlParameter CustParm = new SqlParameter("@pUserGUID", SqlDbType.VarChar);
                CustParm.Value = User.ToString();
                Cmd.Parameters.Add(CustParm);

                SqlParameter CustParm2 = new SqlParameter("@pTourGUID", SqlDbType.VarChar);
                CustParm2.Value = tour.ToString();
                Cmd.Parameters.Add(CustParm2);

                SqlParameter CustParm3 = new SqlParameter("@pRating", SqlDbType.VarChar);
                CustParm3.Value = rating;
                Cmd.Parameters.Add(CustParm3);

                Cmd.ExecuteNonQuery();


            }
            catch (InvalidCastException)
            { }

            catch (Exception ex)
            {
                //throw ex.GetBaseException();
                strErrorMsg = ex.Message;
                return;
            }
            finally
            {
                if (myConnection5.State == ConnectionState.Open)
                {
                    myConnection5.Close();
                }
            }

        }
    }
}
