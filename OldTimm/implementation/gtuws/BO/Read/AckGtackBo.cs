using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;


namespace GTU.BusinessLayer.Read
{
    public class AckGtackBo : BaseReadBo
    {
        public AckGtack AckGtackModel { get; private set; }

        public AckGtackBo()
        {
            AckGtackModel = new AckGtack();
        }

        public AckGtackBo(AckGtack ackGtackModel)
        {
            AckGtackModel = ackGtackModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +ACK:GTACK Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AckGtack Object</returns>
        public AckGtack ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AckGtackModel.CommandName = dataRead[0];
            AckGtackModel.UniqueID = dataRead[1];
            AckGtackModel.SendTime = dataRead[2];
            AckGtackModel.ACKTime = dataRead[3];
            AckGtackModel.CountNum = dataRead[4];
            AckGtackModel.Ver = dataRead[5];
            return AckGtackModel;
        }
    }
}
