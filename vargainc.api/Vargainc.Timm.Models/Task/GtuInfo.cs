using System;
using System.ComponentModel;

namespace Vargainc.Timm.Models
{
    /// <summary>
    /// to use Ramer–Douglas–Peucker algorithm to reduce gtuinfo, let the GtuInfo hiberate from Location.
    /// </summary>
    public class GtuInfo : Location
    {
        #region these property is direct used by T-SQL don't remove modified by jacob.chen
        public string UserColor { get; set; }
        //public double Latitude { get; set; }
        //public double Longitude { get; set; }
        #endregion
        [DefaultValue(0)]
        public long? Id
        {
            get;
            set;
        }
        [DefaultValue(0.0)]
        public double? dwSpeed
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? nHeading
        {
            get;
            set;
        }
        public DateTime? dtSendTime
        {
            get;
            set;
        }
        public DateTime? dtReceivedTime
        {
            get;
            set;
        }
        public string sIPAddress
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? nAreaCode
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? nNetworkCode
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? nCellID
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? nGPSFix
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? nAccuracy
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? nCount
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? nLocationID
        {
            get;
            set;
        }
        public string sVersion
        {
            get;
            set;
        }
        [DefaultValue(0.0)]
        public double? dwAltitude
        {
            get;
            set;
        }
        [DefaultValue(0.0)]
        public double? dwLatitude
        {
            get;
            set;
        }
        [DefaultValue(0.0)]
        public double? dwLongitude
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? PowerInfo
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? TaskgtuinfoId
        {
            get;
            set;
        }
        public string Code
        {
            get;
            set;
        }
        [DefaultValue(0)]
        public int? Status
        {
            get;
            set;
        }
        [DefaultValue(0.0)]
        public double? Distance
        {
            get;
            set;
        }
        public string GtuUniqueID { get; set; }
        public virtual TaskGtuInfoMapping TaskGtuInfoMapping { get; set; }
    }
}
