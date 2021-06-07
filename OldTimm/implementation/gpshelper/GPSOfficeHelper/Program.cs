using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.ServiceModel;

namespace GPSOfficeHelper
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new GPSOfficeHelper() 
            //};
            //ServiceBase.Run(ServicesToRun);
            Console.WriteLine("Starting ExcelHelper WCF server.");
            ServiceHost serviceHost = new ServiceHost(typeof(ExcelHelper));
            
            serviceHost.Open();
            Console.WriteLine("Open ExcelHelper WCF server.");
            while (true)
            {
                Console.WriteLine("Input <Exit> to terminate ExcelHelper WCF server.");
                string key = Console.ReadLine();
                if (key.ToLower().Trim() == "exit")
                {
                    break;
                }
            }
            Console.WriteLine("Closing ExcelHelper WCF server.");
            serviceHost.Close();
            Console.WriteLine("Close ExcelHelper WCF server.");
        }
    }
}
