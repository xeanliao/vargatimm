using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;

namespace GPS.Tool.Data
{
    public class ODBCHelper
    {
        /// <summary>
        /// Create OdbcConnection for dBase (dbf)
        /// </summary>
        /// <param name="FolderPath">the folder path</param>
        /// <returns>OdbcConnection</returns>
        public static OdbcConnection CreateDBaseConnection(string FolderPath)
        {
            OdbcConnection oConn = new OdbcConnection();
            oConn.ConnectionString =
                //@"Driver={Microsoft dBase Driver (*.dbf)};SourceType=DBF;SourceDB=" +
                //FolderPath +
                //";Exclusive=No; Collate=Machine;NULL=NO;DELETED=NO;BACKGROUNDFETCH=NO;";

            "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;SourceDB=" + FolderPath;


            return oConn;
        }

        /// <summary>
        /// Create OdbcConnection for csv, txt
        /// </summary>
        /// <param name="FolderPath">the folder path</param>
        /// <returns>OdbcConnection</returns>
        public static OdbcConnection CreateCSVConnection(string FolderPath)
        {
            OdbcConnection oConn = new OdbcConnection();
            oConn.ConnectionString = 
                @"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + 
                FolderPath + ";";

            return oConn;
        }

        /// <summary>
        /// Create command for connection
        /// </summary>
        /// <param name="fileFullPath">the file full path</param>
        /// <returns>OdbcCommand</returns>
        public static OdbcCommand CreateCommand(OdbcConnection pConn, string fileFullPath)
        {
            OdbcCommand oComm = pConn.CreateCommand();
            oComm.CommandText = @"SELECT * FROM " + fileFullPath;

            return oComm;
        }

        /// <summary>
        /// Create command for connection
        /// </summary>
        /// <param name="sql">sql query</param>
        /// <returns>OdbcCommand</returns>
        public static OdbcCommand CreateSqlCommand(OdbcConnection pConn, string sql)
        {
            OdbcCommand oComm = pConn.CreateCommand();
            oComm.CommandText = sql;

            return oComm;
        }

        /// <summary>
        /// Excute DataTable
        /// </summary>
        /// <param name="oConn">the OdbcConnection</param>
        /// <param name="folderName">the folder path</param>
        /// <param name="fileName">table name or file name</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(
            OdbcConnection oConn, 
            string folderName, 
            string fileName)
        {
            if (oConn == null)
                throw new ArgumentNullException("connection");
            if (String.IsNullOrEmpty(folderName) || String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("file");

            DataTable dt = new DataTable();
            using (OdbcCommand oComm = CreateCommand(oConn, folderName + fileName))
            {
                try
                {
                    if(oConn.State != ConnectionState.Open)
                        oConn.Open();
                    dt.Load(oComm.ExecuteReader());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    oConn.Close();
                }
            }

            return dt;
        }

        /// <summary>
        /// Excute DataTable
        /// </summary>
        /// <param name="oConn">the OdbcConnection</param>
        /// <param name="sql">the sql query</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(OdbcConnection oConn, string sql)
        {
            if (oConn == null)
                throw new ArgumentNullException("connection");
            if (String.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            DataTable dt = new DataTable();
            using (OdbcCommand oComm = CreateSqlCommand(oConn, sql))
            {
                try
                {
                    if (oConn.State != ConnectionState.Open)
                        oConn.Open();
                    dt.Load(oComm.ExecuteReader());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    oConn.Close();
                }
            }

            return dt;
        }

        /// <summary>
        /// Excute Command for get DataTable
        /// </summary>
        /// <param name="oConn">the OdbcConnection</param>
        /// <param name="sql">the sql query</param>
        /// <returns>DataTable</returns>
        //public static void ExecuteDataTable(
        //    OdbcConnection oConn, 
        //    string sql,
        //    DataTable dtCsv,
        //    DataTable dtBGS)
        //{
        //    using (OdbcCommand oComm = CreateSqlCommand(oConn, sql))
        //    {
        //        DataView dvCSV = null;
        //        try{
        //            // Execute the DataReader and access the data.
        //            OdbcDataReader reader = oComm.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                string filterExpression = "COUNTYNAME='{0}' AND TRK='{1}' AND BLK='{2}'";
        //                filterExpression = String.Format(filterExpression,
        //                    reader["COUNTYNAME"].ToString(),
        //                    reader["TRK"].ToString(),
        //                    reader["BLK"].ToString());

        //                dvCSV = new DataView(dtCsv);
        //                dvCSV.RowFilter = filterExpression;

        //                if (dvCSV.Count > 0)
        //                {
        //                    DataRow dr = dtBGS.NewRow();
        //                    dr["BGId"] = reader["BG06_D00_I"];
        //                    dr["Name"] = null;
        //                    dr["State"] = reader["STATE"]??null;
        //                    dr["County"] = reader["CountyID"]??null;
        //                    dr["Tract"] = reader["TRK"];
        //                    dr["BLKGroup"] = reader["BLK"];
        //                    dr["Total"] = dvCSV[0]["Total"];
        //                    dtBGS.Rows.Add(dr);
        //                }
        //            }
        //            // Call Close when done reading.
        //            reader.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            oComm.Connection.Close();
        //        }
        //    }
        //}

        /// <summary>
        /// Get OdbcDataReader
        /// </summary>
        /// <param name="oConn">the OdbcConnection</param>
        /// <param name="sql">the sql query</param>
        /// <returns>OdbcDataReader</returns>
        public static OdbcDataReader ExecuteDataReader(
            OdbcConnection oConn,
            string sql)
        {
            if (oConn == null)
                throw new ArgumentNullException("connection");
            if (String.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            OdbcDataReader reader = null;
            using (OdbcCommand oComm = CreateSqlCommand(oConn, sql))
            {
                try
                {
                    if (oConn.State != ConnectionState.Open)
                        oConn.Open();
                    // Execute the DataReader and access the data.
                    reader = oComm.ExecuteReader();
                    // Call Close when done reading.
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    oConn.Close();
                }
            }
            return reader;
        }
    }
}
