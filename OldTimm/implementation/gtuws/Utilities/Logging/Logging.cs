using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GTU.Utilities.Logging
{
    public class Logging
    {
        private static string _logPath;
        private static string _logif;

        public static object syncRoot = new object();


        public String LogPath { get { return _logPath; } set { _logPath = value; } }

        public String LogIf { get { return _logif; } set { _logif = value; } }

        public Logging()
        {
            _logif = System.Configuration.ConfigurationSettings.AppSettings["logif"].ToString();
            _logPath = System.Configuration.ConfigurationSettings.AppSettings["logPath"].ToString();
        }


        public void WriteLog(string text)
        {
            lock (syncRoot)
            {
                if (LogIf == "1")
                {
                    string path = LogPath;
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter m_streamWriter = new StreamWriter(fs);
                    m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                    m_streamWriter.WriteLine(DateTime.Now.ToString() + " : " + text + "\n");
                    m_streamWriter.Flush();
                    m_streamWriter.Close();
                    fs.Close();
                }
            }
        }
    }

    
}
