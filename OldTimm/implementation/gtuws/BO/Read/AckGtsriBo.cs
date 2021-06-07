using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class AckGtsriBo: BaseReadBo
    {
        public AckGtsri AckGtsriModel { get; private set; }

        public AckGtsriBo()
        {
            AckGtsriModel = new AckGtsri();
        }

        public AckGtsriBo(AckGtsri ackGtsriModel)
        {
            AckGtsriModel = ackGtsriModel;
        }

        public override void Read()
        {
            
        }

        /// <summary>
        /// Resolve +ACK:GTSRI Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AckGtsri Object</returns>
        public AckGtsri ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AckGtsriModel.CommandName = dataRead[0];
            AckGtsriModel.UniqueID = dataRead[1];
            AckGtsriModel.SendTime = dataRead[2];
            AckGtsriModel.ACKTime = dataRead[3];
            AckGtsriModel.CountNum = dataRead[4];
            return AckGtsriModel;
        }

    }
}
