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
using GalaSoft.MvvmLight.Command;
using System.Windows.Browser;
using TIMM.GPS.Helper;

namespace TIMM.GPS.ControlCenter.ViewModel
{
    public class AdministrationViewModel : ViewModelBase
    {
        public RelayCommand UserManagementCommand { get; private set; }
        public RelayCommand NonDeliverablesCommand { get; private set; }
        public RelayCommand GTUManagementCommand { get; private set; }
        public RelayCommand GTUAvailableListCommand { get; private set; }
        public RelayCommand GtubagManagementCommand { get; private set; }
        public RelayCommand AssignGtuBagstoAuditorsCommand { get; private set; }
        public RelayCommand DistributorManagementCommand { get; private set; }

        public AdministrationViewModel()
        {
            UserManagementCommand = new RelayCommand(UserManagementExecute);
            NonDeliverablesCommand = new RelayCommand(NonDeliverablesExecute);
            GTUManagementCommand = new RelayCommand(GTUManagementExecute);
            GTUAvailableListCommand = new RelayCommand(GTUAvailableListExecute);
            GtubagManagementCommand = new RelayCommand(GtubagManagementExecute);
            AssignGtuBagstoAuditorsCommand = new RelayCommand(AssignGtuBagstoAuditorsExecute);
            DistributorManagementCommand = new RelayCommand(DistributorManagementExecute);
        }

        private void UserManagementExecute()
        {
            ScriptHelper.OpenWindow("Users.aspx", "Users");
        }
        private void NonDeliverablesExecute()
        {
            ScriptHelper.OpenWindow("NonDeliverables.aspx", "NonDeliverables");
        }
        private void GTUManagementExecute()
        {
            ScriptHelper.OpenWindow("GtuAdmin.aspx?AssignNameToGTUFlag=true", "GtuAdmin");
        }
        private void GTUAvailableListExecute()
        {
            ScriptHelper.OpenWindow("AvailableGTUList.aspx", "AvailableGTUList");
        }
        private void GtubagManagementExecute()
        {
            ScriptHelper.OpenWindow("AdminGtuToBag.aspx", "AdminGtuToBag");
        }
        private void AssignGtuBagstoAuditorsExecute()
        {
            ScriptHelper.OpenWindow("AdminGtuBagToAuditor.aspx", "AdminGtuBagToAuditor");
        }
        private void DistributorManagementExecute()
        {
            ScriptHelper.OpenWindow("AdminDistributorCompany.aspx", "AdminDistributorCompany");
        }
    }
}
