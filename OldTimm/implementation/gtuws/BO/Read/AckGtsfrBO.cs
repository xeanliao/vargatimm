using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class AckGtsfrBO : BaseReadBo
    {
        public AckGtsfr AckGtsfrModel { get; private set; }

        public AckGtsfrBO()
        {
            AckGtsfrModel = new AckGtsfr();
        }

        public AckGtsfrBO(AckGtsfr ackGtsfrModel)
        {
            AckGtsfrModel = ackGtsfrModel;
        }

        public override void Read()
        {
        }

        /// <summary>
        /// Resolve +ACK:GTSFR Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AckGtsfr Object</returns>
        public AckGtsfr ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AckGtsfrModel.CommandName = dataRead[0];
            AckGtsfrModel.UniqueID = dataRead[1];
            AckGtsfrModel.SendTime = dataRead[2];
            AckGtsfrModel.ACKTime = dataRead[3];
            AckGtsfrModel.CountNum = dataRead[4];
            AckGtsfrModel.Ver = dataRead[5];
            return AckGtsfrModel;
        }
                
    }
}
