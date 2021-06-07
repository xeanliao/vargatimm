using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtgcrBo : BaseReadBo
    {
        public RespGtgcr RespGtgcrModel { get; private set; }

        public RespGtgcrBo()
        {
            RespGtgcrModel = new RespGtgcr();
        }

        public RespGtgcrBo(RespGtgcr respGtgcrModel)
        {
            RespGtgcrModel = respGtgcrModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTGCR Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtgcr Object</returns>
        public RespGtgcr ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtgcrModel.CommandName = dataRead[0];
            RespGtgcrModel.UniqueID = dataRead[1];
            RespGtgcrModel.GpsFix = dataRead[2];
            RespGtgcrModel.GpsAccuracy = dataRead[3];
            RespGtgcrModel.Longitude = dataRead[4];
            RespGtgcrModel.Latitude = dataRead[5];
            RespGtgcrModel.Radius = dataRead[6];
            RespGtgcrModel.CheckInterval = dataRead[7];
            RespGtgcrModel.GeofenceType = dataRead[8];
            RespGtgcrModel.SendTime = dataRead[9];
            RespGtgcrModel.Mcc = dataRead[10];
            RespGtgcrModel.Mnc = dataRead[11];
            RespGtgcrModel.Lac = dataRead[12];
            RespGtgcrModel.Cellid = dataRead[13];
            RespGtgcrModel.Ta = dataRead[14];
            RespGtgcrModel.CountNum = dataRead[15];
            RespGtgcrModel.Ver = dataRead[16];
            return RespGtgcrModel;
        }
    }
}
