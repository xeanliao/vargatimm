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

namespace TIMM.GPS.ControlCenter.ViewModel
{
    public class MonitorViewModel : ViewModelBase
    {
        public ObservableCollection<Task> TaskCollection { get; private set; }
        public ObservableCollection<User> Users { get; private set; }
        public TaskCriteria Criteria { get; private set; }

        public RelayCommand QueryCommand { get; private set; }
        public RelayCommand<int> OpenMonitorCommand { get; private set; }
        public RelayCommand<Task> ModifyCommand { get; private set; }
        public RelayCommand<Task> PublishBackToDMapCommand { get; private set; }
        public RelayCommand<Task> MarkFinishedCommand { get; private set; }

        public MonitorViewModel()
        {
            TaskCollection = new ObservableCollection<Task>();
            Criteria = new TaskCriteria();

            QueryCommand = new RelayCommand(QueryExecute);
            OpenMonitorCommand = new RelayCommand<int>(OpenMonitorExecute);
            ModifyCommand = new RelayCommand<Task>(ModifyExcute);
            PublishBackToDMapCommand = new RelayCommand<Task>(PublishBackToDMapExecute);
            MarkFinishedCommand = new RelayCommand<Task>(MarkFinishedExecute);

            QueryExecute();
        }

        private void QueryExecute()
        {
            HttpClient.Post<QueryResult<Task>>("/service/task/query/",
                Criteria,
                args =>
                {
                    TaskCollection.Clear();
                    args.Data.Result.ForEach(i => TaskCollection.Add(i));
                    RaisePropertyChanged("TaskCollection");
                    Criteria.TotalRecord = args.Data.TotalRecord;
                    RaisePropertyChanged("Criteria");
                });
        }
        private void OpenMonitorExecute(int id)
        {
            string url = string.Format("taskMonitor.aspx?taskid={0}&rn={1}", id, DateTimeOffset.UtcNow.Ticks);
            string windowName = string.Format("Monitor{0}", id);
            ScriptHelper.OpenWindow(url, windowName);
        }
        private void ModifyExcute(Task entity)
        {
            Task editItem = null;
            if (entity == null)
            {
                editItem = new Task { Date = DateTime.Now };
            }
            else
            {
                editItem = entity.DeepClone();
            }
            var dialog = new Views.EditTask();
            var viewModel = new ViewModel.TaskDetailViewModel(editItem);
            dialog.DataContext = viewModel;
            dialog.Closed += (s, e) =>
            {
                dialog.DataContext = null;
                viewModel.Cleanup();
                QueryExecute();
            };
            dialog.Show();
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

        private void PublishBackToDMapExecute(Task task)
        {
            Confirm confirmDialog = new Confirm("Confirm", "Are you sure you want to delete this Task?");
            confirmDialog.Closed += (se, ae) =>
            {
                if (confirmDialog.DialogResult == true)
                {
                    var groups = new int[] { 52, 53 };
                    HttpClient.Post<List<User>>("service/user/groups/",
                        groups,
                        args =>
                        {
                            AssignUser dialog = new AssignUser("Campaign Dismiss", args.Data);
                            dialog.Closed += (s, a) =>
                            {
                                if (dialog.DialogResult == true)
                                {
                                    string json = string.Format(@"{0}""taskIds"":[{1}],""userId"":""{2}""{3}",
                                        "{", task.Id, dialog.SelectedUser, "}");
                                    HttpClient.PostContent<PublishResult>("TaskServices/TaskWriterService.svc/DismissTasks",
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

        private void MarkFinishedExecute(Task task)
        {
            string json = string.Format(@"{0}""tids"":[{1}]{2}", "{", task.Id, "}");
            HttpClient.PostContent<TaskStatus>("TaskServices/TaskReaderService.svc/GetStartOrStopByTaskId",
                json,
                args =>
                {
                    if (args.Data.CanMarkFinish)
                    {
                        Confirm confirmDialog = new Confirm("Confirm", "Are you sure you want to mark finish this Task?");
                        confirmDialog.Closed += (s, e) =>
                        {
                            if (confirmDialog.DialogResult == true)
                            {
                                string finishJson = string.Format(@"{0}""taskIds"":[{1}]{2}", "{", task.Id, "}");
                                HttpClient.PostContent("TaskServices/TaskWriterService.svc/MarkFinish", finishJson,
                                    () =>
                                    {
                                        Criteria.PageIndex = 0;
                                        QueryExecute();
                                    });

                            }
                        };
                        confirmDialog.Show();
                    }
                    else
                    {
                        new Alert("You could not mark finish for this task because you have not stop monitor yet!").Show();
                    }
                });
        }
    }
}
