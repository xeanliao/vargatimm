using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtlbcBo : BaseReadBo
    {
        public RespGtlbc RespGtlbcModel { get; private set; }

        public RespGtlbcBo()
        {
            RespGtlbcModel = new RespGtlbc();
        }

        public RespGtlbcBo(RespGtlbc respGtlbcModel)
        {
            RespGtlbcModel = respGtlbcModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTLBC Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtlbc Object</returns>
        public RespGtlbc ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);
            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtlbcModel.CommandName = dataRead[0];
            RespGtlbcModel.UniqueID = dataRead[1];
            RespGtlbcModel.CallingNumber = dataRead[2];
            RespGtlbcModel.GpsFix = dataRead[3];
            RespGtlbcModel.Speed = dataRead[4];
            RespGtlbcModel.Heading = dataRead[5];
            RespGtlbcModel.Altitude = dataRead[6];
            RespGtlbcModel.GpsAccuracy = dataRead[7];
            RespGtlbcModel.Longitude = dataRead[8];
            RespGtlbcModel.Latitude = dataRead[9];
            RespGtlbcModel.SendTime = dataRead[10];
            RespGtlbcModel.Mcc = dataRead[11];
            RespGtlbcModel.Mnc = dataRead[12];
            RespGtlbcModel.Lac = dataRead[13];
            RespGtlbcModel.Cellid = dataRead[14];
            RespGtlbcModel.Ta = dataRead[15];
            RespGtlbcModel.CountNum = dataRead[16];
            return RespGtlbcModel;
        }
    }
}
