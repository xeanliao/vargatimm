using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.Utilities.Logging;

namespace GTU.DataLayer.Track
{
    public class TrackItemDal
    {
        private GTUTrackDataContext _dbContext = null;

        public TrackItemDal()
        {
            _dbContext = DataContext.CreateDataContext();
        }

        public void Save(GTU.ModelLayer.Track.TrackItem trackItemModel)
        {
            var socketLog = new Logging();
            try
            {
                    TrackItem tm = new TrackItem();
                    tm.TrackId = trackItemModel.TrackId;
                    tm.UniqueId = trackItemModel.UniqueId;
                    tm.StayAlarm = trackItemModel.StayAlarm;
                    tm.SendTime = trackItemModel.SendTime;
                    tm.longitude = trackItemModel.longitude;
                    tm.Latitude = trackItemModel.Latitude;
                    tm.InNonDeliverable = trackItemModel.InNonDeliverable;
                    tm.InDeliverable = trackItemModel.InDeliverable;
                    _dbContext.TrackItems.InsertOnSubmit(tm);
                    _dbContext.SubmitChanges();
                
            }
            catch (Exception ex)
            {
                socketLog.WriteLog(String.Format("Data Save Error:{0}",ex));   
            }
        }

        public IList<TrackItem> GetTrackRecordsByIdAndTimePeriod(String uniqueId, DateTime checkDate, bool outTime)
        {
            var socketLog = new Logging();
            try
            {
                if (outTime)
                {
                    var trackItems = _dbContext.TrackItems.Where(t => t.UniqueId == uniqueId && t.SendTime <= checkDate).ToList();
                    return trackItems;
                }
                else
                {
                    var trackItems = _dbContext.TrackItems.Where(t => t.UniqueId == uniqueId && t.SendTime > checkDate).ToList();
                    return trackItems;
                }
            }
            catch (Exception ex)
            {
                socketLog.WriteLog(String.Format("Data Save Error:{0}", ex));
                return null;
            }
        }
    }
}
