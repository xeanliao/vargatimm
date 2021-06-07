using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net.Mail;
using System.Net;
// using CDO;
using System.Configuration;
using System.Text.RegularExpressions; 

namespace TIMM.GTUService.Service
{
    public class Email
    {
        public Email()
        {
        }

        //public void CDOEmailSend(string userName, string addresses, string uid, string outTime)
        //{
        //    if (addresses == string.Empty) return;
        //    CDO.Configuration conf = new CDO.Configuration();
        //    //conf.Fields[CdoConfiguration.cdoSendUsingMethod].Value = CdoSendUsing.cdoSendUsingPort;
        //    //conf.Fields[CdoConfiguration.cdoSMTPAuthenticate].Value = CdoProtocolsAuthentication.cdoBasic;
        //    //string cdoSMTPServer = ConfigurationManager.AppSettings["cdoSMTPServer"].ToString();
        //    //string cdoSMTPServerPort = ConfigurationManager.AppSettings["cdoSMTPServerPort"].ToString();
        //    //string cdoSMTPAccountName = ConfigurationManager.AppSettings["cdoSMTPAccountName"].ToString();
        //    //string cdoSendUserName = ConfigurationManager.AppSettings["cdoSMTPAccountName"].ToString();
        //    //string cdoSendPassword = ConfigurationManager.AppSettings["cdoSendPassword"].ToString();

        //    //string cdoSendUserReplyEmailAddress = ConfigurationManager.AppSettings["cdoSendUserReplyEmailAddress"].ToString();
        //    //string cdoSendEmailAddress = ConfigurationManager.AppSettings["cdoSendEmailAddress"].ToString();

        //    conf.Fields[CdoConfiguration.cdoSMTPServer].Value = "mail.chuwasoft.com";
        //    conf.Fields[CdoConfiguration.cdoSMTPServerPort].Value = 25;
        //    conf.Fields[CdoConfiguration.cdoSMTPAccountName].Value = "jliu";
        //    conf.Fields[CdoConfiguration.cdoSendUserReplyEmailAddress].Value = "\"jliu\"<jliu@chuwasoft.com>";
        //    conf.Fields[CdoConfiguration.cdoSendEmailAddress].Value = "\"jliu\"<jliu@chuwasoft.com>";
        //    conf.Fields[CdoConfiguration.cdoSendUserName].Value = "jliu";
        //    conf.Fields[CdoConfiguration.cdoSendPassword].Value = "000000jliu";

        //    conf.Fields.Update();
        //    List<string> addLst = new List<string>();
        //    string[] addArray = addresses.Split(';');
        //    for (int i = 0; i < addArray.Length; i++)
        //        if (eAddressValidate(addArray[i]))
        //            addLst.Add(addArray[i]);
        //        else
        //            System.Diagnostics.Trace.TraceInformation("Illegal mail address:{0}\n", addArray[i]);
        //    if (addLst == null || addLst.Count == 0) return;
        //    //string fromstr = "\'" + cdoSMTPAccountName + "\'" + "<" + ConfigurationManager.AppSettings["cdoSendEmailAddress"].ToString() + ">";
        //    foreach (string add in addLst)
        //    {
        //        MessageClass msg = new MessageClass();
        //        msg.Configuration = conf;
        //        msg.To = add;
        //        msg.Subject = "Warning";
        //        msg.HTMLBody = userName + "The gtu(UniqueID:" + uid + ", User:" + userName + ") is out of the boundary of the distribution map at " + outTime + "!";
        //        msg.From = "\"jliu\"<jliu@chuwasoft.com>";
        //        try
        //        {
        //            msg.Send();
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }
        //}

        public void CDOEmailSend(bool isOut,string message,string userName, string addresses, string uid, string outTime)
        {
            string cdoSendEmailAddress = ConfigurationManager.AppSettings["cdoSendEmailAddress"].ToString();
            string cdoSMTPServer = ConfigurationManager.AppSettings["cdoSMTPServer"].ToString();
            string cdoSMTPServerPort = ConfigurationManager.AppSettings["cdoSMTPServerPort"].ToString();
            string cdoSMTPAccountName = ConfigurationManager.AppSettings["cdoSMTPAccountName"].ToString();
            string cdoSendUserName = ConfigurationManager.AppSettings["cdoSMTPAccountName"].ToString();
            string cdoSendPassword = ConfigurationManager.AppSettings["cdoSendPassword"].ToString();
            string cdoSSL = ConfigurationManager.AppSettings["cdoSSL"].ToString();

            System.Diagnostics.Trace.TraceInformation("Begin to CDOEmailSend \n");

            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                msg.From = new System.Net.Mail.MailAddress(cdoSendEmailAddress, cdoSMTPAccountName);
                string[] addArray = addresses.Split(';');
                for (int i = 0; i < addArray.Length; i++)
                    if (eAddressValidate(addArray[i]))
                        msg.To.Add(addArray[i]);
                    else
                        System.Diagnostics.Trace.TraceInformation("Illegal mail address:{0}\n", addArray[i]);
                if (msg.To == null || msg.To.Count == 0) return;
                msg.Subject = "WARNING";
                if (isOut)
                {
                    msg.Body = "The gtu(UniqueID:" + uid + ", User: " + userName + ") is out of the boundary of the distribution map(" + message + ") at " + outTime + "!";
                }
                else
                    msg.Body = "The gtu(UniqueID:" + uid + ", User: " + userName + ") is in the DND Area(" + message + ") at " + outTime + "!";

                msg.BodyEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                msg.Priority = System.Net.Mail.MailPriority.High;
                System.Net.Mail.SmtpClient cliect = new System.Net.Mail.SmtpClient(cdoSMTPServer, Convert.ToInt32(cdoSMTPServerPort));
                cliect.Credentials = new System.Net.NetworkCredential(cdoSMTPAccountName, cdoSendPassword);
                if (cdoSSL == "true" || cdoSSL == "1")
                {
                    cliect.EnableSsl = true;
                }
                cliect.Send(msg);
                System.Diagnostics.Trace.TraceInformation("Begin to CDOEmailSend \n");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceInformation("Begin to CDOEmailSend exception: {0} inner exception {1}\n", ex.ToString(), ex.InnerException);
            }
        }

        //public List<string> getAddressList(string str)
        //{
        //    List<string> addLst = new List<string>();
        //    string[] addArray = str.Split(';');
        //    for (int i = 0; i < addArray.Length; i++)
        //        if (eAddressValidate(addArray[i]))
        //            addLst.Add(addArray[i]);
        //    return addLst;
        //}

        public bool eAddressValidate(string str)
        {
            Regex re = new Regex(@"^((?'name'.+?)\s*<)?(?'email'(?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""(?'user'(?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?'angle'<))?(?'user'(?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(?'domain'((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?'angle')(?(name)>)$", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            return re.IsMatch(str);
        }
    }
}
