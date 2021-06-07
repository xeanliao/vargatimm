using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TIMM.GPS.ControlCenter.Views;

namespace TIMM.GPS.ControlCenter
{
    public partial class App : Application
    {
        public static string MapKey { get; set; }

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.RootVisual = new MainPage();

            if (e.InitParams.ContainsKey("MapKey"))
            {
                App.MapKey = e.InitParams["MapKey"];
            }
            else
            {
                App.MapKey = "AtXXJaslHAB3VXs9lSXbsB9GjK55OJdKpU9lcpFtuA3BfNO-6dQFZu-pD8PIsJE-";
            }
        }

        private void Application_Exit(object sender, EventArgs e)
        {
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
#if DEBUG
            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                MessageBox.Show(e.ExceptionObject.ToString());
            });
            
            return;
#endif


            // NOTE: This will allow the application to continue running after an exception has been thrown
            // but not handled. 
            // For production applications this error handling should be replaced with something that will 
            // report the error to the website and stop the application.
            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                ReportErrorToDOM(e);
            });
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
