using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtrtlBo : BaseReadBo
    {
        public RespGtrtl RespGtrtlModel { get; private set; }

        public RespGtrtlBo()
        {
            RespGtrtlModel = new RespGtrtl();
        }

        public RespGtrtlBo(RespGtrtl respGtrtlModel)
        {
            RespGtrtlModel = respGtrtlModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTRTL Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtrtl Object</returns>
        public RespGtrtl ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtrtlModel.CommandName = dataRead[0];
            RespGtrtlModel.UniqueID = dataRead[1];
            RespGtrtlModel.Number = dataRead[2];
            RespGtrtlModel.ZoneId = dataRead[3];
            RespGtrtlModel.ZoneAlert = dataRead[4];
            RespGtrtlModel.GpsFix = dataRead[5];
            RespGtrtlModel.Speed = dataRead[6];
            RespGtrtlModel.Heading = dataRead[7];
            RespGtrtlModel.Altitude = dataRead[8];
            RespGtrtlModel.GpsAccuracy = dataRead[9];
            RespGtrtlModel.Longitude = dataRead[10];
            RespGtrtlModel.Latitude = dataRead[11];
            RespGtrtlModel.SendTime = dataRead[12];
            RespGtrtlModel.Mcc = dataRead[13];
            RespGtrtlModel.Mnc = dataRead[14];
            RespGtrtlModel.Lac = dataRead[15];
            RespGtrtlModel.Cellid = dataRead[16];
            RespGtrtlModel.Ta = dataRead[17];
            RespGtrtlModel.CountNum = dataRead[18];
            return RespGtrtlModel;
        }
    }
}
