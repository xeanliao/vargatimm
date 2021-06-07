using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Quartz;
using System.IO;
using Common.Logging;
using Quartz.Simpl;
using GPS.DomainLayer.Area;
using TIMM.Jobs.Common;
using System.Configuration;
using OpenSmtp.Mail;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using GPS.DomainLayer.ActiveRecordDatabase;

namespace TIMM.Jobs.NonDeliverableAddresses
{
    public class ParseNonDeliverableAddressesJob : IJob
    {
        private AutoResetEvent m_ResetEvent = new AutoResetEvent(true);
        private static readonly ILog m_Logger = LogManager.GetLogger(typeof(ParseNonDeliverableAddressesJob));
        private long m_QueryCount;
        private ThreadSafeList<ResultAddress> m_Result = new ThreadSafeList<ResultAddress>();

        static ParseNonDeliverableAddressesJob()
        {
            var config = ActiveRecordSectionHandler.Instance;
            ActiveRecordStarter.Initialize(config, typeof(ScheduleTask), typeof(AssistantDatabase));
        }

        public void Execute(IJobExecutionContext context)
        {
            string filePath = context.JobDetail.JobDataMap.GetString("FilePath");
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                
                string jobName = Path.GetFileName(filePath);

                ScheduleTask task;
                task = ScheduleTask.Find(new Guid(jobName));
                if (task != null)
                {
                    task.Status = "S";
                    task.StartDate = DateTime.Now;
                    task.UpdateAndFlush();
                }

                m_Logger.Debug(string.Format("job start for {0}", jobName));
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    List<string> addresses = new List<string>();
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            addresses.Add(reader.ReadLine());
                        }
                    }

                    SimpleThreadPool pool = new SimpleThreadPool(10, ThreadPriority.BelowNormal);

                    pool.MakeThreadsDaemons = true;
                    pool.Initialize();

                    foreach (var address in addresses)
                    {
                        if (!string.IsNullOrWhiteSpace(address))
                        {
                            Interlocked.Increment(ref m_QueryCount);
                            SingleParseWorker worker = new SingleParseWorker(jobName, address.Trim(), QueryThreadCompeleted);
                            pool.RunInThread(worker);
                            m_ResetEvent.Reset();
                        }
                    }

                    m_ResetEvent.WaitOne();

                    int? userId = null;
                    
                    if (task != null)
                    {
                        userId = task.InUser;
                        task.EndDate = DateTime.Now;
                        task.Status = "C";
                        task.UpdateAndFlush();
                    }
                    Clear(filePath);

                    if (userId.HasValue)
                    {
                        using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                        {
                            var user = ws.Repositories.UserRepository.GetUser(userId.Value);
                            if (!string.IsNullOrWhiteSpace(user.Email))
                            {
                                SendMail(user.FullName, user.Email);
                            }
                        }
                        
                    }
                }

                m_Logger.Debug(string.Format("Job compeleted {0}", jobName));
            }
        }

        private void Clear(string fileName)
        {
            File.Delete(fileName);
            File.Delete(string.Format("{0}.ok", fileName));
        }

        private void SendMail(string realName, string email)
        {
            m_Logger.Debug("Begin send mail");
            var result = m_Result.CopyList();
            string successMailTemplate = ConfigurationManager.AppSettings["ParseNDAddressSuccessMailTemplate"];
            string faildMailTemplate = ConfigurationManager.AppSettings["ParseNDAddressFailedMailTemplate"];
            StringBuilder successMessage = new StringBuilder();
            StringBuilder failedMessage = new StringBuilder();
            bool haveFaildAddress = false;
            foreach (var item in result)
            {
                if (!item.IsSuccess)
                {
                    failedMessage.AppendLine(string.Format("  {0}, {1}", item.Street, item.ZipCode));
                    haveFaildAddress = true;
                }
                else
                {
                    successMessage.AppendLine(string.Format("  {0}, {1}", item.Street, item.ZipCode));
                }

            }
            string smtpBody = string.Empty;
            if (haveFaildAddress)
            {
                using (StreamReader reader = new StreamReader(faildMailTemplate))
                {
                    smtpBody = string.Format(reader.ReadToEnd(), realName, failedMessage.ToString());
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(successMailTemplate))
                {
                    smtpBody = string.Format(reader.ReadToEnd(), realName, successMessage.ToString());
                }
            }
            string host = ConfigurationManager.AppSettings["SmtpHost"];
            string user = ConfigurationManager.AppSettings["SmtpUser"];
            string password = ConfigurationManager.AppSettings["SmtpPassword"];
            string port = ConfigurationManager.AppSettings["SmtpPort"];
            string form = ConfigurationManager.AppSettings["ParseNDAddressMailFrom"];
            string subject = ConfigurationManager.AppSettings["ParseNDAddressMailSubject"];
            
            int smtpPort;
            Smtp sender = null;
            if (int.TryParse(port, out smtpPort))
            {
                sender = new Smtp(host, user, password, smtpPort);
            }
            else
            {
                sender = new Smtp(host, user, password);
            }

            sender.SendMail(form, email, subject, smtpBody);
            m_Logger.Debug(string.Format("send mail success to {0}", email));
        }

        private void QueryThreadCompeleted(object sender, ResultAddressEventArgs e)
        {
            m_Result.Add(e.ResultInfo);
            Interlocked.Decrement(ref m_QueryCount);
            if(Interlocked.Read(ref m_QueryCount) <= 0)
            {
                m_ResetEvent.Set();
            }
            
        }

        private class SingleParseWorker : IThreadRunnable
        {
            private string m_JobName;
            private string m_Address;

            public EventHandler<ResultAddressEventArgs> QueryCompleted;

            public SingleParseWorker(string jobName, string address, EventHandler<ResultAddressEventArgs> queryCompleted)
            {
                m_Address = address;
                m_JobName = jobName;
                QueryCompleted += queryCompleted;
            }

            public void Run()
            {
                ResultAddress ret = new ResultAddress();
                try
                {
                    m_Logger.Debug(string.Format("Job {0} query geo service for {1}", m_JobName, m_Address));
                    ExcelNdAddressRecord record = new ExcelNdAddressRecord(m_Address);
                    
                    MapNdAddress address = MapAreaManager.AddNonDeliverableAddress(record.Street, record.ZipCode, record.Geofence, record.Description);
                    if (address != null)
                    {
                        ret.IsSuccess = true;
                        ret.Id = address.Id;
                        ret.Street = address.Street;
                        ret.ZipCode = address.ZipCode;
                        ret.Geofence = address.Geofence;
                        ret.Latitude = address.Latitude;
                        ret.Longitude = address.Longitude;
                        ret.Description = address.Description;
                        double[][] locations = new double[address.Locations.Count][];
                        for (int i = 0; i < address.Locations.Count; i++)
                        {
                            locations[i] = new double[] { address.Locations[i].Latitude, address.Locations[i].Longitude };
                        }
                        ret.Locations = locations;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Street = record.Street;
                        ret.ZipCode = record.ZipCode;
                        ret.Geofence = record.Geofence;
                        ret.Description = record.Description;
                    }
                }
                finally
                {
                    if (QueryCompleted != null)
                    {
                        QueryCompleted(this, new ResultAddressEventArgs(ret));
                    }
                }
            }

        }
    }
}
