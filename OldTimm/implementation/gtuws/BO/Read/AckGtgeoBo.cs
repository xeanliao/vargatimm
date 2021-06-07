using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class AckGtgeoBo : BaseReadBo
    {
        public AckGtgeo AckGtgeoModel { get; private set; }

        public AckGtgeoBo()
        {
            AckGtgeoModel = new AckGtgeo();
        }

        public AckGtgeoBo(AckGtgeo ackGtgeoModel)
        {
            AckGtgeoModel = ackGtgeoModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve +ACK:GTGEO Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AckGtgeo Object</returns>
        public AckGtgeo ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AckGtgeoModel.CommandName = dataRead[0];
            AckGtgeoModel.UniqueID = dataRead[1];
            AckGtgeoModel.GeofenceId = dataRead[2];
            AckGtgeoModel.SendTime = dataRead[3];
            AckGtgeoModel.ACKTime = dataRead[4];
            AckGtgeoModel.CountNum = dataRead[5];
            AckGtgeoModel.Ver = dataRead[6];
            return AckGtgeoModel;
        }
    }
}
