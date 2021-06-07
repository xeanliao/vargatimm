using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using TIMM.GPS.Model;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using TIMM.GPS.Net.Http;
using TIMM.GPS.ControlCenter.Views;
using System.Collections.Generic;
using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.Helper;
using GalaSoft.MvvmLight.Messaging;
namespace TIMM.GPS.ControlCenter.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        public List<CampaignUI> ReportCollection { get; private set; }
        public ObservableCollection<KeyValuePair<string, string>> Clients { get; private set; }
        public ObservableCollection<KeyValuePair<int?, string>> Campaigns { get; private set; }
        public List<CampaignUI> ShowCollection { get; private set; }
        public ReportFilterModel Filter { get; private set; }

        public RelayCommand QueryCommand { get; private set; }
        public RelayCommand FilterClientCommand { get; private set; }
        public RelayCommand FilterCampaignCommand { get; private set; }
        public RelayCommand<int> DismissToMonitorCommand{get; private set;}

        public ReportViewModel()
        {
            ReportCollection = new List<CampaignUI>();
            Clients = new ObservableCollection<KeyValuePair<string, string>>();
            Campaigns = new ObservableCollection<KeyValuePair<int?, string>>();
            ShowCollection = new List<CampaignUI>();
            Filter = new ReportFilterModel();

            QueryCommand = new RelayCommand(QueryExecute);
            FilterClientCommand = new RelayCommand(FilterClientExecute);
            FilterCampaignCommand = new RelayCommand(FilterCampaignExecute);
            DismissToMonitorCommand = new RelayCommand<int>(DismissToMonitorExecute);

            QueryExecute();
        }

        private void QueryExecute()
        {
            HttpClient.Get<List<CampaignUI>>("service/task/report/", 
                args => {
                    Clients.Clear();
                    Campaigns.Clear();
                    ReportCollection.Clear();
                    Dictionary<string, TreeViewItem> clients = new Dictionary<string, TreeViewItem>();
                    args.Data.ForEach(i =>
                    {
                        ReportCollection.Add(i);
                        Campaigns.Add(new KeyValuePair<int?, string>(i.Id, i.DisplayName));
                        if (!clients.ContainsKey(i.ClientName))
                        {
                            clients.Add(i.ClientName, new TreeViewItem { Header = string.Format("Client[{0}]", i.ClientName) });
                            Clients.Add(new KeyValuePair<string,string>(i.ClientName, i.ClientName));
                        }
                        var clientTreeView = clients[i.ClientName];
                        var campaignItem = new TreeViewItem { Header = i.DisplayName };
                        clientTreeView.Items.Add(campaignItem);
                        foreach (var subMap in i.SubMaps)
                        {
                            foreach (var dMap in subMap.DistributionMaps)
                            {
                                campaignItem.Items.Add(new TreeViewItem { Header = dMap.Name });
                            }
                        }
                        
                    });
                    Filter = new ReportFilterModel { ClientName = "", CampaignId = 0 };
                    FilterCampaignExecute();
                    Clients.Insert(0, new KeyValuePair<string,string>("", "--Select Client--"));
                    Campaigns.Insert(0, new KeyValuePair<int?, string>(0, "--Select Campaign--"));
                    RaisePropertyChanged("Clients");
                    RaisePropertyChanged("Campaigns");
                    RaisePropertyChanged("Filter");
                });
        }

        private bool IgnorCampaignFilter = false;

        private void FilterClientExecute()
        {
            IgnorCampaignFilter = true;
            ShowCollection.Clear();
            Campaigns.Clear();
            ReportCollection.ForEach(i =>
            {

                if (string.IsNullOrWhiteSpace(Filter.ClientName) || string.Compare(Filter.ClientName, i.ClientName) == 0)
                {
                    Campaigns.Add(new KeyValuePair<int?, string>(i.Id, i.DisplayName));
                }
            });
            Campaigns.Insert(0, new KeyValuePair<int?, string>(0, "--Select Campaign--"));
            Filter.CampaignId = 0;
            RaisePropertyChanged("Campaigns");
            RaisePropertyChanged("Filter");
            ReportCollection.ForEach(i =>
            {
                if (string.IsNullOrWhiteSpace(Filter.ClientName)
                    || string.Compare(i.ClientName, Filter.ClientName) == 0)
                {
                    ShowCollection.Add(i);
                }
            });
            IgnorCampaignFilter = false;
            Messenger.Default.Send<List<CampaignUI>>(ShowCollection, "report list");
        }

        private void FilterCampaignExecute()
        {
            if (!IgnorCampaignFilter)
            {
                ShowCollection.Clear();

                ReportCollection.ForEach(i =>
                {
                    if (string.IsNullOrWhiteSpace(Filter.ClientName)
                        || string.Compare(i.ClientName, Filter.ClientName) == 0)
                    {

                        if ((Filter.CampaignId ?? 0) == 0
                            || i.Id == Filter.CampaignId.Value)
                        {
                            ShowCollection.Add(i);
                        }
                    }
                });

                Messenger.Default.Send<List<CampaignUI>>(ShowCollection, "report list");
            }
        }

        private void DismissToMonitorExecute(int taskId)
        {
            Confirm dialog = new Confirm("Confirm", "Do you really want to move report back to GPS Montor?");
            dialog.Closed += (s, e) => { 
                if(dialog.DialogResult == true)
                {
                    HttpClient.Get(string.Format("service/task/report/backtomonitor/{0}", taskId), 
                        ()=>
                        {
                            QueryExecute();
                        }
                    );
                }
            };
            dialog.Show();
        }
    }
}
