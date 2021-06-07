using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Common
{
    [Serializable]
    public class Configuration
    {
        public String UniqueId { get; set; }
        public String NickName { get; set; }
        public String IP1 { get; set; }
        public String Port1 { get; set; }
        public String IP2 { get; set; }
        public String Port2 { get; set; }
        public String Smsgate1 { get; set; }
        public String Smsgate2 { get; set; }
        public String LocationByCall { get; set; }
        public String Geo { get; set; }
        public String GeoLongitude { get; set; }
        public String GeoLatitude { get; set; }
        public String GeoRadius { get; set; }
        public String GeoCheckInterval { get; set; }
        public String Area1 { get; set; }
        public String Area1Longitude { get; set; }
        public String Area1Latitude { get; set; }
        public String Area1Radius { get; set; }
        public String Area1CheckInterval { get; set; }
        public String Area2 { get; set; }
        public String Area2Longitude { get; set; }
        public String Area2Latitude { get; set; }
        public String Area2Radius { get; set; }
        public String Area2CheckInterval { get; set; }
        public String Area3 { get; set; }
        public String Area3Longitude { get; set; }
        public String Area3Latitude { get; set; }
        public String Area3Radius { get; set; }
        public String Area3CheckInterval { get; set; }
        public String Area4 { get; set; }
        public String Area4Longitude { get; set; }
        public String Area4Latitude { get; set; }
        public String Area4Radius { get; set; }
        public String Area4CheckInterval { get; set; }
        public String TriStartTime { get; set; }
        public String TriEndTime { get; set; }
        public String TriSendInterval { get; set; }
        public String TriGpsMeasurementInterval { get; set; }
        public String PowerKeyEnable { get; set; }
        public String FunctionKeyEnable { get; set; }
        public String FunctionKeyMode { get; set; }
        public String Reserved { get; set; }
        public String MovementDeteMode { get; set; }
        public String MoveSpeed { get; set; }
        public String MoveDistance { get; set; }
        public String MoveNum { get; set; }
        public String FullChargeBoot { get; set; }
        public String SpeedWarmEnable { get; set; }
        public String SpeedWarmValue { get; set; }

    }
}
