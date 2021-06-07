using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Listener;
using System.Collections;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Collections.Concurrent;

namespace TIMM.Jobs
{
    public class FileListener : TriggerListenerSupport  
    {
        private static ConcurrentQueue<string> s_ExistFiles = new ConcurrentQueue<string>();
        private string m_Name;
        private FileSystemWatcher watcher;

        public FileListener(string name)
        {
            m_Name = name;
            string folder = ConfigurationManager.AppSettings["ParseNDAddressFolder"];
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new Exception("Monitor folder path missing. please check the config file appsettings key=ParseNDAddressFolder.");
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string[] existFiles = Directory.GetFiles(folder, "*.ok");
            foreach (var file in existFiles)
            {
                s_ExistFiles.Enqueue(file.Replace(".ok", ""));
            }

            watcher = new FileSystemWatcher(folder, "*.ok");
            watcher.Created += new FileSystemEventHandler(File_Created);
            watcher.EnableRaisingEvents = true;

            
        }

        void File_Created(object sender, FileSystemEventArgs e)
        {
            s_ExistFiles.Enqueue(e.FullPath.Replace(".ok", ""));
        }

        public override bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context)
        {
            string path;
            if (s_ExistFiles.TryDequeue(out path))
            {
                context.JobDetail.JobDataMap.Add("FilePath", path);
                return false;
            }
            return true;
        }

        public override string Name
        {
            get { return m_Name; }
        }
    }
}
