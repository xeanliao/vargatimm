using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using GTU.Utilities.DataConvert;


namespace GTUWS
{
    public partial class ListenPort : ServiceBase
    {

        private static string logPath;
        private static string logif;
        private volatile Socket ReceiverSocket;
        public bool stopReceiver = false;

        public ListenPort()
        {
            InitializeComponent();

            logif = System.Configuration.ConfigurationSettings.AppSettings["logif"];
            logPath = System.Configuration.ConfigurationSettings.AppSettings["logPath"];
        }

        //public delegate void WaitCallback(Object state);

        protected override void OnStart(string[] args)
        {
            try
            {
                /// old-begin///
                //int port = 7000;
                //string host = "127.0.0.1";
                //IPAddress ip = IPAddress.Parse(host);
                //IPEndPoint ipe = new IPEndPoint(ip, port);
                //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//create a Socket class
                //s.Bind(ipe);//Bind 7000 port
                //s.Listen(5);//begin listen
                //WriteLog("Wait for connect");
                //Socket temp = s.Accept();//Create new socket for the connect 
                //WriteLog("Get a connect");
                //string recvStr = "";
                //byte[] recvBytes = new byte[1024];
                //int bytes;
                //bytes = temp.Receive(recvBytes, recvBytes.Length, 0);//receive message from client 

                //recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                //WriteLog(string.Format("Server Get Message:{0}", recvStr));//Display received message 
                //string sendStr = "Ok!Client Send Message Sucessful!";
                //byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                //temp.Send(bs, bs.Length, 0);//return the confirmation info to client 
                //temp.Close();
                //s.Close();
                ///old-end///

                CreateReceiverSocket();

                // 侦听客户端连接请求线程, 使用委托推断, 不建 CallBack 对象

                Thread thread = new Thread(new ThreadStart(ListenRequest));
                thread.Start();

                //while (!thread.IsAlive) ;
                Thread.Sleep(10);

                thread.Join();

                //System.Threading.WaitCallback waitCallback = new WaitCallback(ListenRequest);


                //ThreadPool.QueueUserWorkItem(waitCallback);

            }
            catch (ArgumentNullException ex1)
            {
               WriteLog(string.Format("ArgumentNullException: {0}", ex1));
            }
            catch (SocketException ex2)
            {
                WriteLog(string.Format("SocketException: {0}", ex2));
            }
        }

        protected override void OnStop()
        {
            ReceiverSocket.Close();
        }

        public void WriteLog(string text)
        {
            if (logif == "1")
            {
                string path = logPath;
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter m_streamWriter = new StreamWriter(fs);
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                //m_streamWriter.WriteLine(DateTime.Now.ToString() + " : " + text + "\n");
                m_streamWriter.WriteLine(text + "\n");
                m_streamWriter.Flush();
                m_streamWriter.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 创建接收服务器的 Socket, 并侦听客户端连接请求
        /// </summary>
        private bool CreateReceiverSocket()
        {
            try
            {
                ReceiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //int port = 7000;
                //string host = "127.0.0.1";

                int port = 7000;
                string host = "127.0.0.1";

                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, port);
                ReceiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//create a Socket class
                ReceiverSocket.Bind(ipe);//Bind 7000 port
                ReceiverSocket.Listen(0);//begin listen
                //WriteLog("开启socket端口");

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ListenRequest()
        {
            while (true)
            {
                try
                {
                    //WriteLog("Enter thread");
                    //bool temp = ReceiverSocket.Poll(10, SelectMode.SelectRead);
                    //WriteLog(String.Format("{0} {1}", "temp = ", temp));
                    //if (ReceiverSocket.Poll(2, SelectMode.SelectRead))
                    //{
                        
                        //WriteLog("服务器处于socket连接状态");

                        //if (ReceiverSocket == null)
                        //{
                        //    WriteLog("ReceiverSocket = null");
                        //}
                        //else
                        //{
                        //    WriteLog("ReceiverSocket != null");
                        //}
                    
                        //WriteLog(client.ToString());
                        //bool aa = ReceiverSocket.Poll(1000, SelectMode.SelectRead);
                        //WriteLog("Poll" + aa);
                         
                        Socket client = ReceiverSocket.Accept();
                        
                        //WriteLog("有数据来了");
                        if (client != null && client.Connected)
                        {
                            try 
                            {
                                //WriteLog("开始接受来自该客户端的数据");
                                 
                                string recvStr = "";
                                byte[] recvBytes = new byte[1024];
                                int bytes;
                                //bytes = client.Receive(recvBytes, recvBytes.Length, 0);//receive message from client 


                                bytes = client.Receive(recvBytes);
                                recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);

                                String[] commandRead = DataConvert.SplitString(recvStr, ",");
                                if (commandRead.Length > 0)
                                {
                                    if (commandRead[0] == "+RESP:GTTRI")
                                    {
                                        //var respGttriBo = new RespGttriBo();
                                        //respGttriBo.RespGttriModel.



                                        WriteLog(string.Format("{0},{1},", commandRead[11], commandRead[10]));//Display received message
                                    }
                                }

                                //WriteLog("接受完毕，开始发送反馈信息");
                                //String curTime = DateTime.Now.ToString();
                                //string sendStr = String.Format("{0},{1}", "Ok!Client Send Message Sucessful!", curTime);
                                //byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                                //client.Send(bs, bs.Length, 0);//return the confirmation info to client 
                                //WriteLog("反馈信息发送完毕");
                                //client.Close();
                                
                            }
                            catch
                            {

                            }
                        }
                        else if (client != null)  // 非空，但没有连接（connected is false）
                        {
                            try
                            {
                                WriteLog("发生异常了--1！！");
                                client.Shutdown(SocketShutdown.Both);
                                client.Close();
                            }
                            catch { }
                        }
                    //}
                }
                catch
                {
                    //if (client != null)
                    //{
                    //    try
                    //    {
                    //        WriteLog("发生异常了--2！！");
                    //        client.Shutdown(SocketShutdown.Both);
                    //        client.Close();
                    //    }
                    //    catch { }
                    //}
                }
                //Thread.Sleep(1000);
                // 该处可以适当暂停若干毫秒
            }

            

            }
            // 该处可以适当暂停若干毫秒

    }
}
