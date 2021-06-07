using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using GPS.Tool.Data;
using System.Data.Common;
using System.IO;

namespace GPS.Tool.Import.Reader
{
    class DbfReader
    {
        string _filePath;
        OdbcConnection conn;
        public void Open()
        {
            conn.Open();
        }
        public DbfReader(string filePath)
        {
            _filePath = filePath;
            conn = ODBCHelper.CreateDBaseConnection((new FileInfo(filePath)).DirectoryName + "\\");
        }
        public DbDataReader Read()
        {
            OdbcCommand cmd = new OdbcCommand(string.Format("SELECT * FROM {0}", _filePath), conn);
            return cmd.ExecuteReader();
        }

        public DbDataReader ExecuteReader()
        {
            OdbcCommand cmd = new OdbcCommand(string.Format("SELECT * FROM {0}", _filePath), conn);
            return cmd.ExecuteReader();
        }

        public void Close()
        {
            conn.Close();
        }
    }
}
