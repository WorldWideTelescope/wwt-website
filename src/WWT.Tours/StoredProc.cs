#nullable disable

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WWT.Tours
{
    public class StoredProc
    {
        private SqlCommand cmd;
        public StoredProc(string strQuery, string connectionString)
        {
            cmd = new SqlCommand(strQuery, new SqlConnection(connectionString));
            cmd.CommandTimeout = 60;
            cmd.Parameters.Add(
                new SqlParameter("ReturnValue",
                SqlDbType.Int,
                /* int size */ 4,
                ParameterDirection.ReturnValue,
                /* bool isNullable */ false,
                /* byte precision */ 0,
                /* byte scale */ 0,
                /* string srcColumn */ string.Empty,
                DataRowVersion.Default,
                /* value */ null
                )
                );
            cmd.Connection.Open();
        }

        ~StoredProc()
        {
            this.Dispose();
        }

        public StoredProc(string spName, SqlConnection conn)
        {
            cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(
            new SqlParameter("ReturnValue",
            SqlDbType.Int,
                /* int size */ 4,
            ParameterDirection.ReturnValue,
                /* bool isNullable */ false,
                /* byte precision */ 0,
                /* byte scale */ 0,
                /* string srcColumn */ string.Empty,
            DataRowVersion.Default,
                /* value */ null
            ));

            cmd.Connection.Open();
        }

        public void Dispose()
        {
            if (cmd != null)
            {
                try
                {
                    SqlConnection connection = cmd.Connection;
                    Debug.Assert(connection != null);
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                    cmd.Dispose();
                    cmd = null;
                    GC.Collect();
                }
                catch
                {
                }
            }
        }

        public int RunQuery(DataTable dataTable)
        {
            if (cmd == null)
                throw new ObjectDisposedException(GetType().FullName);
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.SelectCommand = cmd;
            dataAdapter.Fill(dataTable);

            return (int)dataAdapter.SelectCommand.Parameters["ReturnValue"].Value;

        }

        public SqlDataReader RunGetDataReader()
        {
            if (cmd == null)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            return cmd.ExecuteReader();
        }

        public int RunQuery(DataSet dataSet)
        {
            if (cmd == null)
                throw new ObjectDisposedException(GetType().FullName);
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.SelectCommand = cmd;
            dataAdapter.Fill(dataSet);

            return (int)dataAdapter.SelectCommand.Parameters["ReturnValue"].Value;

        }

        ///	<summary>
        /// Execute this stored procedure.
        ///	<returns>Int32 value returned by the stored procedure</returns>
        /// </summary>

        public int Run()
        {
            if (cmd == null)
                throw new ObjectDisposedException(GetType().FullName);

            cmd.ExecuteNonQuery();
            return (int)cmd.Parameters["ReturnValue"].Value;
        }

        /// <summary>
        ///	Fill a DataSet with the result of executing this stored procedure.
        /// <param name='dataTable'>
        /// DataTable filled with the results of executing the stored procedure</param>
        /// <returns>
        /// Int32 value returned by the stored procedure</returns>
        /// </summary>

        public int Run(DataTable dataTable)
        {
            if (cmd == null)
                throw new ObjectDisposedException(GetType().FullName);

            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            dataAdapter.SelectCommand = cmd;
            dataAdapter.Fill(dataTable);

            return (int)dataAdapter.SelectCommand.Parameters["ReturnValue"].Value;
        }
    }
}
