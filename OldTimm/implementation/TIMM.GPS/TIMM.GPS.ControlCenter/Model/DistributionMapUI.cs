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
using TIMM.GPS.Model;
using System.ComponentModel;
using System.Collections.Generic;

namespace TIMM.GPS.ControlCenter.Model
{
    public class DistributionMapUI : DistributionMap, INotifyPropertyChanged
    {
        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        #endregion

        private bool _isChecked;

        /// <summary>
        /// Use for checkbox in UI
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                this.RaisePropertyChanged("IsChecked");
            }
        }        

        public string PrintDisplayName
        {
            get
            {
                return string.Format("DM MAP {0} ({1})", base.Id, base.Name);
            }
        }

        public string PrintBigDMapTitle
        {
            get
            {
                return string.Format("DM MAP {0} ({1})", Id, Name);
            }
        }

        public string PringBigDMapMapTitle
        {
            get
            {
                return string.Format("{0}-{1}", Campaign.DisplayName, Name);
            }
        }

        public CampaignUI Campaign { get; set; }

        public int DisplayTotal
        {
            get
            {
                return base.Total - base.TotalAdjustment;
            }
        }

        public int DisplayCount
        {
            get
            {
                return base.Penetration - base.CountAdjustment;
            }
        }

        public double DisplayPenetration
        {
            get
            {
                if (DisplayTotal == 0)
                {
                    return 0;
                }
                return (double)DisplayCount / (double)DisplayTotal;
            }
        }

        public List<NdAddressUI> NdAddress { get; set; }
    }
}
