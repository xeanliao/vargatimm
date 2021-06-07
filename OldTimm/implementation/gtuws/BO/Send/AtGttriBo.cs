using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class AtGttriBo : BaseSendBo
    {
        public AtGttri AtGttriModel { get; private set; }

        public AtGttriBo()
        {
            AtGttriModel = new AtGttri();
        }

        public AtGttriBo(AtGttri atGttriModel)
        {
            AtGttriModel = atGttriModel;
        }

        public override void Send()
        {
                
        }

        /// <summary>
        /// Build AT+GTTRI Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(AtGttriModel.CommandName + AtGttriModel.Password + ","
                                + AtGttriModel.BeginTime + ","
                                + AtGttriModel.EndTime + ","
                                + AtGttriModel.SendInterval + ","
                                + AtGttriModel.FixInterval + ","
                                + AtGttriModel.SendTime + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}
