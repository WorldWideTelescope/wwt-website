namespace Microsoft.Research.EarthOnline.NotificationService
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
            this.NotificationServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.NotificationServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // NotificationServiceProcessInstaller
            // 
            this.NotificationServiceProcessInstaller.Password = null;
            this.NotificationServiceProcessInstaller.Username = null;
            // 
            // NotificationServiceInstaller
            // 
            this.NotificationServiceInstaller.Description = "Layerscape Notification Service";
            this.NotificationServiceInstaller.DisplayName = "Layerscape Notification Service";
            this.NotificationServiceInstaller.ServiceName = "NotificationService";
            this.NotificationServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.NotificationServiceProcessInstaller,
            this.NotificationServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller NotificationServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller NotificationServiceInstaller;
    }
}