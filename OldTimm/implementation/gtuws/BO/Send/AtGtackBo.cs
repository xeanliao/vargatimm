using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.ModelLayer.Common;
using GTU.Utilities.Base;

namespace GTU.BusinessLayer.Send
{
    public class AtGtackBo : BaseSendBo
    {
        public AtGtack AtGtackModel { get; private set; }

        public AtGtackBo()
        {
            AtGtackModel = new AtGtack();
        }

        public AtGtackBo(AtGtack atGtackModel)
        {
            AtGtackModel = atGtackModel;
        }

        public override void Send()
        {
            
        }

        /// <summary>
        /// Build AT+GTACK Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(AtGtackModel.CommandName + AtGtackModel.Password + "," + Enum.Format(typeof(CommEnums.AckType), AtGtackModel.AckType, "d") + AtGtackModel.AckValue + "," + AtGtackModel.SendTime + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}
