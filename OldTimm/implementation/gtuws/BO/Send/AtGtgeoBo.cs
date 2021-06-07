using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Send;
using GTU.Utilities.Base;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Send
{
    public class AtGtgeoBo : BaseSendBo
    {
        public AtGtgeo AtGtgeoBoModel { get; private set; }

        public AtGtgeoBo()
        {
            AtGtgeoBoModel = new AtGtgeo();
        }

        public AtGtgeoBo(AtGtgeo atGtgeoBoModel)
        {
            AtGtgeoBoModel = atGtgeoBoModel;
        }

        public override void Send()
        {
            
        }

        /// <summary>
        /// Build AT+GTGEO Command
        /// </summary>
        /// <returns>Byte[] type Command for Socket to send to Terminal</returns>
        public Byte[] BuildCommand()
        {
            StringBuilder commandString = new StringBuilder();
            commandString.Append(AtGtgeoBoModel.CommandName + AtGtgeoBoModel.Password + "," + AtGtgeoBoModel.GeofenceId + "," + AtGtgeoBoModel.Longitude + "," + AtGtgeoBoModel.Latitude + "," + AtGtgeoBoModel.Radius + "," + AtGtgeoBoModel.CheckInterval + "," + Enum.Format(typeof(CommEnums.GeoFenceType), AtGtgeoBoModel.GeofenceType, "d") + "," + AtGtgeoBoModel.SendTime + "\0");
            Byte[] commandByte = Encoding.ASCII.GetBytes(commandString.ToString());
            return commandByte;
        }
        
        
    }
}
