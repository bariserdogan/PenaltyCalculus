using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenaltyCalculation.DataAccess
{
    /*
       The class for database connection.
        All operations are directed in one class. (open connection, close connection, fetch data vs.)      
    */

    public class DatabaseConnector
    {
        public string connString;
        private SqlConnection sqlConnection;

        public DatabaseConnector(string connectionString)
        {
            connString = connectionString;
        }

        public bool IsConnectionOpen
        {
            get
            {
                if (sqlConnection == null)
                    return false;
                return (sqlConnection.State == System.Data.ConnectionState.Open);
            }
        }

        public void OpenConnection()
        {
            if (sqlConnection == null)
            {
                sqlConnection = new SqlConnection(connString);
                sqlConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Close();
                sqlConnection = null;
            }
        }

        public IDbDataParameter CreateParameter(string paramName, object paramValue)
        {
            return new SqlParameter(paramName, paramValue);
        }

        public DataSet GetDataSet(string sqlQuery)
        {
            return GetDataSet(sqlQuery, null);
        }

        public DataSet GetDataSet(string sqlQuery, params IDbDataParameter[] parameters)
        {
            SqlDataAdapter da = new SqlDataAdapter(sqlQuery, connString);

            if (parameters != null)
            {
                foreach (SqlParameter pr in parameters)
                {
                    da.SelectCommand.Parameters.Add(pr);
                }
            }

            DataSet ds = new DataSet();

            da.Fill(ds, "list");
            da.SelectCommand.Parameters.Clear();
            da.Dispose();
            return ds;
        }
    }
}
