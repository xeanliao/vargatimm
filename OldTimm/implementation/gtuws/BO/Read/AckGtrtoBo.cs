using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class AckGtrtoBo : BaseReadBo
    {
        public AckGtrto AckGtrtoModel { get; private set; }

        public AckGtrtoBo()
        {
            AckGtrtoModel = new AckGtrto();
        }

        public AckGtrtoBo(AckGtrto ackGtrtoModel)
        {
            AckGtrtoModel = ackGtrtoModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +ACK:GTRTO Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AckGtrto Object</returns>
        public AckGtrto ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AckGtrtoModel.CommandName = dataRead[0];
            AckGtrtoModel.UniqueID = dataRead[1];
            AckGtrtoModel.Option = dataRead[2];
            AckGtrtoModel.SendTime = dataRead[3];
            AckGtrtoModel.ACKTime = dataRead[4];
            AckGtrtoModel.CountNum = dataRead[5];
            return AckGtrtoModel;
        }
    }
}
