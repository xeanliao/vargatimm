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
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.Model;
using TIMM.GPS.Helper;
using TIMM.GPS.ControlCenter.ViewModel;
using TIMM.GPS.Net.Http;
using TIMM.GPS.Silverlight.Utility;
using System.Windows.Browser;

namespace TIMM.GPS.ControlCenter.Views
{
    public partial class Reports : Page
    {
        public Reports()
        {
            InitializeComponent();

            Messenger.Default.Register<List<CampaignUI>>("report list","report list", RebuildTreeView);

            InitData();
        }

        private void InitData()
        {
            var viewModel = this.DataContext as ReportViewModel;
            if (viewModel != null)
            {
                viewModel.QueryCommand.Execute(true);
            }
        }

        private void RebuildTreeView(List<CampaignUI> items)
        {
            treeViewReports.Items.Clear();
            Dictionary<string, TreeViewItem> clients = new Dictionary<string, TreeViewItem>();
            items.ForEach(i => {
                
                if (!clients.ContainsKey(i.ClientName))
                {
                    StackPanel headerPanel = new StackPanel();
                    
                    treeViewReports.Items.Add(new TreeViewItem {
                        Header = BuildClient(i),
                        IsExpanded = true
                    });
                    clients.Add(i.ClientName, treeViewReports.Items[treeViewReports.Items.Count - 1] as TreeViewItem);
                }
                var root = clients[i.ClientName];
                var campaignItem = new TreeViewItem { Header = BuildCampaign(i), IsExpanded = true };
                root.Items.Add(campaignItem);
                foreach (var subMap in i.SubMaps)
                {
                    foreach (var dMap in subMap.DistributionMaps)
                    {
                        foreach (var task in dMap.Tasks)
                        {
                            campaignItem.Items.Add(new TreeViewItem { Header = BuildTask(task), IsExpanded = true });
                        }
                    }
                }

            });
        }

        private object BuildClient(CampaignUI client)
        {
            Label root = new Label();
            root.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            root.Content = string.Format("Client[{0}]", client.ClientName);
            root.FontSize = 14;
            root.FontWeight = FontWeights.Bold;
            return root;
        }

        private object BuildCampaign(CampaignUI campaign)
        {
            StackPanel root = new StackPanel
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            Label name = new Label
            {
                Content = string.Format("Campaign[{0}]", campaign.DisplayName),
                FontSize = 12,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };
            root.Children.Add(name);
            HyperlinkButton report = new HyperlinkButton
            {
                Content = new TextBlock
                {
                    Text = "[Print]",
                    Foreground = new SolidColorBrush(Colors.Blue),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                },
                
                Margin = new Thickness(10,0,10,0)
            };

            // print format: http ://localhost:49356/NewControlCenter.aspx?id=1923643415#ReportPrint
            //report.Click += (s, e) =>
            //{
            //    NavigationService.Navigate(new Uri("/ReportPrint?id=" + campaign.Id, UriKind.Relative));
            //};

            report.Click += (s, e) =>
            {
                var originUri = HtmlPage.Document.DocumentUri;

                var printUri = new Uri(
                    string.Format("{0}?id={1}#ReportPrint",
                    originUri.OriginalString.Substring(0, originUri.OriginalString.Length - originUri.Fragment.Length),
                    campaign.Id));
                
                //if (true == HtmlPage.IsPopupWindowAllowed)
                    HtmlPage.PopupWindow(printUri, "new", new HtmlPopupWindowOptions());

            };

            #region old report click

            //report.Click += (s,e)=>{
            //    string url = string.Format("Reports.aspx?cid={0}", campaign.Id);
            //    string windowName = string.Format("Reports{0}", campaign.Id);
            //    ScriptHelper.OpenWindow(url, windowName);
            //};

            #endregion

            root.Children.Add(report);
            return root;
        }

        private object BuildTask(Task task)
        {
            StackPanel root = new StackPanel
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            //dismiss check button
            RadioButton ckbDismiss = new RadioButton
            {
                Content = new TextBlock
                {
                    Text = task.Name,
                    FontSize = 12,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                },
                GroupName = "Task",
                Tag = task.Id
            };

            root.Children.Add(ckbDismiss);
            //report button
            HyperlinkButton report = new HyperlinkButton
            {

                Content = new TextBlock
                {
                    Text = "[Report]",
                    Foreground = new SolidColorBrush(Colors.Blue),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                },
                Margin = new Thickness(10, 0, 10, 0)
            };
            report.Click += (s, e) =>
            {
                string url = string.Format("ReportsTask.aspx?tid={0}", task.Id);
                string windowName = string.Format("ReportsTask{0}", task.Id);
                ScriptHelper.OpenWindow(url, windowName);
            };
            root.Children.Add(report);
            //review button
            HyperlinkButton review = new HyperlinkButton
            {
                Content = new TextBlock
                {
                    Text = "[Review]",
                    Foreground = new SolidColorBrush(Colors.Blue),
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                },
                Margin = new Thickness(10, 0, 10, 0)
            };
            review.Click += (s, e) =>
            {
                string url = string.Format("EditGTU.aspx?id={0}", task.Id);
                string windowName = string.Format("EditGTU{0}", task.Id);
                ScriptHelper.OpenWindow(url, windowName);
            };
            root.Children.Add(review);
            
 
            return root;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var allButton = treeViewReports.GetElementType<RadioButton>();
            foreach (var item in allButton)
            {
                if (item.IsChecked == true)
                {
                    var taskId = (int)item.Tag;
                    var viewModel = this.DataContext as ReportViewModel;
                    if (viewModel != null)
                    {
                        viewModel.DismissToMonitorCommand.Execute(taskId);
                    }
                }
            }
        }
    }
}
