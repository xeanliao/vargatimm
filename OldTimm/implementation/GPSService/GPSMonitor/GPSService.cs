using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.ServiceModel;
using GTUService.TIMM;
using System.Configuration;

namespace TIMM.GPSWebService
{
    public partial class GPSService : ServiceBase
    {
        /// <summary>
        /// Member definition
        /// </summary>
        ServiceHost hostQuery = null;
        ServiceHost hostUpdate = null;
        ServiceHost hostPolicy = null;

        GTULisTMonitor gtuListMonitor = null;
 
        public GPSService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Type serviceQueryType = typeof(GTUQueryService);
            hostQuery = new ServiceHost(serviceQueryType);

            Type serviceUpdateType = typeof(GTUUpdateService);
            hostUpdate = new ServiceHost(serviceUpdateType);

            Type servicePolicy = typeof(Policy);
            hostPolicy = new ServiceHost(servicePolicy);

            hostQuery.Open();
            hostUpdate.Open();
            hostPolicy.Open();

            /// TODO: the interval should be configurable, it should match the 
            /// GTU fixing reporting interval (2 times of GTU fixing reporting interval)
            /// Hard code 10 minute since the current GUP is set to  5 mins.
            TaskStore.Instance.getAviableTasks();
            TaskStore.Instance.getNdIds(-1);
            int minutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);
            long nInterval = minutes * 60 * 1000;
            gtuListMonitor = new GTULisTMonitor(nInterval);
            gtuListMonitor.Start();            
        }

        protected override void OnStop()
        {
             if (hostQuery != null)
                 hostQuery.Close();
             if (hostUpdate != null)
                 hostUpdate.Close();
             if (hostPolicy != null)
                 hostPolicy.Close();

       }


    }
}
