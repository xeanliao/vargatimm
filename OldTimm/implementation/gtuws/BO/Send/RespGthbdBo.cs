using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class RespGthbdBo : BaseSendBo
    {
        public RespGthbd RespGthbdModel { get; private set; }

        public RespGthbdBo()
        {
            RespGthbdModel = new RespGthbd();
        }

        public RespGthbdBo(RespGthbd respGthbdModel)
        {
            RespGthbdModel = respGthbdModel;
        }

        public override void Send()
        {
                
        }

        /// <summary>
        /// Build RESP:GTHBD Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(RespGthbdModel.CommandName + ","
                                + RespGthbdModel.Content + ","
                                + RespGthbdModel.UniqueId + ","
                                + RespGthbdModel.SendTime + ","
                                + RespGthbdModel.CountNum + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
    }
}
