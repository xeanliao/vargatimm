using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TIMM.GPS.Model;
using System.Collections.ObjectModel;
using TIMM.GPS.Net.Http;
using System.Collections.Generic;
using TIMM.GPS.ControlCenter.Model;
using System;
using System.Windows.Browser;
using TIMM.GPS.Helper;
using TIMM.GPS.ControlCenter.Views;

namespace TIMM.GPS.ControlCenter.ViewModel
{
    public class DistributionMapsViewModel : ViewModelBase
    {
        public ObservableCollection<CampaignUI> CampaignCollection { get; private set; }
        public CampaignCriteria Criteria { get; set; }

        public RelayCommand<bool> SelectAllCommand { get; private set; }
        public RelayCommand QueryCommand { get; private set; }
        public RelayCommand<int> OpenDMapWindowCommand { get; private set; }
        public RelayCommand<CampaignUI> PublishBackToCMapCommand { get; private set; }
        public RelayCommand<CampaignUI> PublishToMonitorCommand { get; private set; }

        public DistributionMapsViewModel()
        {
            CampaignCollection = new ObservableCollection<CampaignUI>();
            Criteria = new CampaignCriteria();

            SelectAllCommand = new RelayCommand<bool>(SelectAllExecute);
            QueryCommand = new RelayCommand(QueryExecute);
            OpenDMapWindowCommand = new RelayCommand<int>(OpenDMapWindowExecute);

            PublishBackToCMapCommand = new RelayCommand<CampaignUI>(PublishBackToCMapExecute);
            PublishToMonitorCommand = new RelayCommand<CampaignUI>(PublishToMonitorExecute);
            
            QueryCommand.Execute(true);
        }

        private void QueryExecute()
        {
            
            HttpClient.Post<QueryResult<CampaignUI>>("/service/campaign/dmap/", Criteria,
                args => {
                    CampaignCollection.Clear();
                    args.Data.Result.ForEach(i => CampaignCollection.Add(i));
                    Criteria.TotalRecord = args.Data.TotalRecord;
                    RaisePropertyChanged("CampaignCollection");
                    RaisePropertyChanged("Criteria");
                });
            
        }
        private void SelectAllExecute(bool state)
        {
            foreach (var item in CampaignCollection)
            {
                item.IsChecked = state;
            }
            RaisePropertyChanged("CampaignCollection");
        }
        private void OpenDMapWindowExecute(int id)
        {
            //open new window to edit campaign
            string url = string.Format("DistributionMap.aspx?cid={0}&rn={1}", id, DateTimeOffset.UtcNow.Ticks);
            string windowName = string.Format("DMap{0}", id);
            ScriptHelper.OpenWindow(url, windowName);
        }

        private void PublishBackToCMapExecute(CampaignUI campaign)
        {
            string message = string.Format("Are you sure you would like to move \r\n{0}\r\nto Campaigns? Any changes that were made will be lost.", campaign.DisplayName);
            Confirm confirmDialog = new Confirm("Confirm", message);
            confirmDialog.Closed += (se, ae) =>
            {
                if (confirmDialog.DialogResult == true)
                {
                    var groups = new int[] { 46, 47, 53 };
                    HttpClient.Post<List<User>>("service/user/groups/",
                        groups,
                        args =>
                        {
                            AssignUser dialog = new AssignUser("Campaign Dismiss", args.Data);
                            dialog.Closed += (s, a) =>
                            {
                                if (dialog.DialogResult == true)
                                {
                                    string json = string.Format(@"{0}""campaignIds"":[{1}],""userId"":""{2}"",""status"":0{3}",
                                        "{", campaign.Id, dialog.SelectedUser, "}");
                                    HttpClient.PostContent<PublishResult>("CampaignServices/CampaignWriterService.svc/PublishCampaign",
                                        json, result =>
                                        {
                                            if (result.Data.IsSuccess)
                                            {
                                                Criteria.PageIndex = 0;
                                                QueryCommand.Execute(true);
                                            }
                                            else
                                            {
                                                new Alert(result.Data.Message).Show();
                                            }
                                        });
                                }
                            };
                            dialog.Show();
                        });
                }
            };
            confirmDialog.Show();
        }

        private void PublishToMonitorExecute(CampaignUI campaign)
        {
            var groups = new int[] { 48, 49, 50, 51, 52, 53, 46 };
            HttpClient.Post<List<User>>("service/user/groups/",
                groups,
                args =>
                {
                    AssignUser dialog = new AssignUser("Camapign Publish", args.Data);
                    dialog.Closed += (s, a) =>
                    {
                        if (dialog.DialogResult == true)
                        {
                            string json = string.Format(@"{0}""campaignIds"":[{1}],""userId"":""{2}"",""status"":2{3}",
                                "{", campaign.Id, dialog.SelectedUser, "}");
                            HttpClient.PostContent<PublishResult>("CampaignServices/CampaignWriterService.svc/PublishCampaign",
                                json, result =>
                                {
                                    if (result.Data.IsSuccess)
                                    {
                                        Criteria.PageIndex = 0;
                                        QueryCommand.Execute(true);
                                    }
                                    else
                                    {
                                        new Alert(result.Data.Message).Show();
                                    }
                                });
                        }
                    };
                    dialog.Show();
                });
        }
    }
}
