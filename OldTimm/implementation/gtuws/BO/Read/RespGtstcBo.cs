using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtstcBo : BaseReadBo
    {
        public RespGtstc RespGtstcModel { get; private set; }

        public RespGtstcBo()
        {
            RespGtstcModel = new RespGtstc();
        }

        public RespGtstcBo(RespGtstc respGtstcModel)
        {
            RespGtstcModel = respGtstcModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTSTC Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtstc Object</returns>
        public RespGtstc ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtstcModel.CommandName = dataRead[0];
            RespGtstcModel.UniqueID = dataRead[1];
            RespGtstcModel.SendTime = dataRead[2];
            RespGtstcModel.CountNum = dataRead[3];
            return RespGtstcModel;
        }
    }
}
