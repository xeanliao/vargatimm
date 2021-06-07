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
using TIMM.GPS.ControlCenter.Model;
using System.ComponentModel.DataAnnotations;
using GalaSoft.MvvmLight.Command;
using TIMM.GPS.Net.Http;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;

namespace TIMM.GPS.ControlCenter.ViewModel
{
    public class CampaignDetailViewModel : ValidationViewModel
    {
        public CampaignUI Detail { get; private set; }

        [Required, Display(Name="Client Name")]
        public string ClientName
        {
            get
            {
                return Detail.ClientName;
            }
            set
            {
                bool isValid = ValidateProperty("ClientName", value);
                if (isValid)
                {
                    Detail.ClientName = value;
                    RaisePropertyChanged("ClientName");
                }
            }
        }

        [Required, Display(Name = "Client Code")]
        public string ClientCode
        {
            get
            {
                return Detail.ClientCode;
            }
            set
            {
                bool isValid = ValidateProperty("ClientCode", value);
                if (isValid)
                {
                    Detail.ClientCode = value;
                    RaisePropertyChanged("ClientCode");
                }
            }
        }

        [Required, Display(Name = "Contact Name")]
        public string ContactName
        {
            get
            {
                return Detail.ContactName;
            }
            set
            {
                bool isValid = ValidateProperty("ContactName", value);
                if (isValid)
                {
                    Detail.ContactName = value;
                    RaisePropertyChanged("ContactName");
                }
            }
        }

        [Required, Display(Name = "Area Description")]
        public string AreaDescription
        {
            get
            {                
                return Detail.AreaDescription;
            }
            set
            {
                bool isValid = ValidateProperty("AreaDescription", value);
                if (isValid)
                {
                    Detail.AreaDescription = value;
                    RaisePropertyChanged("AreaDescription");
                }
            }
        }

        [Required, Display(Name = "Date")]
        public DateTime Date
        {
            get
            {
                return Detail.Date;
            }
            set
            {
                bool isValid = ValidateProperty("Date", value);
                if (isValid)
                {
                    Detail.Date = value;
                    RaisePropertyChanged("Date");
                }
            }
        }

        public RelayCommand SaveCommand { get; private set; }

        ObservableCollection<string> totalTypes;

        public CampaignDetailViewModel(CampaignUI campaign)
        {
            Detail = campaign;
            
            totalTypes = new ObservableCollection<string>()
            {
                "<select total type>",
                "APT + HOME",
                "APT ONLY",
                "HOME ONLY"
            };

            if (!totalTypes.Contains(Detail.AreaDescription))
            {
                Detail.AreaDescription = totalTypes[0];
            }
            
            SaveCommand = new RelayCommand(SaveExcute);
        }

        private bool ValidateAllProperty()
        {
            bool isValid = true;
            if (isValid)
            {
                isValid = this.ValidateProperty("ClientName", Detail.ClientName);
            }
            if (isValid)
            {
                isValid = this.ValidateProperty("ClientCode", Detail.ClientCode);
            }
            if (isValid)
            {
                isValid = this.ValidateProperty("ContactName", Detail.ContactName);
            }
            if (isValid)
            {
                if (Detail.AreaDescription == totalTypes[0])
                {
                    isValid = false;
                }
                else
                {
                    isValid = this.ValidateProperty("AreaDescription", Detail.AreaDescription);
                }
            }
            if (isValid)
            {
                isValid = this.ValidateProperty("Date", Detail.Date);
            }
            return isValid;
        }

        private void SaveExcute()
        {
            bool isValid = ValidateAllProperty();
            if (isValid)
            {
                Action saveAction = () =>
                {
                    HttpClient.Post<CampaignUI>(
                        "/service/campaign/modify/",
                        Detail,
                        args =>
                        {
                            Messenger.Default.Send<bool, CampaignViewModel>(true);
                            Messenger.Default.Send<bool, Views.AddCampagin>(true);
                            new CampaignViewModel().QueryCommand.Execute(true);//when saved,reload data.
                        }
                        );
                };

                if (Detail.LogoFile != null && Detail.LogoFile.Length > 0)
                {
                    HttpClient.PostFile<UploadResult>("/Handler/uploadfile.ashx?type=campaignimage", Detail.LogoFileName, Detail.LogoFile, result =>
                    {
                        if (result.Data.IsSuccess == true)
                        {
                            Detail.Logo = result.Data.Name;
                            saveAction();
                        }
                    });
                }
                else
                {
                    saveAction();
                }
            }
            else
            {
                var dialog = new Views.Alert("All the fields are required,Please check your input and try again.");
                dialog.Show();
            }
        }
    }
}
