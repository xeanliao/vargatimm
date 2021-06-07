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

namespace GPSListener.TIMM
{
    public partial class GPSListenerService : ServiceBase
    {
        /// <summary>
        /// Member definition
        /// </summary>
        ServiceHost hostQuery = null;
        ServiceHost hostPolicy = null;

        static AsynchronousSocketListener oListener = new GTUListener();

        public GPSListenerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            oListener.Start();
        }

        protected override void OnStop()
        {
             oListener.Stop();

       }


    }
}
