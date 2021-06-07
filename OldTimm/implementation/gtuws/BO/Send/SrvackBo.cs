using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class SrvackBo : BaseSendBo
    {
        public Srvack SrvackModel { get; private set; }

        public SrvackBo()
        {
            SrvackModel = new Srvack();
        }

        public SrvackBo(Srvack srvackModel)
        {
            SrvackModel = srvackModel;
        }

        public override void Send()
        {
                
        }

        /// <summary>
        /// Build +SRVACK Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(SrvackModel.CommandName + ","
                                + SrvackModel.CountNum + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}
