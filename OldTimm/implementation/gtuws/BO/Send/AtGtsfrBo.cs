using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class AtGtsfrBo : BaseSendBo
    {
        public AtGtsfr AtGtsfrModel { get; private set; }

        public AtGtsfrBo()
        {
            AtGtsfrModel = new AtGtsfr();
        }

        public AtGtsfrBo(AtGtsfr atGtsfrModel)
        {
            AtGtsfrModel = atGtsfrModel;
        }

        public override void Send()
        {
                
        }

        /// <summary>
        /// Build AT+GTSFR Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(AtGtsfrModel.CommandName + AtGtsfrModel.Password + ","
                                + Enum.Format(typeof(CommEnums.PowerKeyEnable), AtGtsfrModel.PowerKeyEnable, "d").ToString() + ","
                                + Enum.Format(typeof(CommEnums.FunctionKeyEnable), AtGtsfrModel.FunctionKeyEnable, "d").ToString() + ","
                                + Enum.Format(typeof(CommEnums.FunctionKeyMode), AtGtsfrModel.FunctionKeyMode, "d").ToString() + ","
                                + AtGtsfrModel.Reserved + ","
                                + Enum.Format(typeof(CommEnums.MovementDetectMode), AtGtsfrModel.MovementDetectMode, "d").ToString() + ","
                                + AtGtsfrModel.MovementSpeed + ","
                                + AtGtsfrModel.MovementDistance + ","
                                + AtGtsfrModel.MovementSendNumber + ","
                                + Enum.Format(typeof(CommEnums.FullChargeBoot), AtGtsfrModel.FullChargeBoot, "d").ToString() + ","
                                + AtGtsfrModel.SendTime + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}
