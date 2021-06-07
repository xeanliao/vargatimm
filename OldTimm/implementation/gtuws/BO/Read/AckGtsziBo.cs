using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class AckGtsziBo : BaseReadBo
    {
        public AckGtszi AckGtsziModel { get; private set; }

        public AckGtsziBo()
        {
            AckGtsziModel = new AckGtszi();
        }

        public AckGtsziBo(AckGtszi ackGtsziModel)
        {
            AckGtsziModel = ackGtsziModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +ACK:GTSZI Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AckGtszi Object</returns>
        public AckGtszi ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AckGtsziModel.CommandName = dataRead[0];
            AckGtsziModel.UniqueID = dataRead[1];
            AckGtsziModel.ZoneId = dataRead[2];
            AckGtsziModel.SendTime = dataRead[3];
            AckGtsziModel.ACKTime = dataRead[4];
            AckGtsziModel.CountNum = dataRead[5];
            return AckGtsziModel;
        }
    }
}
