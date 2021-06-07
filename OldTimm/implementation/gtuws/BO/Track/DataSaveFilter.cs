using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.ModelLayer.Common;
using GTU.Utilities.Logging;

namespace GTU.BusinessLayer.Track
{
    public class DataSaveFilter : IDataTrackProcess
    {
        //public IDataTrackProcess Node
        //{
        //    get;
        //    set;
        //}

        public DataSaveFilter()
        {
            //this.Node = node;
        }

        #region IDataTrackProcess Members

        public bool Process(TrackItem trackItem)
        {
            var socketLog = new Logging();
            try
            {
                //Save Track information
                var trackItemBo = new TrackItemBo();
                trackItemBo.TrackItemModel = trackItem;
                trackItemBo.Save();
                return true;
            }
            catch (Exception ex)
            {
                socketLog.WriteLog(String.Format("Save Data Error:{0}",ex));
                return false;
            }
            
        }

        #endregion
    }

    //public IDataTrackProcess<TrackItemBo> Node
    //    {
    //        get;
    //        set;
    //    }

    //    public DataSaveFilter(IDataTrackProcess<TrackItemBo> node)
    //    {
    //        this.Node = node;
    //    }

    //    #region IDataTrackProcess<TrackItemBo>

    //    public bool Process(ref TrackItemBo trackItemBo)
    //    {
    //        var socketLog = new Logging();
    //        socketLog.LogIf = System.Configuration.ConfigurationSettings.AppSettings["logif"].ToString();
    //        socketLog.LogPath = System.Configuration.ConfigurationSettings.AppSettings["logPath"].ToString();
    //        socketLog.WriteLog("OutofDeliverable is true!");
    //        //Save Track information
    //        trackItemBo.TrackItemModel.InNonDeliverable = true;
    //        //trackItem.DefaultView.RowFilter = "id =" + this.FilterID.ToString();
    //        //dataSource = dataSource.DefaultView.ToTable();
    //        trackItemBo.Save();
    //        return true;
    //    }

    //    #endregion
    //}

    

}
