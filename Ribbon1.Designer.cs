namespace SAReportsAddin
{
    partial class Ribbon1 : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon1()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.SA_Reports = this.Factory.CreateRibbonTab();
            this.ReportsGroup = this.Factory.CreateRibbonGroup();
            this.button1 = this.Factory.CreateRibbonButton();
            this.Status = this.Factory.CreateRibbonLabel();
            this.SA_Reports.SuspendLayout();
            this.ReportsGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // SA_Reports
            // 
            this.SA_Reports.Groups.Add(this.ReportsGroup);
            this.SA_Reports.Label = "SA Reports";
            this.SA_Reports.Name = "SA_Reports";
            // 
            // ReportsGroup
            // 
            this.ReportsGroup.Items.Add(this.button1);
            this.ReportsGroup.Items.Add(this.Status);
            this.ReportsGroup.Label = "Reports";
            this.ReportsGroup.Name = "ReportsGroup";
            // 
            // button1
            // 
            this.button1.Image = global::SAReportsAddin.Properties.Resources.favicon;
            this.button1.Label = "Create Report";
            this.button1.Name = "button1";
            this.button1.ShowImage = true;
            this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Button1_Click);
            // 
            // Status
            // 
            this.Status.Label = " ";
            this.Status.Name = "Status";
            // 
            // Ribbon1
            // 
            this.Name = "Ribbon1";
            this.RibbonType = "Microsoft.Outlook.Explorer, Microsoft.Outlook.Mail.Read";
            this.Tabs.Add(this.SA_Reports);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
            this.SA_Reports.ResumeLayout(false);
            this.SA_Reports.PerformLayout();
            this.ReportsGroup.ResumeLayout(false);
            this.ReportsGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Microsoft.Office.Tools.Ribbon.RibbonTab SA_Reports;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ReportsGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel Status;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon1 Ribbon1
        {
            get { return this.GetRibbon<Ribbon1>(); }
        }
    }
}
