using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GPS.Tool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Control.CheckForIllegalCrossThreadCalls = false;
            //Application.Run(new frmSelectMapping());
            Application.Run(new ImportData());
        }
    }
}