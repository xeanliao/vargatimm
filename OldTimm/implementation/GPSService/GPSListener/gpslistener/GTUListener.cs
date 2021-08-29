using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using GTUService.TIMM; 
using TIMM.QueueUpdater;
using System.Globalization;

namespace GPSListener.TIMM
{
    /// <summary>
    /// Sub class AsynchronousSocketListener
    /// </summary>
    public class GTUListener : AsynchronousSocketListener
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GTUListener));
        private GTUUpdateServiceClient _gpsServiceClient;
        private QueueUpdater<GTU> _GTUQUpdater;

        public GTUListener()
        {
            _gpsServiceClient = new GTUUpdateServiceClient();
            _GTUQUpdater = new QueueUpdater<GTU>();
            _GTUQUpdater.Init(new QueueUpdater<GTU>.ProcessQHandler(this.UpdateGPSWebService), null);
        }
        protected sealed override SocketPacket GetSocketPacket()
        {
            return new GTUPacket(_GTUQUpdater);
        }
        public override void Stop()
        {
            _GTUQUpdater.Stop();
            base.Stop();
        }
        /// <summary>
        /// This is to implemet the ProcessQHandler delegate  for QueueUpdater.
        ///     This method will be called in the thread pool of ProcessQHandler.
        /// </summary>
        /// <param name="oGTU"></param>
        /// <returns></returns>
        bool UpdateGPSWebService(GTU oGTU)
        {
            bool bRet = false;
            try
            {
                _gpsServiceClient.UpdateGTU(oGTU.Code, oGTU);
                bRet = true;
            }
            catch (System.ServiceModel.FaultException exp)
            {
                log.Error("Failed to call web service.", exp);
            }
            catch (Exception exp)
            {
                log.Error("Failed to call web service.", exp);
            }
            return bRet;

        }


    }

    /// <summary>
    /// Sub class of SocketPacket
    /// </summary>
    public sealed class GTUPacket : SocketPacket
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GTUPacket));
        private static string sSentenceBreaker = "\0";
        private static int sSentenceBreakerLen = sSentenceBreaker.Length;
        private const long nMaxButter = 1024*1024;
        private StringBuilder _sInMessage = new StringBuilder();
        private QueueUpdater<GTU> _GTUQUpdater;

        public GTUPacket(QueueUpdater<GTU> gtuQUpdater)
        {
            _GTUQUpdater = gtuQUpdater;
        }
        public String GetSentence()
        {
            log.Info("GetSentence");
            String sSentence = "";
            var currentMessage = _sInMessage.ToString();
            int gl100nIndex = currentMessage.IndexOf(sSentenceBreaker);
            int gl300nIndex = currentMessage.IndexOf("$");
            int nIndex = -1;
            if (gl100nIndex > -1 && gl300nIndex > -1)
            {
                nIndex = Math.Min(gl100nIndex, gl300nIndex);
            }
            else
            {
                nIndex = Math.Max(gl100nIndex, gl300nIndex);
            }

            if (nIndex > -1)
            {
                sSentence = _sInMessage.ToString().Substring(0, nIndex);
                _sInMessage.Remove(0, nIndex);
                _sInMessage.Remove(0, sSentenceBreakerLen);  //Padding

            }
            log.InfoFormat("nIndex:{0}, sSentence:{1}", nIndex, sSentence);
            return sSentence;
        }
        /// <summary>
        /// Process Heartbeat message
        /// </summary>
        /// <param name="sIn"></param>
        private void ProcessHeartBeat(String sIn)
        {
            log.Debug("Begin Process GL100 HeartBeat");
            log.DebugFormat(">>{0}", sIn);
            string sReply = "+RESP:GTHBD,GPRS ACTIVE,HeartBeat";
            AddReply(sReply);
            log.Debug("End Process GL100 HeartBeat");
        }
        /// <summary>
        /// Process Heartbeat message for GL300
        /// the request message should only have 3 filed in the document.
        /// I am not sure why the real device send more filed.
        /// any way. just need the request message have more than 1 filed.
        /// </summary>
        /// <param name="sIn"></param>
        private void ProcessHeartBeatForGL300(String sIn)
        {
            log.Debug("Begin Process GL300 HeartBeat");
            //device send +SACK:GTHDB,{Count number},$
            //server should reply: +SACK:{Count number}$
            var msg = sIn.Split(new char[] { ',' }, StringSplitOptions.None);
            if (msg.Length < 2)
            {
                log.ErrorFormat("Recived bad Heart Beat: {0}", sIn);
                return;
            }

            string sReply = string.Format("+SACK:{0}$", msg[1]);
            AddReply(sReply);
            log.Debug("End Process GL300 HeartBeat");
        }
        /// <summary>
        /// This function processed in TCP thread pool, need to finish the function as soon as possible;
        /// Process the Fixing timing reporting
        /// </summary>
        /// <param name="sIn"></param>
        private void ProcessFixedTimingReportingForGL100(String sIn)
        {
            log.Debug("Begin Parse GL1000 +RESP:GTTRI");
            log.DebugFormat(">>{0}", sIn);
            char[] charSeparators = new char[] { ',' };
            string[] result = sIn.Split(charSeparators, StringSplitOptions.None);
            int n = result.GetLength(0);
            if( n != 20 )
            {
                log.ErrorFormat("the message should only have 20 fields.\r\n>>{0}", sIn);
                return;
            }
            try
            {
                string sCode = result[1];
                if (sCode == null || sCode.Length == 0)
                {
                    log.Error("Failed to get sCode");
                    return;
                }
                int nGPSFix; 
                if(!int.TryParse(result[5], out nGPSFix))
                {
                    log.ErrorFormat("Failed to get GPSFix {0}", result[5]);
                    return ;
                }
                double dwSpeed;
                if (!double.TryParse(result[6], out dwSpeed))
                {
                    log.ErrorFormat("Failed to get Speed {0}", result[6]);
                    return;
                }

                int nHeading;
                if(!int.TryParse(result[7], out nHeading))
                {
                    log.ErrorFormat("Failed to get Heading {0}", result[7]);
                    return ;
                }
                
                double dwAltitude;
                if (!double.TryParse(result[8], out dwAltitude))
                {
                    log.ErrorFormat("Failed to get Altitude {0}", result[8]);
                    return;
                }

                int nAccuracy; 
                if(!int.TryParse(result[9], out nAccuracy))
                {
                    log.ErrorFormat("Failed to get Accuracy {0}", result[9]);
                    return ;
                }

                double dwLongtitude;
                if (!double.TryParse(result[10], out dwLongtitude ))
                {
                    //update log information
                    log.ErrorFormat("Failed to get Longtitude {0} in {1}", result[10], sIn);
                    return;
                }

                double dwLatitude;
                if (!double.TryParse(result[11], out dwLatitude))
                {
                    log.ErrorFormat("Failed to get Latitude {0}", result[11]);
                    return;
                }

                DateTime dtSentTime = DateTime.Now;
                string sDateTime = result[12];
                try
                {
                    dtSentTime = new DateTime(int.Parse(sDateTime.Substring(0, 4))     //Year
                                                , int.Parse(sDateTime.Substring(4, 2))     //Month
                                                , int.Parse(sDateTime.Substring(6, 2))      //Date
                                                , int.Parse(sDateTime.Substring(8, 2))      // Hour
                                                , int.Parse(sDateTime.Substring(10, 2))    //Minutes
                                                , int.Parse(sDateTime.Substring(12, 2))    //Seconds
                                            );
                    dtSentTime = TimeZoneInfo.ConvertTime(dtSentTime, TimeZoneInfo.Utc, TimeZoneInfo.Local); 
                }
                catch (FormatException)
                {
                    log.ErrorFormat("Failed to get DateTime {0}", result[12]);
                }

                int nMMC;
                if (!int.TryParse(result[13], out nMMC))
                {
                    log.ErrorFormat("Failed to get MMC {0}", result[13]);
                    return;
                }

                int nMNC;
                if (!int.TryParse(result[14], out nMNC))
                {
                    log.ErrorFormat("Failed to get MNC {0}", result[14]);
                    return;
                }


                int nLac;
                try
                {
                    nLac = Convert.ToInt32(result[15], 16);
                }
                catch
                {
                    log.ErrorFormat("Failed to get Lac {0}", result[15]);
                    return;
                }

                int nCellID;
                try
                {
                    nCellID = Convert.ToInt32(result[16], 16);
                }
                catch
                {
                    log.ErrorFormat("Failed to get CellID {0}", result[16]);
                    return;
                }
                int nCount;
                try
                {
                    nCount = Convert.ToInt32(result[18], 16);
                }
                catch
                {
                    log.ErrorFormat("Failed to get Count {0}", result[18]);
                    return;
                }

                String sVer = result[19];

                String sAreadCode = result[15];
                String sCellID = result[16];
                GTU oGut = new GTU();
                oGut.Code = sCode;
                oGut.GPSFix = nGPSFix;
                oGut.Speed = dwSpeed;
                oGut.Heading = nHeading;
                oGut.Accuracy = nAccuracy;
                
                oGut.CurrentCoordinate = new Coordinate();
                oGut.CurrentCoordinate.Altitude = dwAltitude;
                oGut.CurrentCoordinate.Longitude = dwLongtitude;
                oGut.CurrentCoordinate.Latitude  = dwLatitude;
                oGut.SendTime = dtSentTime;
                oGut.ReceivedTime = DateTime.Now;
                oGut.AreaCode = nMMC;
                oGut.NetworkCode = nMNC;
                oGut.Count = nCount;
                oGut.Version = sVer;
                oGut.LocationID = nLac;

                log.DebugFormat("GL100 recived: \r\n{0}", Newtonsoft.Json.JsonConvert.SerializeObject(oGut));

                if (dwLatitude != 0 && dwLongtitude != 0)
                {
                    _GTUQUpdater.AddQ(oGut);
                }
                else
                {
                    log.ErrorFormat("Lat add/or Long invalid Lat:{0} Long:{1}\r\n>>{2}", dwLatitude, dwLongtitude, sIn);
                }
                
            }
            catch(Exception ex)
            {
                log.Error(string.Format("Parse GL1000 unkown error\r\n>>{0}", sIn), ex);
            }
            log.Debug("End Parse GL1000 +RESP:GTTRI");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sIn"></param>
        private void ProcessFixedTimingReportingForGL300(String sIn)
        {
            log.Debug("Begin Parse GL3000 +RESP:GTFR");
            log.DebugFormat(">>{0}", sIn);
            string[] result = sIn.Split(new char[] { ',' }, StringSplitOptions.None);
            int n = result.Length;
            if (n != 22)
            {
                log.ErrorFormat("the message should only have 22 fields.\r\n>>{0}", sIn);
                return;
            }
            try
            {
                #region Parse Report
                //GL300 not version use default value GL300
                String sVer = result[1];
                //devicename
                string sCode = result[2];
                if (sCode == null || sCode.Length == 0)
                {
                    log.ErrorFormat("Failed to get sCode=UniqueID/IMEI at poistion:3 in \r\n>> {0}", sIn);
                    return;
                }
                //For GL300 no GPSFix use default value 0;
                int nGPSFix = 0;

                int nAccuracy;
                if (!int.TryParse(result[7], out nAccuracy))
                {
                    log.ErrorFormat("Failed to get GPS Accuracy at poistion:8 in \r\n>> {0}", sIn);
                    return;
                }

                double dwSpeed;
                if (!double.TryParse(result[8], out dwSpeed))
                {
                    log.ErrorFormat("Failed to get Speed at poistion:9 in \r\n>> {0}", sIn);
                    return;
                }
                //GL300 do not have Heading use azimuth replace
                int nHeading;
                if (!int.TryParse(result[9], out nHeading))
                {
                    log.ErrorFormat("Failed to get Azimuth at position:10 in \r\n>> {0}", sIn);
                    return;
                }

                double dwAltitude;
                if (!double.TryParse(result[10], out dwAltitude))
                {
                    log.ErrorFormat("Failed to get Altitude at poistion:11 in \r\n>> {0}", sIn);
                    return;
                }

                double dwLongtitude;
                if (!double.TryParse(result[11], out dwLongtitude))
                {
                    log.ErrorFormat("Failed to get Longtitude at poistion:12 in \r\n>> {0}", sIn);
                    return;
                }

                double dwLatitude;
                if (!double.TryParse(result[12], out dwLatitude))
                {
                    log.ErrorFormat("Failed to get Latitude at poistion:13 in \r\n>> {0}", sIn);
                    return;
                }

                DateTime dtSentTime;
                string sDateTime = result[13];
                System.Globalization.CultureInfo enUS = new System.Globalization.CultureInfo("en-US");
                if (!DateTime.TryParseExact(sDateTime, "yyyyMMddHHmmss", enUS, System.Globalization.DateTimeStyles.AssumeUniversal, out dtSentTime))
                {
                    dtSentTime = DateTime.Now;
                    log.ErrorFormat("Failed to get GPS UTC Time at poistion:14 in \r\n>> {0} \r\n== use server time replace ==", sIn);
                }
                
                //use GL300 MCC replace
                int nMMC;
                if (!int.TryParse(result[14], out nMMC))
                {
                    log.ErrorFormat("Failed to get MCC at poistion:15 in \r\n>> {0}", sIn);
                    return;
                }

                int nMNC;
                if (!int.TryParse(result[15], out nMNC))
                {
                    log.ErrorFormat("Failed to get MNC at poistion:16 in \r\n>> {0}", sIn);
                    return;
                }


                int nLac;
                if (!int.TryParse(result[16], NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier, null, out nLac))
                {
                    log.ErrorFormat("Failed to get LAC at poistion:17 in \r\n>> {0}", sIn);
                    return;
                }
               

                int nCellID;
                if (!int.TryParse(result[17], NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier, null, out nCellID))
                {
                    log.ErrorFormat("Failed to get CellID at poistion:18 in \r\n>> {0}", sIn);
                    return;
                }
                //if Report type == 0 means there will NOT have I/O status. the count field will be in position: 22
                //if Report type == 1 means there will have I/O status. the count field will be in position: 23
                // In GL300M the count always in position 22 and have 2 new report type 16 and 17
                //if Report type == 16 means The message is a scheduled postiion report generated in MOTION state.
                //if Report type == 17 means The message is a turning point report generated in MOTION state.
                int nCount;
                if (result[5] == "0" || result[5] == "16" || result[5] == "17")
                {
                    if (!int.TryParse(result[21], NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier, null, out nCount))
                    {
                        log.ErrorFormat("Failed to get Count at poistion:21 when report type is 0 in \r\n>> {0}", sIn);
                        return;
                    }
                }
                else
                {
                    if (!int.TryParse(result[22], NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier, null, out nCount))
                    {
                        log.ErrorFormat("Failed to get Count at poistion:22 when report type is 1 in \r\n>> {0}", sIn);
                        return;
                    }
                }

                #endregion

                GTU oGut = new GTU();
                oGut.Code = sCode;
                oGut.GPSFix = nGPSFix;
                oGut.Speed = dwSpeed;
                oGut.Heading = nHeading;
                oGut.Accuracy = nAccuracy;
                oGut.CurrentCoordinate = new Coordinate();
                oGut.CurrentCoordinate.Altitude = dwAltitude;
                oGut.CurrentCoordinate.Longitude = dwLongtitude;
                oGut.CurrentCoordinate.Latitude = dwLatitude;
                oGut.SendTime = dtSentTime;
                oGut.ReceivedTime = DateTime.Now;
                oGut.AreaCode = nMMC;
                oGut.NetworkCode = nMNC;
                oGut.Count = nCount;
                oGut.Version = sVer;
                oGut.LocationID = nLac;

                log.DebugFormat("GL300 recived: \r\n{0}", Newtonsoft.Json.JsonConvert.SerializeObject(oGut));

                if (dwLatitude != 0 && dwLongtitude != 0)
                {
                    _GTUQUpdater.AddQ(oGut);
                }
                else
                {
                    log.ErrorFormat("Lat add/or Long invalid Lat:{0} Long:{1}\r\n>>{2}", dwLatitude, dwLongtitude, sIn);
                }
                
            }
            catch(Exception ex)
            {
                log.Error(string.Format("Parse GL3000 unkown error\r\n>>{0}", sIn), ex);
            }
            log.Debug("End Parse GL3000 +RESP:GTTR");
        }
        /// <summary>
        /// This function processed in TCP thread pool, need to finish the function as soon as possible;
        /// </summary>
        /// <param name="sIn"></param>
        private void ProcessPowerInfo(String sIn)
        {
            System.Diagnostics.Trace.TraceInformation("ProcessPowerInfo\n");
            char[] charSeparators = new char[] { ',' };
            string[] result = sIn.Split(charSeparators, StringSplitOptions.None);
            int n = result.GetLength(0);
            if (n != 5)
            {
                System.Diagnostics.Trace.TraceError("\n ProcessPowerDown, there are ony {0}fields \n", n);
                return;
            }
            try
            {
                /*   PowerInfo nPwrInfo = PowerInfo.UnKnown;
                   string snPwrInfo = result[0];
                   if( snPwrInfo.StartsWith("+RESP:GTPFA"))
                   {
                       nPwrInfo = PowerInfo.OFF;
                   }
                   else if (snPwrInfo.StartsWith("+RESP:GTPNA"))
                   {
                       nPwrInfo = PowerInfo.ON;
                   }
                   else if (snPwrInfo.StartsWith("+RESP:GTPLA"))
                   {
                       nPwrInfo = PowerInfo.Low;
                   }

                   string sCode = result[1];
                   if (sCode == null || sCode.Length == 0)
                   {
                       System.Diagnostics.Trace.TraceError("\n Failed to get sCode\n");
                       return;
                   }

                   string sDateTime = result[2];
                   DateTime dtSentTime = DateTime.Now;
                   try
                   {
                       dtSentTime = new DateTime(int.Parse(sDateTime.Substring(0, 4))     //Year
                                                   , int.Parse(sDateTime.Substring(4, 2))     //Month
                                                   , int.Parse(sDateTime.Substring(6, 2))      //Date
                                                   , int.Parse(sDateTime.Substring(8, 2))      // Hour
                                                   , int.Parse(sDateTime.Substring(10, 2))    //Minutes
                                                   , int.Parse(sDateTime.Substring(12, 2))    //Seconds
                                               );
                   }
                   catch (FormatException)
                   {
                       System.Diagnostics.Trace.TraceError("\n ProcessPowerInfo Failed to get DateTime {0} \n", result[2]);
                   }

                   GTU oGut = new GTU();
                   oGut.Code = sCode;
                   oGut.ePowerInfo = nPwrInfo;
                   oGut.ReceivedTime = DateTime.Now;
                   GTUStore.Instance.UpdateGTU(sCode, oGut);
                   */
            }
            catch
            {
                System.Diagnostics.Trace.TraceError("\n ProcessPowerInfo: unknown error {0} ", sIn);
            }
        }
        public override void ProcessMessage(string sMessage)
        {
            //lock (this) //Only allow one thread to process this
            //{
                //System.Diagnostics.Trace.TraceInformation("ProcessMessage \n");

                log.Debug("======enter ProcessMessage======");
                
                if (_sInMessage.Length > nMaxButter)
                {
                    log.ErrorFormat("out of buffer Disconnect!");
                    Disconnect(); //To prevent the DOS attach, just shutdown the socket for this case
                    //_sInMessage.Remove(0, sMessage.Length);// If the buffer size is overflow, discard the oldest one 
                }
                String sSentence = null;
                _sInMessage.Append(sMessage);
                sSentence = GetSentence();
                log.InfoFormat("get sSentence:{0}, length:{1}, will into loop if lenght > 0", sSentence, sSentence.Length);

                while (sSentence.Length > 0)
                {
                    log.Debug("======ProcessMessage======");

                    log.DebugFormat("Recived Message at:{0}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    log.DebugFormat(">>{0}", sSentence);

                    _oMostRecentUpdatedTime = DateTime.Now;

                    //for GL100 Buff report.
                    if (sSentence.StartsWith("#BUF#"))
                    {
                        log.DebugFormat("found gl100 buffer report");
                        sSentence = sSentence.Substring(5);
                    }
                    
                    int index = sSentence.IndexOf(',');
                    var protocol = sSentence.Substring(0, index).Trim();
                    log.DebugFormat("protocol: {0}", protocol);
                    switch (protocol)
                    {
                        //GL100 Heart Beat
                        case "AT+GTHBD":
                            ProcessHeartBeat(sSentence);
                            break;
                        //GL100 Heart Beat
                        case "AT+GTHBD=HeartBeat":
                            ProcessHeartBeat(sSentence);
                            break;
                        //GL200 or GL300 Heart Beat
                        case "+ACK:GTHBD":
                            ProcessHeartBeatForGL300(sSentence);
                            break;
                        //GL100 Location Report
                        case "+RESP:GTTRI":
                            ProcessFixedTimingReportingForGL100(sSentence);
                            break;
                        //GL200 or GL300 Location Report
                        case "+RESP:GTFRI":
                            ProcessFixedTimingReportingForGL300(sSentence);
                            break;
                        //GL200 or GL300 Buff Location Report
                        case "+BUFF:GTFRI":
                            ProcessFixedTimingReportingForGL300(sSentence);
                            break;
                        //GL100 Power off report
                        case "+RESP:GTPFA":
                            ProcessPowerInfo(sSentence);
                            break;
                        //GL100 Power on report
                        case "+RESP:GTPNA":
                            ProcessPowerInfo(sSentence);
                            break;
                        //GL100 unkown Power info
                        case "+RESP:GTPLA":
                            ProcessPowerInfo(sSentence);
                            break;
                        //GL300
                        case "+RESP:GTINF":
                        case "+BUFF:GTINF":
                            //report device status. ignor.
                        break;
                        case "+ACK:GTFKS":
                            //report for +AT:GTFKS. report for function key settings.
                            break;
                        case "+RESP:GTSTC":
                            //stop charging report
                            break;
                        case "RESP:GTEPF":
                            //diconnection external power supply report
                            break;
                        case "+RESP:GTGSM":
                            //report for the infomation of the service cell and the neighbor cells.
                            break;
                        case "+ACK:GTBSI":
                            //report for AT+GTBSI command set GPRS parameter.
                            break;
                        case "+ACK:GTSRI":
                            //reort for GT+GTSRI command set backend service.
                            break;
                        case "+ACK:GTQSS":
                            //report for AT+GTQSS command set GPRS and backend server.
                            break;
                        case "+ACK:GTFRI":
                            //
                            break;
                        case "+RESP:GTSTT":
                            
                            break;
                        case "+RESP:GTBPL":
                            // ignore Battery low report
                        break;
                        default:
                            log.Error("Unkown Message");
                            log.ErrorFormat(">>{0}", sSentence);
                            break;
                    }

                    sSentence = GetSentence();
                    log.Debug("======End ProcessMessage======");
                }
           // }
        }

    }

}
