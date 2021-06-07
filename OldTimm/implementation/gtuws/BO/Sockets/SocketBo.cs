using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using GTU.ModelLayer.Common;
using GTU.Utilities.Logging;
using GTU.Utilities.DataConvert;
using GTU.BusinessLayer.Read;
using GTU.BusinessLayer.Track;

namespace GTU.BusinessLayer.Sockets
{
    public class SocketBo
    {
        private volatile Socket _receiverSocket;

        public Socket ReceiverSocket
        {
            get { return _receiverSocket; }
            set { _receiverSocket = value; }
        }

        public SocketBo()
        { }

        public bool CreateReceiverSocket(int port, String host)
        {
            try
            {
                var socketLog = new Logging();
                _receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, port);
                ReceiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//create a Socket class
                ReceiverSocket.Bind(ipe);//Bind 7000 port
                ReceiverSocket.Listen(0);//begin listen
                socketLog.WriteLog("开启socket端口");

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
