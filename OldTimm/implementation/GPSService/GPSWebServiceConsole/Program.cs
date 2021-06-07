using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTUService.TIMM;
using System.ServiceModel;
using System.Configuration;

namespace GPSWebServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Type serviceQueryType = typeof(GTUQueryService);
            var hostQuery = new ServiceHost(serviceQueryType);

            Type serviceUpdateType = typeof(GTUUpdateService);
            var hostUpdate = new ServiceHost(serviceUpdateType);

            Type servicePolicy = typeof(Policy);
            var hostPolicy = new ServiceHost(servicePolicy);

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
            var gtuListMonitor = new GTULisTMonitor(nInterval);
            gtuListMonitor.Start();

            Console.ReadKey();
        }
    }
}
