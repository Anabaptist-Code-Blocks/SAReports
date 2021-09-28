using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Outlook;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SAReportsAddin
{
    public partial class Ribbon1
    {
        public ThisAddIn Addin { get; set; }
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private string FolderPath = "";

        private void Button1_Click(object sender, RibbonControlEventArgs e)
        {
            Task.Run(() => {
                string progress = "Starting";
                try
                {
                    var stores = Globals.ThisAddIn.Application.GetNamespace("MAPI").Stores;
                    progress = "Got address stores";
                    var cnt = 0;
                    foreach (Store store in stores)
                    {
                        cnt++;
                        var folder = (Folder)store.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
                        progress = "Got folder " + cnt;
                        if (folder.FolderPath.Contains("@"))
                        {
                            progress = "Ready to search for reports";
                            FolderPath = folder.FolderPath;
                            SearchForReports(folder);
                            System.Diagnostics.Debug.WriteLine("Folder: " + folder.FolderPath);
                            return;
                        }
                    }
                    return;
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.Message + "\n\nError occured at: " + progress);
                    Status.Label = "Error!";
                }
            });
        }

        private string GetStringBetween(string input, string start, string end)
        {
            if (!input.Contains(start)) return "";
            var text = input.Substring(input.IndexOf(start) + start.Length);
            if (!text.Contains(end)) return "";
            return text.Substring(0, text.IndexOf(end));
        }

        private void SearchForReports(MAPIFolder folder)
        {
            Status.Label = "Searching Emails";
            var filter = "urn:schemas:mailheader:subject like '%Report%'" +
                 " and %thismonth(\"urn:schemas:httpmail:datereceived\")%";

            Addin.Application.AdvancedSearchComplete += Application_AdvancedSearchComplete;
            var search = Addin.Application.AdvancedSearch("'" + folder.FolderPath + "'", filter, true, "reports");
        }

        private void Application_AdvancedSearchComplete(Search SearchObject)
        {
            var items = new List<MailItem>();
            System.Diagnostics.Debug.WriteLine("Results: " + SearchObject.Results.Count);
            foreach (MailItem item in SearchObject.Results)
            {
                if (item.SenderEmailAddress.Contains("thesecurityappliance.com") && !item.Subject.Contains("Autofix") && item.Attachments.Count > 0)
                {
                    items.Add(item);
                    System.Diagnostics.Debug.WriteLine("Subject: " + item.Subject);
                }
            }
            System.Diagnostics.Debug.WriteLine("Count: " + items.Count);
            if (items.Count > 0) GenerateReport(items);
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void GenerateReport(List<MailItem> items)
        {
            Task.Run(() => {
                Status.Label = "Generating Report";
                var progress = "Generating Report";
                if (items == null) progress = "Items is null";
                try
                {
                    var summary_html = "<template id=\"report_summary\"><table class=\"table1\"><tr><th>Name</th><th>Active Devices</th><th>Inactive Devices</th><th>Autofixes</th>" +
                        "<th>Media Views</th><th>Page Views</th><th>Searches</th></tr><tr>";
                    var reports_html = "";
                    var list_html = "<div id=\"main_div\" class=\"main_div\">" +
                        "<div id =\"left_menu_div\" class=\"left_menu_div\">" +
                        "<h3 style =\"height: 20px; margin: 5px 0px;\">Reports List</h3>" +
                        "<div id =\"report_list\" class=\"report_list\">" +
                        "<button rpt_id =\"report_summary\" class=\"select_button\" selected=\"True\" " +
                        " onclick =\"selectReport(this)\">Summary</button>";
                    var summmaries = new Dictionary<int, string>();
                    var lists = new Dictionary<int, string>();
                    var names = new Dictionary<int, string>();
                    var reportnum = 0;
                    foreach (MailItem item in items)
                    {
                        if (item.ReceivedTime.Month == DateTime.Today.Month && item.ReceivedTime.Year == DateTime.Today.Year)
                        {
                            foreach (Attachment attachment in item.Attachments)
                            {
                                Status.Label = "Processing Report " + reportnum;
                                //if (reportnum <= 2)
                                //{
                                var file = System.IO.Path.GetTempFileName();
                                attachment.SaveAsFile(file);
                                var html = System.IO.File.ReadAllText(file);
                                var body = GetStringBetween(html, "<body>", "</body>");
                                var name = GetStringBetween(body, "<h2 class=\"text-center\">Usage Report - ", "</h2>");
                                names.Add(reportnum, name);
                                var active = GetStringBetween(GetStringBetween(body, "Active Devices:</td>", "</td>") + "<", ">", "<");
                                var inactive = GetStringBetween(GetStringBetween(body, "Inactive Devices:</td>", "</td>") + "<", ">", "<");
                                var page_allow = "100";
                                if (body.Contains("Page Views by Filter Action"))
                                {
                                    var temp = body.Substring(body.IndexOf("Page Views by Filter Action"));
                                    page_allow = GetStringBetween(temp, "allow-", "%");
                                }
                                var search_allow = "100";
                                if (body.Contains("Searches by Filter Action"))
                                {
                                    var temp = body.Substring(body.IndexOf("Searches by Filter Action"));
                                    search_allow = GetStringBetween(temp, "allow-", "%");
                                }
                                var media_views = 0;
                                if (body.Contains(" Media Views on"))
                                {
                                    var views = body.Split(new string[] { " Media Views on" }, StringSplitOptions.None);
                                    for (int i = 0; i < views.Length - 1; i++)
                                    {
                                        var num = views[i].Substring(views[i].Length - 4).Split(new string[] { ">" }, StringSplitOptions.None)[1];
                                        if (int.TryParse(num, out int inum)) media_views += inum;
                                    }
                                }
                                var autofix_count = Regex.Matches(body, "Autofix Message").Count;
                                reports_html += "<template id=\"report_" + reportnum + "\">" + body + "</template>";
                                //create summary record

                                summmaries.Add(reportnum, "\n<tr>\n<td>" + name + "</td>\n<td>" + active + "</td>\n<td>" + inactive +
                                    "</td>\n<td>" + autofix_count + "</td>\n<td>" + media_views + "</td>" +
                                    "\n<td><div class=\"progress\" style=\"--progress: " + page_allow + "%\">" +
                                    page_allow + "%</div>\n</td>" +
                                    "\n<td><div class=\"progress\" style=\"--progress: " + search_allow + "%\">" +
                                    search_allow + "%</div>\n</td>\n</tr>");

                                //add this list button
                                lists.Add(reportnum, "<button rpt_id=\"report_" + reportnum + "\" class=\"select_button\" selected=\"False\" " +
                                    "onclick =\"selectReport(this)\">" + name + " (" + active + ")</button>");
                                System.Diagnostics.Debug.WriteLine(item.ConversationTopic + " - " + item.ReceivedTime + ", " + attachment.DisplayName);
                                //}
                                reportnum++;
                            }
                        }
                    }

                    Status.Label = "Finalizing Report";
                    names = names.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                    foreach (var name in names.Keys)
                    {
                        list_html += lists[name];
                        summary_html += summmaries[name];
                    }
                    summary_html += "</table></template>";
                    list_html += "</div></div><div id=\"reports_container\" class=\"reports_container\"> " +
                            "<div id =\"report_page\" class=\"report_page\"></div></div></div>";
                    //compile the report from all the pieces
                    var head = Properties.Settings.Default.HeadHtml;
                    var tale = "</body></html>";
                    var path = System.IO.Path.GetTempPath() + "report.html";
                    System.IO.File.WriteAllText(path, head + list_html + summary_html + reports_html + tale);
                    System.Diagnostics.Process.Start(path);
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.Message + "\n\nError occured at: " + progress + "\n\nFolder Path: " + FolderPath);
                    Status.Label = "Error!";
                }

                Status.Label = " ";

            });
        }
    }

}
