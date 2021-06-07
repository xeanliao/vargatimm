using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.ModelLayer.Common;
using GTU.Utilities.Logging;

namespace GTU.BusinessLayer.Track
{
    public class InvalidAreaFilter : IDataTrackProcess
    {
        public InvalidAreaFilter()
        {
            //this.Node = node;
        }

        #region IDataTrackProcess Members

        public bool Process(TrackItem trackItem)
        {
            var socketLog = new Logging();
            try
            {
                ShapeMethods sm = new ShapeMethods();

                if (sm.PointInPolygons(trackItem.InvalidArea, Convert.ToDouble(trackItem.Latitude), Convert.ToDouble(trackItem.longitude)))
                {
                    socketLog.WriteLog(String.Format("InValid Area:Wrong.({0}, {1}) enter the inhibitive area.", trackItem.Latitude, trackItem.longitude));
                    trackItem.InNonDeliverable = true;
                }
                else
                {
                    socketLog.WriteLog(String.Format("InValid Area:OK.({0}, {1}).", trackItem.Latitude, trackItem.longitude));
                    trackItem.InNonDeliverable = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                socketLog.WriteLog("Invalid Area Filter Error:{0}" + ex);
                return false;
            }
        }

        #endregion
    }

    
}
