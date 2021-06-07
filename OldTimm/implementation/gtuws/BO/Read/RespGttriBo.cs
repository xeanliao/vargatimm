using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGttriBo : BaseReadBo
    {
        public RespGttri RespGttriModel { get; set; }

        public RespGttriBo()
        {
            RespGttriModel = new RespGttri();
        }

        public RespGttriBo(RespGttri respGttriModel)
        {
            RespGttriModel = respGttriModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTTRI Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGttri Object</returns>
        public RespGttri ResolveCommand(Byte[] byteStr)
        {
            //RespGttri RespGttriModel = new RespGttri();
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGttriModel.CommandName = dataRead[0];
            RespGttriModel.UniqueID = dataRead[1];
            RespGttriModel.Number = dataRead[2];
            RespGttriModel.Reserved1 = dataRead[3];
            RespGttriModel.Reserved2 = dataRead[4];
            RespGttriModel.GpsFix = dataRead[5];
            RespGttriModel.Speed = dataRead[6];
            RespGttriModel.Heading = dataRead[7];
            RespGttriModel.Altitude = dataRead[8];
            RespGttriModel.GpsAccuracy = dataRead[9];
            RespGttriModel.Longitude = dataRead[10];
            RespGttriModel.Latitude = dataRead[11];
            RespGttriModel.SendTime = dataRead[12];
            RespGttriModel.Mcc = dataRead[13];
            RespGttriModel.Mnc = dataRead[14];
            RespGttriModel.Lac = dataRead[15];
            RespGttriModel.Cellid = dataRead[16];
            RespGttriModel.Ta = dataRead[17];
            RespGttriModel.CountNum = dataRead[18];
            return RespGttriModel;
        }
    }
}
