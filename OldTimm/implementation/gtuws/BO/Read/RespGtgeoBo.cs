using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtgeoBo: BaseReadBo
    {
        public RespGtgeo RespGtgeoModel { get; private set; }

        public RespGtgeoBo()
        {
            RespGtgeoModel = new RespGtgeo();
        }

        public RespGtgeoBo(RespGtgeo respGtgeoModel)
        {
            RespGtgeoModel = respGtgeoModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTGEO Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtgeo Object</returns>
        public RespGtgeo ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtgeoModel.CommandName = dataRead[0];
            RespGtgeoModel.UniqueID = dataRead[1];
            RespGtgeoModel.Active = dataRead[2];
            RespGtgeoModel.SendTime = dataRead[3];
            RespGtgeoModel.CountNum = dataRead[4];
            return RespGtgeoModel;
        }
    }
}
