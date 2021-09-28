using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace SAReportsAddin
{
    public partial class ThisAddIn
    {
        Outlook.Inspectors inspectors;
        private UserControl1 taskPaneControl1;
        private Microsoft.Office.Tools.CustomTaskPane taskPaneValue;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //inspectors = this.Application.Inspectors;
            //inspectors.NewInspector +=
            //new Microsoft.Office.Interop.Outlook.InspectorsEvents_NewInspectorEventHandler(Inspectors_NewInspector);

            //taskPaneControl1 = new UserControl1();
            //taskPaneValue = this.CustomTaskPanes.Add(
            //    taskPaneControl1, "MyCustomTaskPane");
            //taskPaneValue.VisibleChanged +=
            //    new EventHandler(taskPaneValue_VisibleChanged);
            //taskPaneValue.Visible = true;
            Globals.Ribbons.Ribbon1.Addin = this;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        void Inspectors_NewInspector(Microsoft.Office.Interop.Outlook.Inspector Inspector)
        {
            Outlook.MailItem mailItem = Inspector.CurrentItem as Outlook.MailItem;
            if (mailItem != null)
            {
                if (mailItem.EntryID == null)
                {
                    mailItem.Subject = "This text was added by using code (Test)";
                    mailItem.Body = "This text was added by using code";
                }

            }
        }

        private void taskPaneValue_VisibleChanged(object sender, System.EventArgs e)
        {
            //Globals.Ribbons.ManageTaskPaneRibbon.toggleButton1.Checked =
            //    taskPaneValue.Visible;
        }

        public Microsoft.Office.Tools.CustomTaskPane TaskPane
        {
            get
            {
                return taskPaneValue;
            }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
