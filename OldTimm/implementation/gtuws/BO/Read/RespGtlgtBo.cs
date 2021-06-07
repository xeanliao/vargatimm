using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtlgtBo : BaseReadBo
    {
        public RespGtlgt RespGtlgtModel { get; private set; }

        public RespGtlgtBo()
        {
            RespGtlgtModel = new RespGtlgt();
        }

        public RespGtlgtBo(RespGtlgt respGtlgtModel)
        {
            RespGtlgtModel = respGtlgtModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTLGT Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtlgt Object</returns>
        public RespGtlgt ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtlgtModel.CommandName = dataRead[0];
            RespGtlgtModel.UniqueID = dataRead[1];
            RespGtlgtModel.Content = dataRead[2];
            RespGtlgtModel.SendTime = dataRead[3];
            RespGtlgtModel.CountNum = dataRead[4];
            return RespGtlgtModel;
        }
    }
}
