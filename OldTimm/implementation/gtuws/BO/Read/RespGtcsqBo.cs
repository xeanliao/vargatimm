using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class RespGtcsqBo : BaseReadBo
    {
        public RespGtcsq RespGtcsqModel { get; private set; }

        public RespGtcsqBo()
        {
            RespGtcsqModel = new RespGtcsq();
        }

        public RespGtcsqBo(RespGtcsq respGtcsqModel)
        {
            RespGtcsqModel = respGtcsqModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +RESP:GTCSQ Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>RespGtcsq Object</returns>
        public RespGtcsq ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            RespGtcsqModel.CommandName = dataRead[0];
            RespGtcsqModel.UniqueID = dataRead[1];
            RespGtcsqModel.Content1 = dataRead[2];
            RespGtcsqModel.Content2 = dataRead[3];
            RespGtcsqModel.SendTime = dataRead[4];
            RespGtcsqModel.CountNum = dataRead[5];
            return RespGtcsqModel;
        }
    }
}
