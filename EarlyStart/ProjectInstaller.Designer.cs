namespace EarlyStart
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
            this.LogInstaller = new System.Diagnostics.EventLogInstaller();
            this.ProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // LogInstaller
            // 
            this.LogInstaller.CategoryCount = 0;
            this.LogInstaller.CategoryResourceFile = null;
            this.LogInstaller.Log = "EarlyStart Service";
            this.LogInstaller.MessageResourceFile = null;
            this.LogInstaller.ParameterResourceFile = null;
            this.LogInstaller.Source = "EarlyStartService";
            // 
            // ProcessInstaller
            // 
            this.ProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ProcessInstaller.Password = null;
            this.ProcessInstaller.Username = null;
            // 
            // ServiceInstaller
            // 
            this.ServiceInstaller.Description = "Launches programs before Windows Explorer when opening a session.";
            this.ServiceInstaller.DisplayName = "EarlyStart launch service";
            this.ServiceInstaller.ServiceName = "EarlyStart";
            this.ServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.LogInstaller,
            this.ProcessInstaller,
            this.ServiceInstaller});

        }

        #endregion

        private System.Diagnostics.EventLogInstaller LogInstaller;
        private System.ServiceProcess.ServiceProcessInstaller ProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ServiceInstaller;
    }
}