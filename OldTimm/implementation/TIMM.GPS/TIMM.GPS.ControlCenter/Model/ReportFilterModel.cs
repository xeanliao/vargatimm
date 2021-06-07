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
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace TIMM.GPS.ControlCenter.Model
{
    public class ReportFilterModel : ObservableObject
    {
        private string m_ClientName;
        public string ClientName
        {
            get
            {
                return m_ClientName;
            }
            set
            {
                m_ClientName = value;
                RaisePropertyChanged("ClientName");
            }
        }

        private int? m_CampaignId;
        public int? CampaignId
        {
            get
            {
                return m_CampaignId;
            }
            set
            {
                m_CampaignId = value;
                RaisePropertyChanged("CampaignId");
            }
        }
    }
}
