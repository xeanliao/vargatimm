using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class AtGtrtoBo : BaseSendBo
    {
        public AtGtrto AtGtrtoModel { get; private set; }

        public AtGtrtoBo()
        {
            AtGtrtoModel = new AtGtrto();
        }

        public AtGtrtoBo(AtGtrto atGtrtoModel)
        {
            AtGtrtoModel = atGtrtoModel;
        }

        public override void Send()
        {
                
        }

        /// <summary>
        /// Build AT+GTRTO Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            var curCommandOption = Enum.Format(typeof(CommEnums.CommandOption), AtGtrtoModel.CommandOption, "d").ToString();
            if (curCommandOption == "10") { curCommandOption = "A"; }

            commandString.Append(AtGtrtoModel.CommandName + AtGtrtoModel.Password + "," + curCommandOption + "," + Enum.Format(typeof(CommEnums.SpeedWarnEnable), AtGtrtoModel.SpeedWarmEnable, "d").ToString() + "," + AtGtrtoModel.SpeedWarmValue + "," + AtGtrtoModel.SendTime + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}
