using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.ModelLayer.Common;
using GTU.Utilities.Logging;


namespace GTU.BusinessLayer.Track
{
    //public class UnvalidAreaFilter : IDataTrackProcess
    //{
    //    //public IDataTrackProcess Node
    //    //{
    //    //    get;
    //    //    set;
    //    //}

    //    private List<ICoordinate> _validCoordinates;

    //    /// <summary>
    //    /// Define Valid Area
    //    /// </summary>
    //    public List<ICoordinate> ValidCoordinates
    //    {
    //        get { return _validCoordinates; }
    //        set { _validCoordinates = value; }
    //    }

    //    public UnvalidAreaFilter()
    //    {
    //        //this.Node = node;
    //        this.ValidCoordinates = null;
    //    }

    //    #region IDataTrackProcess

    //    public bool Process()
    //    {
    //        //TrackItemBo trackItemBo = param as TrackItemBo;
    //        var socketLog = new Logging();
    //        socketLog.LogIf = System.Configuration.ConfigurationSettings.AppSettings["logif"].ToString();
    //        socketLog.LogPath = System.Configuration.ConfigurationSettings.AppSettings["logPath"].ToString();
    //        socketLog.WriteLog("OutofDeliverable is true!");

    //        //判断是否在指定监控区域内
    //        //trackItemBo.TrackItemModel.OutofDeliverable = true;
    //        //trackItem.DefaultView.RowFilter = "id =" + this.FilterID.ToString();
    //        //dataSource = dataSource.DefaultView.ToTable();
            
    //        return true;
    //    }

    //    #endregion
    //}


    public class ValidAreaFilter : IDataTrackProcess
    {
        //public IDataTrackProcess Node
        //{
        //    get;
        //    set;
        //}

        //private List<ICoordinate> _validCoordinates;

        /// <summary>
        /// Define Valid Area
        /// </summary>
        //public List<ICoordinate> ValidCoordinates
        //{
        //    get { return _validCoordinates; }
        //    set { _validCoordinates = value; }
        //}

        //public UnvalidAreaFilter(IDataTrackProcess node)
        public ValidAreaFilter()
        {
            //this.Node = node;
        }

        #region IDataTrackProcess<TrackItem>

        public bool Process(TrackItem trackItem)
        {
            var socketLog = new Logging();
            try
            {
                ShapeMethods sm = new ShapeMethods();

                if (sm.PointInPolygons(trackItem.TrackArea, Convert.ToDouble(trackItem.Latitude), Convert.ToDouble(trackItem.longitude)))
                {
                    socketLog.WriteLog(String.Format("Valid Area:OK.({0}, {1}).", trackItem.Latitude, trackItem.longitude));
                    trackItem.InDeliverable = true;
                }
                else
                {
                    socketLog.WriteLog(String.Format("Valid Area:Wrong.({0}, {1}) is out of valid area.", trackItem.Latitude, trackItem.longitude));
                    trackItem.InDeliverable = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                socketLog.WriteLog("Valid Area Filter Error:{0}" + ex);
                return false;
            }
            
        }

        #endregion
    }
}
