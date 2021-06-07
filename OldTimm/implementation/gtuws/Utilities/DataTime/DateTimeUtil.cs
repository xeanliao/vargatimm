using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.Utilities.DateTimeUtil
{
    public static class DateTimeUtil
    {
        
        public static String GetSendTime()
        {
            DateTime dt = DateTime.Now;
            StringBuilder dtStr = new StringBuilder();
            dtStr.Append(dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString());
            return dtStr.ToString();
        }

        public static DateTime ConvertStrToDate(String dateStr)
        {
            var socketLog = new GTU.Utilities.Logging.Logging();
            try
            {
                socketLog.WriteLog(String.Format("datestr = {0}", dateStr));
                String date = dateStr.Substring(0, 4) + "-" + dateStr.Substring(4, 2) + "-" + dateStr.Substring(6, 2) + " " + dateStr.Substring(8, 2) + ":" + dateStr.Substring(10, 2) + ":" + dateStr.Substring(12, 2);
                socketLog.WriteLog(String.Format("date = {0}", date));
                return Convert.ToDateTime(date);
            }
            catch (Exception ex)
            {
                socketLog.WriteLog(String.Format("ConvertStrToDate{0}", ex));
                return DateTime.Now;
            }
            
        }

        public static String ConvertDateToStr(DateTime date)
        {
            var socketLog = new GTU.Utilities.Logging.Logging();
            try
            {
                String dateStr = date.Year.ToString() + date.ToString("MM") + date.ToString("dd") + date.ToString("hh") + date.ToString("mm") + date.ToString("ss");
                socketLog.WriteLog(String.Format("datetimestr = {0}", dateStr));
                return dateStr;
            }
            catch (Exception ex)
            {
                socketLog.WriteLog(String.Format("ConvertDateToStr{0}", ex));
                return "";
            }
        }

    }
}
