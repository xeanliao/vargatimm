using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace GPS.Utilities
{
    public class DBUtil
    {
        public static string ConnectionString = null;   // this is a static variable
        private string connectionString = null;  // this is a instance variable, use can new an instance of DBUtil and pass in connection string

        internal static SqlConnection GetSqlConnection()
        {
            if (ConnectionString == null)
                ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];

            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        private SqlConnection getSqlConnection()
        {
            SqlConnection conn = new SqlConnection(this.connectionString);
            conn.Open();
            return conn;
        }

        public DBUtil()
        {
            this.connectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
        }

        public DBUtil(string sConnectionString)
        {
            this.connectionString = sConnectionString;
        }

        /// <summary>
        /// Execute SQL Query without return-value
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sCommand)
        {
            SqlConnection conn = GetSqlConnection();

            try
            {
                SqlCommand comm = new SqlCommand(sCommand, conn);
                comm.CommandTimeout = conn.ConnectionTimeout;
                return comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }


        /// <summary>
        /// Execute SQL Stored procedure without return-value
        /// </summary>
        /// <param name="sProcedureName"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sProcedureName, SqlParameter[] paramList)
        {
            SqlConnection conn = GetSqlConnection();

            try
            {
                SqlCommand comm = new SqlCommand(sProcedureName, conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = conn.ConnectionTimeout;

                if (paramList != null)
                {
                    foreach (SqlParameter p in paramList)
                        comm.Parameters.Add(p);
                }

                return comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public static object ExecuteScalar(string sQuery)
        {
            return new DBUtil().executeScalar(sQuery);
        }

        public object executeScalar(string sQuery)
        {
            using (SqlConnection conn = this.getSqlConnection())
            {
                SqlCommand comm = new SqlCommand(sQuery, conn);
                comm.CommandTimeout = conn.ConnectionTimeout;
                return comm.ExecuteScalar();
            }
        }

        public static object ExecuteScalar(string sProcedureName, SqlParameter[] paramList)
        {
            SqlConnection conn = GetSqlConnection();

            try
            {
                SqlCommand comm = new SqlCommand(sProcedureName, conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = conn.ConnectionTimeout;

                if (paramList != null)
                {
                    foreach (SqlParameter p in paramList)
                        comm.Parameters.Add(p);
                }

                return comm.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataTable ExecuteTable(string sQuery)
        {
            return new DBUtil().executeTable(sQuery);
        }

        public DataTable executeTable(string sQuery)
        {
            using (SqlConnection conn = this.getSqlConnection())
            {
                DataTable dtData = new DataTable("table1");
                SqlDataAdapter adapter = new SqlDataAdapter(sQuery, conn);
                adapter.SelectCommand.CommandTimeout = conn.ConnectionTimeout;

                adapter.Fill(dtData);
                return dtData;
            }
        }

        public static DataTable ExecuteTable(string sProcedureName, SqlParameter[] paramList)
        {
            SqlConnection conn = GetSqlConnection();

            try
            {
                SqlCommand comm = new SqlCommand(sProcedureName, conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = conn.ConnectionTimeout;

                if (paramList != null)
                {
                    foreach (SqlParameter p in paramList)
                        comm.Parameters.Add(p);
                }

                DataTable dtData = new DataTable("table1");
                SqlDataAdapter adapter = new SqlDataAdapter(comm);

                adapter.Fill(dtData);
                return dtData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        // return a DataSet
        public static DataSet ExecuteDataSet(string sProcedureName, SqlParameter[] paramList)
        {
            SqlConnection conn = GetSqlConnection();

            try
            {
                SqlCommand comm = new SqlCommand(sProcedureName, conn);
                comm.CommandType = CommandType.StoredProcedure;

                if (paramList != null)
                {
                    foreach (SqlParameter p in paramList)
                        comm.Parameters.Add(p);
                }

                DataSet dsData = new DataSet("DataSet1");
                SqlDataAdapter adapter = new SqlDataAdapter(comm);

                adapter.Fill(dsData);
                return dsData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public static void UpdateTable(string tableName, DataTable dtData)
        {
            using (SqlConnection conn = GetSqlConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter("select * from " + tableName, conn);
                adapter.SelectCommand.CommandTimeout = conn.ConnectionTimeout;

                SqlCommandBuilder commBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(dtData);
            }
        }

        public static void UpdateTableByProcedure(DataTable dtData, string procedure)
        {
            new DBUtil().updateTableByProcedure(dtData, procedure);
        }

        public void updateTableByProcedure(DataTable dtData, string procedure)
        {
            using (SqlConnection conn = getSqlConnection())
            {
                SqlCommand comm = new SqlCommand(procedure, conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = conn.ConnectionTimeout;

                int iColumn = dtData.Columns.Count;
                foreach (DataRow row in dtData.Rows)
                {
                    if (comm.Parameters.Count == 0)
                    {
                        for (int i = 0; i < iColumn; i++)
                            comm.Parameters.Add(new SqlParameter("@" + dtData.Columns[i].ColumnName, row[i]));
                    }
                    else
                    {
                        for (int i = 0; i < iColumn; i++)
                            comm.Parameters["@" + dtData.Columns[i].ColumnName].Value = row[i];
                    }
                    comm.ExecuteNonQuery();
                }
            }
        }


        public static string TableToString(DataTable dtData)
        {
            // get column Names
            StringBuilder sbText = new StringBuilder();
            foreach (DataColumn col in dtData.Columns)
                sbText.Append(col.ColumnName + "\t");
            sbText.AppendLine();

            int iColumnCount = dtData.Columns.Count;
            foreach (DataRow row in dtData.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                for (int i = 0; i < iColumnCount; i++)
                {
                    sbText.Append(Convert.ToString(row[i]).Replace("\t", " ").Replace("\n", "").Replace("\r", "") + "\t");
                }
                sbText.AppendLine();
            }

            return sbText.ToString();
        }

        public static void TableToCSV(DataTable dtData, string file)
        {
            StreamWriter writer = new StreamWriter(file);

            // get column Names
            foreach (DataColumn col in dtData.Columns)
                writer.Write(col.ColumnName + ",");
            writer.WriteLine();

            int iColumnCount = dtData.Columns.Count;
            foreach (DataRow row in dtData.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                writer.Write("\"" + Convert.ToString(row[0]).Replace("\"", "\"\"").Replace("\t", " ").Replace("\n", "").Replace("\r", "") + "\"");
                for (int i = 1; i < iColumnCount; i++)
                {
                    writer.Write(",\"" + Convert.ToString(row[i]).Replace("\"", "\"\"").Replace("\t", " ").Replace("\n", "").Replace("\r", "") + "\"");
                }
                writer.WriteLine();
            }

            writer.Flush();
            writer.Close();
        }


        public static string TableToHtml(DataTable dtData)
        {
            // get column Names
            StringBuilder sbText = new StringBuilder();
            sbText.Append("<table>");
            sbText.Append("<tr>");
            foreach (DataColumn col in dtData.Columns)
                sbText.Append("<td>" + col.ColumnName + "</td>");
            sbText.Append("</tr>");

            int iColumnCount = dtData.Columns.Count;
            foreach (DataRow row in dtData.Rows)
            {
                sbText.Append("<tr>");
                for (int i = 0; i < iColumnCount; i++)
                    sbText.Append("<td>" + Convert.ToString(row[i]) + "</td>");
                sbText.Append("</tr>");
            }

            sbText.Append("</table>");
            return sbText.ToString();
        }


        public static string TableToXml(DataTable dtData)
        {
            if (dtData.TableName == "")
                dtData.TableName = "X";

            // get column Names
            StringBuilder sbText = new StringBuilder();
            StringWriter writer = new StringWriter(sbText);

            dtData.WriteXml(writer);
            return sbText.ToString();
        }

        public static DataTable XMLToTable(string sXML)
        {
            try
            {
                System.IO.StringReader reader = new System.IO.StringReader(sXML);
                DataSet ds = new DataSet();
                ds.ReadXml(reader);
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

    }
}
