using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.ModelLayer.Common;
using GTU.Utilities.Logging;
using GTU.DataLayer.Track;
using GTU.DataLayer.Track;

namespace GTU.BusinessLayer.Track
{
    public class StayFilter : IDataTrackProcess
    {
        //public IDataTrackProcess Node
        //{
        //    get;
        //    set;
        //}

        //private int _stayPeriod;

        /// <summary>
        /// Define the alarm time (minutes)
        /// </summary>
        //public int StayPeriod
        //{
        //    get { return _stayPeriod; }
        //    set { _stayPeriod = value; }
        //}

        //public StayFilter(IDataTrackProcess node)
        //{
        //    this.Node = node;
        //}

        public StayFilter()
        {
            
        }


        #region IDataTrackProcess

        public bool Process(TrackItem trackItem)
        {
            var socketLog = new Logging();
            bool isMove = false;
            ShapeMethods sm = new ShapeMethods();
            try
            {
                if (trackItem != null)
                {
                    TrackItemDal trackItemDal = new TrackItemDal();
                    Double timeSpan = Convert.ToDouble(System.Configuration.ConfigurationSettings.AppSettings["StayPeriod"]);
                    timeSpan = -timeSpan;

                    if (trackItemDal.GetTrackRecordsByIdAndTimePeriod(trackItem.UniqueId, trackItem.SendTime.AddMinutes(timeSpan), true).Count > 0)
                    {
                        var trackHistories = trackItemDal.GetTrackRecordsByIdAndTimePeriod(trackItem.UniqueId, trackItem.SendTime.AddMinutes(timeSpan), false);
                        foreach (var track in trackHistories)
                        {
                            //socketLog.WriteLog(String.Format("move distance{0}", sm.GetDistance(Convert.ToDouble(track.Latitude), Convert.ToDouble(track.longitude), Convert.ToDouble(trackItem.Latitude), Convert.ToDouble(trackItem.longitude))));
                            if (sm.GetDistance(Convert.ToDouble(track.Latitude), Convert.ToDouble(track.longitude), Convert.ToDouble(trackItem.Latitude), Convert.ToDouble(trackItem.longitude)) > Convert.ToDouble(System.Configuration.ConfigurationSettings.AppSettings["StayRange"]))
                            {
                                isMove = true;
                            }
                        }

                        if (!isMove)
                        {
                            socketLog.WriteLog(String.Format("Stop moving for more than {0} minutes", System.Configuration.ConfigurationSettings.AppSettings["StayPeriod"]));
                        }

                    }
                }
                
                return true;

            }
            catch (Exception ex)
            {
                socketLog.WriteLog(ex.ToString());
                return false;
            }
        }

        #endregion
    }
}
