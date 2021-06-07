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
using WHYTAlgorithmService.Geo;
using System.Configuration;

namespace WHYTAlgorithmService.Service
{
    public partial class AlgorithmService : ServiceBase
    {
        /// <summary>
        /// Member definition
        /// </summary>
        ServiceHost hostGeofencing = null;

        UpdateDNDAreasMonitor updateDNDAreasMonitor = null;

 
        public AlgorithmService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
           Type serviceGeofencingType = typeof(Geofencing);
           hostGeofencing = new ServiceHost(serviceGeofencingType);


           hostGeofencing.Open();

           //DNDAreaStore.Instance.GetAllDNDAreas();

           //int minutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);
           //long nInterval = minutes * 60 * 8;
           long nInterval = 1000 * 3600 * 8;
           updateDNDAreasMonitor = new UpdateDNDAreasMonitor(nInterval);
           updateDNDAreasMonitor.Start();  

        }

        protected override void OnStop()
        {
            if (hostGeofencing != null)
                hostGeofencing.Close();

       }


    }
}
