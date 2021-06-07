using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Area;
using System.IO;
using log4net;
using System.Drawing;
using System.Drawing.Imaging;

namespace GPS.Website.SilverlightServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LocationService" in code, svc and config file together.
    [ServiceContract(Namespace = "TIMM.Website.SilverlightServices")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LocationService
    {
        [OperationContract]
        public List<object> PointInPolygon(double[][] coordinates,double[] point,List<object> gti)
        {
            bool isIn=ShapeMethods.PointInPolygon(coordinates, point);
            if (!isIn)
                return gti;
            return null;
        }

        [OperationContract]
        public List<List<object>> PointsInPolygon(double[][] coordinates, Dictionary<List<object>,double[]> points)
        {
            List<List<object>> retList = new List<List<object>>();
            foreach (List<object> o in points.Keys)
            {
                double[] latlon = points[o];
                bool isIn = ShapeMethods.PointInPolygon(coordinates, latlon);
                if (!isIn)
                {
                    retList.Add(o);
                }
            }
            if (retList.Count > 0)
                return retList;
            //bool isIn = ShapeMethods.PointInPolygon(coordinates, points[0]);
            //if (!isIn)
            //    return gti;
            return null;
        }

        /// <summary>
        /// Simple send mail service
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attach">just one attach</param>
        [OperationContract]
        public void SendMail(string to, string subject, string body, string attachName, byte[] attach)
        {
            if (attach == null || string.IsNullOrEmpty(subject))
            {
                ILog logger = log4net.LogManager.GetLogger(typeof(LocationService));
                logger.Error("parameters error. no attached file or no subject");
            }

            //string[] strArr = content.Split(new string[] { "#####" }, StringSplitOptions.None);
            //to = strArr[0];
            //cc = strArr[1];
            //subject = strArr[2];
            //context = strArr[3];
            //attach = strArr[4];

            if (string.IsNullOrEmpty(to))
            {
                ILog logger = log4net.LogManager.GetLogger(typeof(LocationService));
                logger.Error("The auditor mail is empty");

                return; // never send when the mail is empty;
            }

            try
            {
                var mmsg = new System.Net.Mail.MailMessage();
                var appSetting = System.Configuration.ConfigurationManager.AppSettings;

                //发送地址
                if (to != null && to != "")
                {
                    mmsg.To.Add(to);
                }
            
                //抄送地址
                var cc = appSetting["Mail_cc"];
                if (!string.IsNullOrWhiteSpace(cc))
                {
                    string[] ccCollection = cc.Split(',');
                    foreach (string address in ccCollection)
                    {
                        mmsg.CC.Add(address);
                    }
                }

                //发件人信息
                //mmsg.From = new System.Net.Mail.MailAddress("wayne.wei@maesinfo.com", "Wayne", System.Text.Encoding.UTF8);
                mmsg.From = new System.Net.Mail.MailAddress(appSetting["Mail_from"], "TIMM", System.Text.Encoding.UTF8);

                //标题
                mmsg.Subject = subject;
                mmsg.SubjectEncoding = System.Text.Encoding.UTF8;
                //内容
                mmsg.Body = body;
                mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                mmsg.IsBodyHtml = false;

                //附件            
                if (attach != null && attach.Length > 0)
                {
                    var ct = new System.Net.Mime.ContentType()
                    {
                        MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                        Name = attachName
                    };

                    //check BMP file format
                    if (attach[0] == 0x42 && attach[1] == 0x4D)
                    {
                        var data = new System.Net.Mail.Attachment(ConvertToJpeg(attach), ct);
                        mmsg.Attachments.Add(data);
                    }
                    else
                    {
                        var data = new System.Net.Mail.Attachment(new MemoryStream(attach), ct);
                        mmsg.Attachments.Add(data);
                    }
                }

                //优先级
               mmsg.Priority = System.Net.Mail.MailPriority.High;
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                
                smtpClient.Credentials = new System.Net.NetworkCredential(
                    appSetting["Mail_username"],
                    appSetting["Mail_password"]);
                smtpClient.EnableSsl = Convert.ToBoolean(int.Parse(appSetting["Mail_Ssl"])); 
                smtpClient.Host = appSetting["Mail_smtp"];
                smtpClient.Port = int.Parse(appSetting["Mail_port"]); 

                object userState = mmsg;

                smtpClient.SendCompleted += (s, e) =>
                    {
                        if (e.Cancelled)
                        {
                            this.log(string.Format("Cancel Send - {0}", e.Error.Message));
                            ILog logger = log4net.LogManager.GetLogger(typeof(LocationService));
                            logger.Error("Cancel Send");
                            return;
                        }

                        if (e.Error != null)
                        {
                            this.log(string.Format("Send failed:1. Message {0}, 2. InnerException {1}", e.Error.Message, e.Error.InnerException));

                            ILog logger = log4net.LogManager.GetLogger(typeof(LocationService));
                            logger.Error(string.Format("Send failed:1. Message {0}, 2. InnerException {1}", e.Error.Message, e.Error.InnerException));
                            return;
                        }

                        var asncTuple = (e.UserState as System.Net.Mail.MailMessage);

                        asncTuple.Dispose();
                    };
                
                smtpClient.SendAsync(mmsg, userState);
            }
            catch (Exception ex)
            {
                ILog logger = log4net.LogManager.GetLogger(typeof(LocationService));
                logger.Error("Unknown error", ex);

                this.log(ex.Message);
            }
        }

        private MemoryStream ConvertToJpeg(byte[] data)
        {
            MemoryStream outputStream = new MemoryStream();
            using(MemoryStream imgStream = new MemoryStream())
            {
                imgStream.Write(data, 0, data.Length);
                imgStream.Seek(0, SeekOrigin.Begin);
                Image source = Image.FromStream(imgStream);
                source.Save(outputStream, ImageFormat.Jpeg);
                outputStream.Seek(0, SeekOrigin.Begin);
            }
            return outputStream;
        }

        void log(string error)
        {
            var folder = @"D:\Debug_Log";
            var path = string.Format(@"D:\Debug_Log\{0}-{1}-{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            File.AppendAllText(path, error + "\n");
        }
    }
}
