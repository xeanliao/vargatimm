using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class AtGtsziBo : BaseSendBo
    {
        public AtGtszi AtGtsziModel { get; private set; }

        public AtGtsziBo()
        {
            AtGtsziModel = new AtGtszi();
        }

        public AtGtsziBo(AtGtszi atGtsziModel)
        {
            AtGtsziModel = atGtsziModel;
        }

        public override void Send()
        {
                
        }

        /// <summary>
        /// Build AT+GTSZI Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(AtGtsziModel.CommandName + AtGtsziModel.Password + ","
                                + AtGtsziModel.ZoneId + ","
                                + AtGtsziModel.Longitude + ","
                                + AtGtsziModel.Latitude + ","
                                + AtGtsziModel.Radius + ","
                                + AtGtsziModel.CheckInterval + ","
                                + AtGtsziModel.SendTime + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}
