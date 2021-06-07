using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;
using GPS.Tool;
using GPS.Tool.Data;

namespace GPS.Tool.Import
{
    public class ReadArea
    {
        private static DataTable ReadBdfFile(string FolderPath, string sql)
        {
            DbfReader.FolderPath = FolderPath;
            DataTable dt = DbfReader.ExecuteSql(sql);
            return dt;
        }

        public static void ReadFile(string shpName, 
            string dbfName, 
            string folderPath, 
            ref ShpReader shpReader,
            ref DataTable dt)
        {
            shpReader = new ShpReader();
            shpReader.readShapeFile(folderPath, shpName);
            string sql = "SELECT * FROM {0}{1}";
            sql = string.Format(sql, folderPath, dbfName);
            dt = ReadBdfFile(folderPath, sql);
        }

        public static void ReadFile(
            string dbfName,
            string folderPath,
            ref DataTable dt)
        {
            string sql = "SELECT * FROM {0}{1}";
            sql = string.Format(sql, folderPath, dbfName);
            dt = ReadBdfFile(folderPath, sql);
        }
    }
}
