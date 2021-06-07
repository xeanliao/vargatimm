using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using log4net;

namespace GPSListener.TIMM
{

    // Socket Packet for reading client data asynchronously
    public abstract class SocketPacket// : IDisposable
    {
    #region Public Fields
        public Socket CurrentSocket;
        // Buffer to store the data sent by the client
        public byte[] DataBuffer = new byte[1024];
        public Queue<string> ReplyMessageQ = new Queue<string>();

        public DateTime MostRecentUpdatedTime
        {
            get
            {
                return _oMostRecentUpdatedTime;
            }
        }

        public TimeSpan TimeoutValue
        {
            get
            {
                return _oTimeoutValue;
            }
        }
    #endregion

    #region Private Fields
    
        protected DateTime _oMostRecentUpdatedTime;
        protected TimeSpan  _oTimeoutValue;
    #endregion

        /// <summary>
        /// Sub class must implement the abstact funtions
        /// </summary>
        #region Abstract funtions
            public abstract void ProcessMessage(string sMessage);
        #endregion

        public SocketPacket()
        {
            CurrentSocket = null;
            _oMostRecentUpdatedTime = DateTime.Now;
            _oTimeoutValue = new TimeSpan(1, 0, 0);// 1 hour TODO: read from configuration file 

        }

        /// <summary>
        /// Addreply will add the reply message in a Queue and will be sent 
        /// by listener asynchronously
        /// </summary>
        protected  void AddReply(string s)
        {
            ReplyMessageQ.Enqueue(s);
        }

        public  void Disconnect()
        {
            CurrentSocket.Shutdown(SocketShutdown.Both);
            CurrentSocket.Close();
        }

        ~SocketPacket()
        {
            Dispose(false);
            Console.WriteLine("In destructor.");
        }
        /// <summary>
        /// Use dispose design pattern to close the Socket
        /// </summary>
        /// <param name="disposing"></param>
    
        protected virtual void Dispose(bool disposing)
	    {
	      if (disposing)
	      {
              if (CurrentSocket != null)
              {
                  Disconnect();
              }
          }
	  }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
}

    public abstract class AsynchronousSocketListener 
    {
        private static ILog log = LogManager.GetLogger(typeof(AsynchronousSocketListener));

        private static ManualResetEvent _allDone = new ManualResetEvent(false);
        private static bool _bListening = true;
        public Socket _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Hashtable _clientHandlerTable = Hashtable.Synchronized(new Hashtable());
        
        private Thread _oThread = null;
        private Timer _oTimer = null;
        /// <summary>
        ///  Sub class must implement the follwoing method 
        /// </summary>
        protected abstract SocketPacket GetSocketPacket();

        public AsynchronousSocketListener()
        {
        }
        
        private String  SendReceive( Socket s, String request)
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];

            // Send request to the server.
            s.Send(bytesSent, bytesSent.Length, 0);

            // Receive the server home page content.
            int bytes = 0;
            string sRet= "";

            // The following will block until te page is transmitted.
            do
            {
                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                sRet = sRet + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            return sRet;
        }

        private bool Initialize()
        {
            bool bRet = true;
            return bRet;
        }

        public virtual void  Stop()
        {
            _bListening = false;
            // Signal the Listening main thread to Stop.
            _allDone.Set();
            if (_oThread != null)
            {
                if (!_oThread.Join(10000))  //Wait for 10 seconds, TODO: change to be configurable
                {
                    _oThread.Abort(); // kill the thread
                    _oThread.Join();            
                }
            }

        }
        public void Start()
        {
            _oThread = new Thread(new ThreadStart(StartListening));
            _oThread.Start();
            long nCleanupTimeInterval = 60 * 1000;//Clean up every 1 hour. TODO: be configurable
            _oTimer = new Timer(new TimerCallback(CleanSocketPackets), null, nCleanupTimeInterval, nCleanupTimeInterval);// 

        }
        private void CleanSocketPackets(object state)
        {
            if (Monitor.TryEnter(_clientHandlerTable))
            {
                try
                {
                    System.Diagnostics.Trace.TraceInformation("CleanSocketPackets\n");
                    List<Socket> oDeletedList = new List<Socket>();
                    foreach (DictionaryEntry de in _clientHandlerTable)
                    {
                        Socket handler = (Socket)de.Key;
                        SocketPacket oSoketPkt = (SocketPacket)de.Value;
                        if (!handler.Connected)
                        {
                            oDeletedList.Add(handler);
                        }
                        else if (DateTime.Now - oSoketPkt.MostRecentUpdatedTime > oSoketPkt.TimeoutValue)
                        {
                            oDeletedList.Add(handler);
                        }
                    }
                    foreach (Socket skt in oDeletedList)
                    {
                        ((SocketPacket)_clientHandlerTable[skt]).Dispose();
                        _clientHandlerTable.Remove(skt);

                    }
                }
                finally
                {
                    Monitor.Exit(_clientHandlerTable);
                }
            }
        }
        private void StartListening()
        {
            _bListening = true; 
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];


            #region Modify by steve.j.yin on 2014-12-02 for listen in any address and make listen port can be changed in app.config file
            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);            
            var configPort = ConfigurationManager.AppSettings["Listen_Port"];
            int port;
            if (!int.TryParse(configPort, out port))
            {
                port = 11000;
            }
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            log.InfoFormat("======Begin Listen on {0}======", port);
            #endregion

            // Create a TCP/IP socket.
            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                _Socket.Bind(localEndPoint);
                _Socket.Listen(100);

                while (_bListening)
                {
                    // Set the event to nonsignaled state.
                    _allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    System.Diagnostics.Trace.TraceInformation("Waiting for a connection...");
                    _Socket.BeginAccept(new AsyncCallback(OnClientConnect), null);

                    // Wait until a connection is made before continuing.
                    _allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceInformation(e.ToString());
            }


        }

        // Start waiting for data from the client
        public void WaitForData(SocketPacket theSocPkt )
        {
            try
            {
                Socket handler = theSocPkt.CurrentSocket;
                handler.BeginReceive(theSocPkt.DataBuffer, 0,
                    theSocPkt.DataBuffer.Length,
                    SocketFlags.None,
                    new AsyncCallback (OnDataReceived),
                    theSocPkt);
            }
            catch (SocketException e)
            {
                System.Diagnostics.Trace.TraceError("\n OnClientConnection: Socket exception {0}\n", e.Message);
            }
            catch
            {
                System.Diagnostics.Trace.TraceError("\n OnClientConnection: Socket exception, unknown error \n");
            }
        }
        private void OnClientConnect(IAsyncResult ar)
        {
            log.Info("have client connected!");
            // Signal the main thread to continue.
            _allDone.Set();
            try
            {

                Socket handler = _Socket.EndAccept(ar);
                SocketPacket theSocPkt = GetSocketPacket();
                theSocPkt.CurrentSocket = handler;
                // The Add method throws an exception if the socket is  already in the  table.
                try
                {
//                    _clientHandlerTable.Add(handler, theSocPkt);
                    WaitForData(theSocPkt);
                }
                catch
                {
                    System.Diagnostics.Trace.TraceError(" The socket already exists.");
                }
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Trace.TraceWarning("\n OnClientConnection: Socket has been closed\n");
            }
            catch (SocketException e)
            {
                System.Diagnostics.Trace.TraceError("\n OnClientConnection: Socket exception {0}\n", e.Message );
            }
            catch
            {
                System.Diagnostics.Trace.TraceWarning("\nOnClientConnection: Unknown error\n");
            }

        }

        private  void OnDataReceived(IAsyncResult ar)
        {
            SocketPacket socketData = (SocketPacket)ar.AsyncState;
            try
            {
                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                Socket handler = socketData.CurrentSocket;
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var recived = Encoding.ASCII.GetString(socketData.DataBuffer, 0, bytesRead);
                    log.InfoFormat("recived: {0}", recived);
                    try
                    {

                        socketData.ProcessMessage(recived);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Process Message Error", ex);
                    }
                    if (handler.Connected)
                    {
                        Send(socketData);
                    }
                    WaitForData(socketData);
                }
                else
                {
                    socketData.Disconnect(); 
                    //Bad connection, close it;
                }
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Trace.TraceWarning("\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054) // Error code for Connection reset by peer
                {
                    System.Diagnostics.Trace.TraceWarning("Client {0} Disconnected\n", (((IPEndPoint)socketData.CurrentSocket.RemoteEndPoint).Address.ToString ()));
                }
                else
                {
                    System.Diagnostics.Trace.TraceError("\n OnClientConnection: Socket exception {0}\n", e.Message);
                }
            }
            catch
            {
                System.Diagnostics.Trace.TraceWarning("\nOnDataReceived: Unknown error\n");
            }

        }
        private static void Send(SocketPacket socketData)
        {
            try
            {
                foreach (string sReply in socketData.ReplyMessageQ)
                {
                    // Convert the string data to byte data using ASCII encoding.
                    byte[] byteData = Encoding.ASCII.GetBytes(sReply);

                    // Begin sending the data to the remote device.
                    socketData.CurrentSocket.BeginSend(byteData, 0, byteData.Length, 0,
                        new AsyncCallback(SendCallback), socketData);
                }
                socketData.ReplyMessageQ.Clear();
            }
            catch (SocketException e)
            {
                System.Diagnostics.Trace.TraceError("\nSend: Socket exception {0}\n", e.Message);
            }
            catch
            {
                System.Diagnostics.Trace.TraceError("\n Send, unknown error \n");
            }

        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                SocketPacket socketData = (SocketPacket)ar.AsyncState;
                Socket handler = socketData.CurrentSocket;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                System.Diagnostics.Trace.TraceInformation("Sent {0} bytes to client.", bytesSent);
                System.Diagnostics.Trace.TraceInformation("\n Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("\n SendCallback: exception {0}\n", e.Message);
            }
            catch
            {
                System.Diagnostics.Trace.TraceError("\n Send, unknown error \n");
            }

        }

    }
}
