using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using WHYTAlgorithmService.Service;

namespace TIMM.GPSWebService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
		    { 
			    new AlgorithmService() 
		    };
            if (Environment.UserInteractive)
            {
                Type type = typeof(ServiceBase);
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(System.Console.Out, "consoleListener"));
                MethodInfo method = type.GetMethod("OnStart", flags);
			    foreach(ServiceBase service in ServicesToRun)
			    {
                    method.Invoke(service, new object[] { args });
			    }
                Console.WriteLine("Press any key to exit");
                Console.Read();

                foreach (ServiceBase service in ServicesToRun)
                {
                    service.Stop();
                }
            }
            else
            {
                ServiceBase.Run(ServicesToRun);
            }
            System.Diagnostics.Trace.Flush();
        }
    }
}
