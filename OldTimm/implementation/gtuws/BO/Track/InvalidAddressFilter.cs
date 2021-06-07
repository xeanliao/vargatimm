using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.ModelLayer.Common;
using GTU.Utilities.Logging;

namespace GTU.BusinessLayer.Track
{
    public class InvalidAddressFilter : IDataTrackProcess
    {
        //public IDataTrackProcess Node
        //{
        //    get;
        //    set;
        //}

        private ICoordinate _addressCoordinate;
        private IRadius _radius;

        /// <summary>
        /// Define forbidden address central location
        /// </summary>
        public ICoordinate AddressCoordinate
        {
            get { return _addressCoordinate; }
            set { _addressCoordinate = value; }
        }

        /// <summary>
        /// Define forbidden address's Radius
        /// </summary>
        public IRadius Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public InvalidAddressFilter()
        {
            //this.Node = node;
        }

        #region IDataTrackProcess

        public bool Process(TrackItem trackItem)
        {
            var socketLog = new Logging();
            try
            {
                ShapeMethods sm = new ShapeMethods();

                Double dis = sm.GetDistance(trackItem.InvalidAddress[0].Latitude, trackItem.InvalidAddress[0].Longitude, Convert.ToDouble(trackItem.Latitude), Convert.ToDouble(trackItem.longitude));
                socketLog.WriteLog(String.Format("Distance:{0}", dis));
                if (sm.PointInAddress(trackItem.InvalidAddress, Convert.ToDouble(trackItem.Latitude), Convert.ToDouble(trackItem.longitude)))
                {
                    socketLog.WriteLog(String.Format("Enter InValid Address:Entered.({0}, {1}).", trackItem.Latitude, trackItem.longitude));
                    trackItem.InNonDeliverable = true;
                }
                else
                {
                    socketLog.WriteLog(String.Format("Enter InValid Address:No Entered.({0}, {1}).", trackItem.Latitude, trackItem.longitude));
                    trackItem.InNonDeliverable = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                socketLog.WriteLog("Enter Invalid Address Filter Error:{0}" + ex);
                return false;
            }
        }

        #endregion
    }
}
