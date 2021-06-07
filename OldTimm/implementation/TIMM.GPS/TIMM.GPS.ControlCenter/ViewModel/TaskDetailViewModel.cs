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
using TIMM.GPS.Model;

namespace TIMM.GPS.ControlCenter.ViewModel
{
    public class TaskDetailViewModel : ValidationViewModel
    {
        public Task Detail { get; private set; }

        [Required, Display(Name = "Name")]
        public string Name
        {
            get
            {
                return Detail.Name;
            }
            set
            {
                bool isValid = ValidateProperty("Name", value);
                if (isValid)
                {
                    Detail.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        [Required, Display(Name = "Telephone")]
        public string Telephone
        {
            get
            {
                return Detail.Telephone;
            }
            set
            {
                bool isValid = ValidateProperty("Telephone", value);
                if (isValid)
                {
                    Detail.Telephone = value;
                    RaisePropertyChanged("Telephone");
                }
            }
        }

        [Required, Display(Name = "Email")]
        public string Email
        {
            get
            {
                return Detail.Email;
            }
            set
            {
                bool isValid = ValidateProperty("Email", value);
                if (isValid)
                {
                    Detail.Email = value;
                    RaisePropertyChanged("Email");
                }
            }
        }

        [Required, Display(Name = "Auditor")]
        public User Auditor
        {
            get
            {
                return Detail.Auditor;
            }
            set
            {
                bool isValid = ValidateProperty("Auditor", value);
                if (isValid)
                {
                    Detail.Auditor = value;
                    RaisePropertyChanged("Auditor");
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

        public TaskDetailViewModel(Task task)
        {
            Detail = task;

            SaveCommand = new RelayCommand(SaveExcute);
        }

        private bool ValidateAllProperty()
        {
            bool isValid = true;
            if (isValid)
                isValid = this.ValidateProperty("Name", Detail.Name);
            if (isValid)
                isValid = this.ValidateProperty("Telephone", Detail.Telephone);
            if (isValid)
                isValid = this.ValidateProperty("Email", Detail.Email);
            if (isValid)
                isValid = this.ValidateProperty("Auditor", Detail.Auditor);
            if (isValid)
                isValid = this.ValidateProperty("Date", Detail.Date);
            return isValid;
        }

        private void SaveExcute()
        {
            bool isValid = ValidateAllProperty();
            if (isValid)
            {
                Action saveAction = () =>
                {
                    HttpClient.Post<Task>(
                        "/service/task/modify/",
                        Detail,
                        args =>
                        {
                            Messenger.Default.Send<bool, TaskDetailViewModel>(true);
                            Messenger.Default.Send<bool, Views.EditTask>(true);
                            new MonitorViewModel().QueryCommand.Execute(true);
                        }
                        );
                };

                saveAction();
            }
            else
            {
                var dialog = new Views.Alert("All the fields are required,Please check your input and try again.");
                dialog.Show();
            }
        }
    }
}
