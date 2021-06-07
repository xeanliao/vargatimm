using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class AckGttriBo : BaseReadBo
    {
        public AckGttri AckGttriModel { get; private set; }

        public AckGttriBo()
        {
            AckGttriModel = new AckGttri();
        }

        public AckGttriBo(AckGttri ackGttriModel)
        {
            AckGttriModel = ackGttriModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +ACK:GTTRI Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AckGttri Object</returns>
        public AckGttri ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AckGttriModel.CommandName = dataRead[0];
            AckGttriModel.UniqueID = dataRead[1];
            AckGttriModel.SendTime = dataRead[2];
            AckGttriModel.ACKTime = dataRead[3];
            AckGttriModel.CountNum = dataRead[4];
            return AckGttriModel;
        }
    }
}
