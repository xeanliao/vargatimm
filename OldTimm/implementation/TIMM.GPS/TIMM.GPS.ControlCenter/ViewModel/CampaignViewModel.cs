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
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Linq;

namespace TIMM.GPS.ControlCenter.ViewModel
{
    public class CampaignViewModel : ViewModelBase, INotifyDataErrorInfo
    {

        public ObservableCollection<CampaignUI> CampaignCollection { get; private set; }
        public ObservableCollection<User> Users { get; private set; }
        public CampaignCriteria Criteria { get; set; }
        //public CampaignUI Detail { get; private set; }
        public RelayCommand<bool> SelectAllCommand { get; private set; }
        public RelayCommand QueryCommand { get; private set; }
        public RelayCommand<CampaignUI> CopyCommand { get; private set; }
        public RelayCommand<CampaignUI> EditCommand { get; private set; }
        public RelayCommand<int> DeleteCommand { get; private set; }
        public RelayCommand<int> OpenCMapWindowCommand { get; private set; }
        public RelayCommand<int> PublishToDMapCommand { get; private set; }


        public CampaignViewModel()
        {
            CampaignCollection = new ObservableCollection<CampaignUI>();
            Criteria = new CampaignCriteria();
            //Detail = new CampaignUI();
            Users = new ObservableCollection<User>();

            SelectAllCommand = new RelayCommand<bool>(SelectAllExecute);
            QueryCommand = new RelayCommand(QueryExecute);
            CopyCommand = new RelayCommand<CampaignUI>(CopyExecute);
            EditCommand = new RelayCommand<CampaignUI>(EditExecute);
            DeleteCommand = new RelayCommand<int>(DeleteExecute);
            OpenCMapWindowCommand = new RelayCommand<int>(OpenCMapWindowExecute);
            PublishToDMapCommand = new RelayCommand<int>(PublishToDMapExecute);

            Messenger.Default.Register<CampaignUI>(this, RefreshData);

            QueryCommand.Execute(true);
        }

        private void QueryExecute()
        {
            
            HttpClient.Post<QueryResult<CampaignUI>>("/service/campaign/cmap/", Criteria,
                args => {
                    CampaignCollection.Clear();
                    args.Data.Result.ForEach(i => CampaignCollection.Add(i));
                    Criteria.TotalRecord = args.Data.TotalRecord;
                    RaisePropertyChanged("CampaignCollection");
                    RaisePropertyChanged("Criteria");
                });
            
        }

        private void RefreshData(CampaignUI entity)
        {
            CampaignUI orgItem = null;
            foreach (CampaignUI item in CampaignCollection)
            {
                if (item.Id == entity.Id)
                {
                    orgItem = item;
                    break;
                }
            }
            orgItem = entity;
            RaisePropertyChanged("CampaignCollection");
            
        }
        private void SelectAllExecute(bool state)
        {
            foreach (var item in CampaignCollection)
            {
                item.IsChecked = state;
            }
            RaisePropertyChanged("CampaignCollection");
        }
        private void CopyExecute(CampaignUI campaign)
        {
            string message = string.Format("Are you sure you want to copy \r\nthe following Campaign(s): \r\n{0}", campaign.DisplayName);
            Confirm dialog = new Confirm("Confirm", message);
            dialog.Closed += (s, a) =>
            {
                if (dialog.DialogResult == true)
                {
                    string json = string.Format(@"{0}""campaignIds"":[{1}]{2}", "{", campaign.Id, "}");
                    HttpClient.PostContent("/CampaignServices/CampaignWriterService.svc/CopyCampaigns",
                        json,
                        () =>
                        {
                            Criteria.PageIndex = 0;
                            QueryExecute();
                        });
                }
            };
            dialog.Show();
        }

        private void EditExecute(CampaignUI entity)
        {
            LoadSales(() =>
            {
                CampaignUI editItem = null;
                if (entity == null)
                {
                    editItem = new CampaignUI { Date = DateTime.Now };
                }
                else
                {
                    editItem = entity.DeepClone();
                }
                var dialog = new Views.AddCampagin();
                var viewModel = new ViewModel.CampaignDetailViewModel(editItem);
                dialog.DataContext = viewModel;
                dialog.Closed += (s, e) => {
                    dialog.DataContext = null;
                    viewModel.Cleanup();
                    //when updated,reload data to grid.
                    QueryExecute();
                };
                dialog.Show();
            });
        }
        private void DeleteExecute(int id)
        {
            string message = "Are you sure you want to delete this Campaign?";
            Confirm dialog = new Confirm("Confirm", message);
            dialog.Closed += (s, a) =>
            {
                if (dialog.DialogResult == true)
                {
                    string json = string.Format(@"{0}""campaignIds"":[{1}]{2}", "{", id, "}");
                    HttpClient.PostContent("CampaignServices/CampaignWriterService.svc/DeleteCampaigns",
                        json,
                        () =>
                        {
                            Criteria.PageIndex = 0;
                            QueryExecute();
                        }
                    );
                }
            };
            dialog.Show();
        }
        private void OpenCMapWindowExecute(int id)
        {
            //open new window to edit campaign
            string url = string.Format("Campaign.aspx?cid={0}&rn={1}", id, DateTimeOffset.UtcNow.Ticks);
            string windowName = string.Format("CMap{0}", id);
            ScriptHelper.OpenWindow(url, windowName);
        }
        private void PublishToDMapExecute(int id)
        {
            var groups = new int[] { 51, 52, 53 };
            HttpClient.Post<List<User>>("service/user/groups/", 
                groups, 
                args => {
                    AssignUser dialog = new AssignUser("Camapign Publish", args.Data);
                    dialog.Closed += (s, a) => {
                        if (dialog.DialogResult == true)
                        {
                            string json = string.Format(@"{0}""campaignIds"":[{1}],""userId"":""{2}"",""status"":1{3}",
                                "{", id, dialog.SelectedUser, "}");
                            HttpClient.PostContent<PublishResult>("CampaignServices/CampaignWriterService.svc/PublishCampaign",
                                json, result => {
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

        private void LoadSales(Action toDo)
        {
            HttpClient.Get<List<User>>("service/user/sales/", 
                args => 
                {
                    Users.Clear();
                    args.Data.ForEach(i => Users.Add(i));
                    RaisePropertyChanged("Users");
                    toDo();
                }
            );
        }
        #region INotifyDataErrorInfo
        protected Dictionary<string, List<string>> m_ErrorList = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            CheckErrorCollectionForProperty(propertyName);

            return m_ErrorList[propertyName];
        }

        public bool HasErrors
        {
            get { return m_ErrorList.Values.Sum(c => c.Count) > 0; }
        }

        protected void CheckErrorCollectionForProperty(string propertyName)
        {
            if (!m_ErrorList.ContainsKey(propertyName))
            {
                m_ErrorList[propertyName] = new List<string>();
            }
        }
        #endregion
    }
}
