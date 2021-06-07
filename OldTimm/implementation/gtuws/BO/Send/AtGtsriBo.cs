using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class AtGtsriBo : BaseSendBo
    {
        public AtGtsri AtGtsriModel { get; private set; }

        public AtGtsriBo()
        {
            AtGtsriModel = new AtGtsri();
        }

        public AtGtsriBo(AtGtsri atGtsriModel)
        {
            AtGtsriModel = atGtsriModel;
        }

        public override void Send()
        {
         
        }

        /// <summary>
        /// Build AT+GTSRI Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(AtGtsriModel.CommandName + AtGtsriModel.Password + ","
                                + Enum.Format(typeof(CommEnums.ReportMode), AtGtsriModel.ReportMode, "d").ToString() + ","
                                + Enum.Format(typeof(CommEnums.ActiveSession), AtGtsriModel.ActiveSession, "d").ToString() + ","
                                + AtGtsriModel.Apn + ","
                                + AtGtsriModel.ApnUserName + ","
                                + AtGtsriModel.ApnUserPassword + ","
                                + AtGtsriModel.MainServerIP + ","
                                + AtGtsriModel.MainServerPort + ","
                                + AtGtsriModel.MainSMSGateway + ","
                                + AtGtsriModel.SendTime + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}

