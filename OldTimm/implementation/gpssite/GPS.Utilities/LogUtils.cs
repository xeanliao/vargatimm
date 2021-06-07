using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using log4net.Config;

namespace GPS.Utilities
{
    public class LogUtils
    {
        private static ILog mLog = LogManager.GetLogger(typeof(LogUtils));

        public static void Error(string message, Exception ex)
        {
            log4net.Config.XmlConfigurator.Configure();
            mLog.Error(message, ex);
        }

        public static void Warn(string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            mLog.Warn(message);
        }

        public static void Info(string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            mLog.Info(message);
        }

        public static void Debug(string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            mLog.Debug(message);
        }
    }
}
