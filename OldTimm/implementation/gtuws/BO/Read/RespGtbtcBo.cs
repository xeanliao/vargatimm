using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtbtcBo : BaseReadBo
    {
        public RespGtbtc RespGtbtcModel { get; private set; }

        public RespGtbtcBo()
        {
            RespGtbtcModel = new RespGtbtc();
        }

        public RespGtbtcBo(RespGtbtc respGtbtcModel)
        {
            RespGtbtcModel = respGtbtcModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTBTC Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtbtc Object</returns>
        public RespGtbtc ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtbtcModel.CommandName = dataRead[0];
            RespGtbtcModel.UniqueID = dataRead[1];
            RespGtbtcModel.SendTime = dataRead[2];
            RespGtbtcModel.CountNum = dataRead[3];
            return RespGtbtcModel;
        }
    }
}
