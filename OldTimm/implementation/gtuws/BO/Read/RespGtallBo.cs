using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtallBo : BaseReadBo
    {
        public RespGtall RespGtallModel { get; private set; }

        public RespGtallBo()
        {
            RespGtallModel = new RespGtall();
        }

        public RespGtallBo(RespGtall respGtallModel)
        {
            RespGtallModel = respGtallModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTALL Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtall Object</returns>
        public RespGtall ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtallModel.CommandName = dataRead[0];
            RespGtallModel.ConfigurationContent.UniqueId = dataRead[1];
            RespGtallModel.ConfigurationContent.NickName = dataRead[2];
            RespGtallModel.ConfigurationContent.IP1 = dataRead[3];
            RespGtallModel.ConfigurationContent.Port1 = dataRead[4];
            RespGtallModel.ConfigurationContent.IP2 = dataRead[5];
            RespGtallModel.ConfigurationContent.Port2 = dataRead[6];
            RespGtallModel.ConfigurationContent.Smsgate1 = dataRead[7];
            RespGtallModel.ConfigurationContent.Smsgate2 = dataRead[8];
            RespGtallModel.ConfigurationContent.LocationByCall = dataRead[9];
            RespGtallModel.ConfigurationContent.Geo = dataRead[10];
            RespGtallModel.ConfigurationContent.GeoLongitude = dataRead[11];
            RespGtallModel.ConfigurationContent.GeoLatitude = dataRead[12];
            RespGtallModel.ConfigurationContent.GeoRadius = dataRead[13];
            RespGtallModel.ConfigurationContent.GeoCheckInterval = dataRead[14];
            RespGtallModel.ConfigurationContent.Area1 = dataRead[15];
            RespGtallModel.ConfigurationContent.Area1Longitude = dataRead[16];
            RespGtallModel.ConfigurationContent.Area1Latitude = dataRead[17];
            RespGtallModel.ConfigurationContent.Area1Radius = dataRead[18];
            RespGtallModel.ConfigurationContent.Area1CheckInterval = dataRead[19];
            RespGtallModel.ConfigurationContent.Area2 = dataRead[20];
            RespGtallModel.ConfigurationContent.Area2Longitude = dataRead[21];
            RespGtallModel.ConfigurationContent.Area2Latitude = dataRead[22];
            RespGtallModel.ConfigurationContent.Area2Radius = dataRead[23];
            RespGtallModel.ConfigurationContent.Area2CheckInterval = dataRead[24];
            RespGtallModel.ConfigurationContent.Area3 = dataRead[25];
            RespGtallModel.ConfigurationContent.Area3Longitude = dataRead[26];
            RespGtallModel.ConfigurationContent.Area3Latitude = dataRead[27];
            RespGtallModel.ConfigurationContent.Area3Radius = dataRead[28];
            RespGtallModel.ConfigurationContent.Area3CheckInterval = dataRead[29];
            RespGtallModel.ConfigurationContent.Area4 = dataRead[30];
            RespGtallModel.ConfigurationContent.Area4Longitude = dataRead[31];
            RespGtallModel.ConfigurationContent.Area4Latitude = dataRead[32];
            RespGtallModel.ConfigurationContent.Area4Radius = dataRead[33];
            RespGtallModel.ConfigurationContent.Area4CheckInterval = dataRead[34];
            RespGtallModel.ConfigurationContent.TriStartTime = dataRead[35];
            RespGtallModel.ConfigurationContent.TriEndTime = dataRead[36];
            RespGtallModel.ConfigurationContent.TriSendInterval = dataRead[37];
            RespGtallModel.ConfigurationContent.TriGpsMeasurementInterval = dataRead[38];
            RespGtallModel.ConfigurationContent.PowerKeyEnable = dataRead[39];
            RespGtallModel.ConfigurationContent.FunctionKeyEnable = dataRead[40];
            RespGtallModel.ConfigurationContent.FunctionKeyMode = dataRead[41];
            RespGtallModel.ConfigurationContent.Reserved = dataRead[42];
            RespGtallModel.ConfigurationContent.MovementDeteMode = dataRead[43];
            RespGtallModel.ConfigurationContent.MoveSpeed = dataRead[44];
            RespGtallModel.ConfigurationContent.MoveDistance = dataRead[45];
            RespGtallModel.ConfigurationContent.MoveNum = dataRead[46];
            RespGtallModel.ConfigurationContent.FullChargeBoot = dataRead[47];
            RespGtallModel.ConfigurationContent.SpeedWarmEnable = dataRead[48];
            RespGtallModel.ConfigurationContent.SpeedWarmValue = dataRead[49];
            RespGtallModel.SendTime = dataRead[50];
            RespGtallModel.CountNum = dataRead[51];
            return RespGtallModel;
        }
    }
}
