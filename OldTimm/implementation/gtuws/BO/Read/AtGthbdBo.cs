using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Read;
using GTU.Utilities.Base;
using GTU.Utilities.DataConvert;

namespace GTU.BusinessLayer.Read
{
    public class AtGthbdBo : BaseReadBo
    {
        public AtGthbd AtGthbdModel { get; private set; }

        public AtGthbdBo()
        {
            AtGthbdModel = new AtGthbd();
        }

        public AtGthbdBo(AtGthbd atGthbdModel)
        {
            AtGthbdModel = atGthbdModel;
        }

        public override void Read()
        {
                
        }

        /// <summary>
        /// Resolve AT+GTHBD=HeartBeat Command Data
        /// </summary>
        /// <param name="byteStr">Byte[] String recevied by Socket from Terminal</param>
        /// <returns>AtGthbd Object</returns>
        public AtGthbd ResolveCommand(Byte[] byteStr)
        {
            String strRead = DataConvert.FromASCIIByteArray(byteStr);
            strRead = DataConvert.ClearEndChar(strRead);

            String[] dataRead = DataConvert.SplitString(strRead, ",");
            AtGthbdModel.CommandName = dataRead[0];
            AtGthbdModel.Content = dataRead[1];
            AtGthbdModel.UniqueID = dataRead[2];
            AtGthbdModel.SendTime = dataRead[3];
            AtGthbdModel.CountNum = dataRead[4];
            return AtGthbdModel;
        }
    }
}
