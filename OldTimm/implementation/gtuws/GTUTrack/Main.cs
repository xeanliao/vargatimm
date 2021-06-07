using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using GTU.Utilities.Logging;
using GTU.Utilities.DataConvert;
using GTU.Utilities.DateTimeUtil;
using GTU.BusinessLayer.Read;
using GTU.BusinessLayer.Track;
using GTU.BusinessLayer.Jobs;
using GTU.BusinessLayer.Sockets;
using GTU.ModelLayer.Device.Read;
using GTU.ModelLayer.Track;
using GTU.ModelLayer.Common;
using GTU.ModelLayer.Jobs;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;
using System.Configuration;
using System.Xml.Serialization;

namespace GTUTrack
{
    public partial class Main : Form
    {
        //private volatile Socket ReceiverSocket;
        public bool stopReceiver = false;
        public Job currentJob = new Job();
        //private SocketBo socketBo;
        public ReceiveCount receiveCount;
        public Logging socketLog = new Logging();

      
        public Socket clientsocket;
        //public Thread clientservice;

        public Main()
        {
            InitializeComponent();
            receiveCount = new ReceiveCount();
            receiveCount.CurrentNumber = 0;
        }

       private void button1_Click(object sender, EventArgs e)
        {
           //Read Job File
            try
            {
                var jobBo = new JobBo();
                currentJob = jobBo.ReadJob();

            }
            catch (Exception ex)
            {
                socketLog.WriteLog(String.Format("Read Job File Error:{0}", ex));
            }


            //Open Socket port and listen
            try
            {
                ListenRequest();
                //System.Threading.WaitCallback waitCallback = new WaitCallback(ListenRequest);
                //ThreadPool.QueueUserWorkItem(waitCallback);
            }
            catch (ArgumentNullException ex1)
            {
                socketLog.WriteLog(string.Format("ArgumentNullException: {0}", ex1));
            }
            catch (SocketException ex2)
            {
                socketLog.WriteLog(string.Format("SocketException: {0}", ex2));
            }
        }

        /// <summary>
        /// Listen Socket and create thread for each connection
        /// </summary>
        public void ListenRequest()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7000);
            listener.Start();

            while (true)
            {
                try
                {
                    Socket s = listener.AcceptSocket();
                    clientsocket = s;

                    Thread clientservice = new Thread(new ThreadStart(RecevieRequest));
                    clientservice.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            //socketBo = new SocketBo();

            //if (!socketBo.CreateReceiverSocket(int.Parse(currentJob.Socket), "127.0.0.1"))
            //{
            //    return;
            //}
            //while (true)
            //{
            //    try
            //    {
            //        Socket s = socketBo.ReceiverSocket.Accept();
            //        clientsocket = s;

            //        Thread clientservice = new Thread(new ThreadStart(RecevieRequest));
            //        clientservice.Start();
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.ToString());
            //    }
            //}
        }

        /// <summary>
        /// Receive and deal with data
        /// </summary>
        public void RecevieRequest()
        {
            try
            {
                //bool temp = ReceiverSocket.Poll(10, SelectMode.SelectRead);
                //if (ReceiverSocket.Poll(2, SelectMode.SelectRead))
                //{
                    
                    lock (this)
                    {
                        Socket tempclient = clientsocket;
                        //bool keepalive = true; 

                        if (tempclient != null && tempclient.Connected)
                        {
                            try
                            {
                                socketLog.WriteLog("开始接受");
                                receiveCount.IncreaseCount();
                                socketLog.WriteLog(receiveCount.CurrentNumber.ToString());
                                string recvStr = "";
                                byte[] recvBytes = new byte[1024];
                                int bytes;

                                bytes = tempclient.Receive(recvBytes);
                                recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);

                                String[] commandRead = DataConvert.SplitString(recvStr, ",");
                                if (commandRead.Length > 0)
                                {
                                    if (commandRead[0] == "+RESP:GTTRI")
                                    {
                                        //receiveCount.IncreaseCount();
                                        //socketLog.WriteLog(receiveCount.CurrentNumber.ToString());
                                        socketLog.WriteLog(string.Format("Server Get Message:{0}", recvStr));//Display received message
                                        var respGttriBo = new RespGttriBo();
                                        respGttriBo.RespGttriModel = respGttriBo.ResolveCommand(recvBytes);

                                        var trackBo = new TrackItemBo();

                                        int trackId = Guid.NewGuid().GetHashCode();
                                        trackBo.TrackItemModel.TrackId = trackId;
                                        trackBo.TrackItemModel.UniqueId = respGttriBo.RespGttriModel.UniqueID;
                                        trackBo.TrackItemModel.longitude = respGttriBo.RespGttriModel.Longitude;
                                        trackBo.TrackItemModel.Latitude = respGttriBo.RespGttriModel.Latitude;
                                        //test data in valid area
                                        //trackBo.TrackItemModel.longitude = "116.38000";
                                        //trackBo.TrackItemModel.Latitude = "39.972000";

                                        //test data in invalid address
                                        //trackBo.TrackItemModel.longitude = "116.368075";
                                        //trackBo.TrackItemModel.Latitude = "39.90988";

                                        trackBo.TrackItemModel.SendTime = DateTimeUtil.ConvertStrToDate(respGttriBo.RespGttriModel.SendTime);
                                        trackBo.TrackItemModel.InDeliverable = false;
                                        trackBo.TrackItemModel.InNonDeliverable = false;
                                        trackBo.TrackItemModel.TrackArea = currentJob.ValidArea;
                                        trackBo.TrackItemModel.InvalidArea = currentJob.InvalidArea;
                                        trackBo.TrackItemModel.InvalidAddress = currentJob.InvalidAddress;

                                        IUnityContainer containerConfig = new UnityContainer();
                                        UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                                        if (section != null)
                                        {
                                            //section.Containers["trackcontainer"].Configure(containerConfig);
                                            section.Configure(containerConfig, "trackcontainer");
                                        }

                                        IEnumerable<IDataTrackProcess> processAll = containerConfig.ResolveAll<IDataTrackProcess>();
                                        foreach (IDataTrackProcess process in processAll)
                                        {
                                            process.Process(trackBo.TrackItemModel);
                                        }
                                    }
                                }

                                socketLog.WriteLog("接受完毕");
                                //String curTime = DateTime.Now.ToString();
                                //string sendStr = String.Format("{0},{1}", "Ok!Client Send Message Sucessful!", curTime);
                                //byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                                //client.Send(bs, bs.Length, 0);//return the confirmation info to client 
                                //socketLog.WriteLog("反馈信息发送完毕");
                                tempclient.Close();
                            }
                            catch (Exception ex)
                            {
                                socketLog.WriteLog(string.Format("Error:{0}", ex));
                                tempclient.Close();
                            }
                        }
                        //else if (tempclient != null) //connected is false
                        else if (!tempclient.Connected) //connected is false
                        {
                            try
                            {
                                socketLog.WriteLog("Error！！");
                                tempclient.Shutdown(SocketShutdown.Both);
                                tempclient.Close();
                            }
                            catch { }
                        }
                }

                //}
            }
            catch
            {

            }
        }

    }
}
