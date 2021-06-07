using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

namespace GPS.Utilities
{
    public class DBLog
    { 
    
        public static void LogInfo(string info)
        {
            WriteLog("info", info);
        }

        public static void LogWaring(string warning)
        {
            WriteLog("warning", warning);
        }

        public static void LogError(string error)
        {
            WriteLog("error", error);
        }

        private static void WriteLog(string sType, string sInfo)
        {
            try
            {
                SqlParameter[] paramList = new SqlParameter[]
                {
                    new SqlParameter("@logType", sType),
                    new SqlParameter("@info", sInfo)
                };

                DBUtil.ExecuteNonQuery("AddLog", paramList);
            }
            catch (Exception)
            { 
                // no where to go
            }
        }
    }
}