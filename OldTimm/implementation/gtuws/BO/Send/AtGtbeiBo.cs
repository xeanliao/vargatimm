using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class AtGtbeiBo : BaseSendBo
    {
        public AtGtbei AtGtbeiModel { get; private set; }

        public AtGtbeiBo()
        {
            AtGtbeiModel = new AtGtbei();
        }

        public AtGtbeiBo(AtGtbei atGtbeiModel)
        {
            AtGtbeiModel = atGtbeiModel;
        }

        public override void Send()
        {
                
        }

        /// <summary>
        /// Build AT+GTBEI Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(AtGtbeiModel.CommandName + AtGtbeiModel.Password + "," + AtGtbeiModel.Reserved1 + "," + AtGtbeiModel.Reserved2 + "," + Enum.Format(typeof(CommEnums.LocationByCall), AtGtbeiModel.LocationByCall, "d") + "," + AtGtbeiModel.SecondServerIp + "," + AtGtbeiModel.SecondServerPort + "," + AtGtbeiModel.SecondSmsGateway + "," + AtGtbeiModel.SendTime + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}
