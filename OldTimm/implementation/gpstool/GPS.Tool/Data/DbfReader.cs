using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Web;

namespace GPS.Tool.Data
{
    /// <summary>
    /// Read DBF file under the web Site
    /// </summary>
    public sealed class DbfReader
    {
        public static string FolderPath = string.Empty;
        static OdbcConnection oConn = null;
        
        public DbfReader() { }

        /// <summary>
        /// Excute DataTable
        /// </summary>
        /// <param name="tableName">table name or dbf file name</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(string tableName)
        {
            oConn = ODBCHelper.CreateDBaseConnection(FolderPath);

            return ODBCHelper.ExecuteDataTable(oConn, FolderPath, tableName); ;
        }

        /// <summary>
        /// Excute DataTable
        /// </summary>
        /// <param name="sql">sql query</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteSql(string sql)
        {
            oConn = ODBCHelper.CreateDBaseConnection(FolderPath);

            return ODBCHelper.ExecuteDataTable(oConn, sql); ;
        }
    }
}
