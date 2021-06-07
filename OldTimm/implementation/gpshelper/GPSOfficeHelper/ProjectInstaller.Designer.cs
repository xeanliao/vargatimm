namespace GPSOfficeHelper
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GPSOfficeHelperProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.GPSOfficeHelperInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // GPSOfficeHelperProcessInstaller
            // 
            this.GPSOfficeHelperProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.GPSOfficeHelperProcessInstaller.Password = null;
            this.GPSOfficeHelperProcessInstaller.Username = null;
            // 
            // GPSOfficeHelperInstaller
            // 
            this.GPSOfficeHelperInstaller.ServiceName = "GPSOfficeHelper2";
            this.GPSOfficeHelperInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.GPSOfficeHelperProcessInstaller,
            this.GPSOfficeHelperInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller GPSOfficeHelperProcessInstaller;
        private System.ServiceProcess.ServiceInstaller GPSOfficeHelperInstaller;
    }
}