using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class AckGtbeiBo : BaseReadBo
    {
        public AckGtbei AckGtbeiModel { get; private set; }

        public AckGtbeiBo()
        {
            AckGtbeiModel = new AckGtbei();
        }

        public AckGtbeiBo(AckGtbei ackGtbeiModel)
        {
            AckGtbeiModel = ackGtbeiModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +ACK:GTBEI Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AckGtbei Object</returns>
        public AckGtbei ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AckGtbeiModel.CommandName = dataRead[0];
            AckGtbeiModel.UniqueID = dataRead[1];
            AckGtbeiModel.SendTime = dataRead[2];
            AckGtbeiModel.ACKTime = dataRead[3];
            AckGtbeiModel.CountNum = dataRead[4];
            return AckGtbeiModel;
        }
    }
}
