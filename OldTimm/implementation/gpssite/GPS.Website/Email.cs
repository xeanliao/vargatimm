using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.Net.Mail;
using System.Net;
using CDO;
using log4net;

namespace GPS.Website
{
    public class Email
    {
        //private static object lockHelper = new object(); 
        //private string _From; 
        //private string _FromEmail; 
        //private string _Subject; 
        //private string _Body; 
        //private string _SmtpServer; 
        //private string _SmtpPort = "25"; 
        //private string _SmtpUserName; 
        //private string _SmtpPassword; 
        //private System.Web.Mail.MailFormat _Format = System.Web.Mail.MailFormat.Html; 
        //private System.Text.Encoding _Encoding = System.Text.Encoding.Default;
        //public System.Web.Mail.MailFormat Format { set { _Format = value; } }        
        //public System.Text.Encoding Encoding { set { _Encoding = value; } }        
        //public string FromEmail { set { _FromEmail = value; } } 
        //public string From { set { _From = value; }}
        //public string Subject { set { _Subject = value; }}
        //public string Body { set { _Body = value; }}
        //public string SmtpServer { set { _SmtpServer = value; }}    
        //public string SmtpPort { set { _SmtpPort = value; }} 
        //public string SmtpUserName { set { _SmtpUserName = value; }}      
        //public string SmtpPassword { set { _SmtpPassword = value; }}

        public Email() {
        }

        public void CDOEmailSend() {
            /*
            CDO.Configuration conf = new CDO.Configuration();
            conf.Fields[CdoConfiguration.cdoSendUsingMethod].Value = CdoSendUsing.cdoSendUsingPort;
            conf.Fields[CdoConfiguration.cdoSMTPServer].Value = "mail.chuwasoft.com";
            conf.Fields[CdoConfiguration.cdoSMTPServerPort].Value = 25;
            conf.Fields[CdoConfiguration.cdoSMTPAccountName].Value = "jliu";
            conf.Fields[CdoConfiguration.cdoSendUserReplyEmailAddress].Value = "\"jliu\"<jliu@chuwasoft.com>";
            conf.Fields[CdoConfiguration.cdoSendEmailAddress].Value = "\"jliu\"<jliu@chuwasoft.com>";
            conf.Fields[CdoConfiguration.cdoSMTPAuthenticate].Value = CdoProtocolsAuthentication.cdoBasic;
            conf.Fields[CdoConfiguration.cdoSendUserName].Value = "jliu";
            conf.Fields[CdoConfiguration.cdoSendPassword].Value = "000000jliu";

            conf.Fields.Update();
            MessageClass msg = new MessageClass();
            msg.Configuration = conf;


            msg.To = "sprint_jia@tom.com";
            */

            System.Net.Mail.SmtpClient smtp = new SmtpClient("mail.chuwasoft.com", 25);
            System.Net.Mail.MailMessage msg = new MailMessage("jliu@chuwasoft.com", "sprint_jia@tom.com");
            
            msg.Subject = "Warning";
            msg.Body = "This gtu's location is out of the boundary of the distribution map!";
            msg.IsBodyHtml = true;
            
            try
            {
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                ILog logger = LogManager.GetLogger(GetType());
                logger.Error("Mail Service Unhandle Error", ex);
            }    
        }

        //public void CDOMessageSend(string toEmail, int sendusing)
        //{
        //    lock (lockHelper)
        //    {
        //        CDO.Message objMail = new CDO.Message();
        //        try
        //        {
        //            objMail.To = toEmail;
        //            objMail.From = _FromEmail;
        //            objMail.Subject = _Subject;
        //            if (_Format.Equals(System.Web.Mail.MailFormat.Html)) objMail.HTMLBody = _Body;
        //            else objMail.TextBody = _Body;
        //            if (!_SmtpPort.Equals("25")) objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"].Value = _SmtpPort; //设置端口                    
        //            objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"].Value = _SmtpServer;
        //            objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value = sendusing;
        //            objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendemailaddress"].Value = _FromEmail;
        //            objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpuserreplyemailaddress"].Value = _FromEmail;
        //            objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpaccountname"].Value = _SmtpUserName;
        //            objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"].Value = _SmtpUserName;
        //            objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"].Value = _SmtpPassword;
        //            objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"].Value = 1;
        //            objMail.Configuration.Fields.Update();
        //            objMail.Send();
        //            //bool isTrue = SendMail(toEmail, _Subject, _Body, _FromEmail, _SmtpServer, _SmtpUserName, _SmtpPassword, "");
        //            return true;
        //        }
        //        catch { }
        //        finally { }
        //        return false;
        //    }
        //}

        //public void DoSendEmail()
        //{
        //    if (CDOMessageSend(""))
        //    {
        //        string strTo = objMail.To;
        //        string strSubject = objMail.Subject;
        //        string strBody = objMail.Body;
        //        string strFrom = objMail.From;
        //        string smtpServer = objMail.SmtpServer;
        //        string userName = objMail.UserName;
        //        string password = objMail.Password;
        //        bool isTrue = SendMail(strTo, strSubject, strBody, strFrom, smtpServer, userName, password, "");
        //    }
        //}       
        //public void SendMail(string strTo, string strSubject, string strBody,
        //    string strFrom, string smtpServer, string userName,
        //    string password, string attachments)
        //{
        //    Email email = new Email();
        //    email.SmtpServer = smtpServer;
        //    email.SmtpUserName = userName;
        //    email.SmtpPassword = password;
        //    email.SmtpPort = smtpServer.ToLower().Contains("gmail") ? "587" : "25";
        //    //email.EnableSsl = smtpServer.ToLower().Contains("gmail") ? true : false;            
        //    email.FromEmail = strFrom;
        //    email.Subject = strSubject;
        //    email.Body = strBody;
        //    email.Encoding = System.Text.Encoding.UTF8;
        //    bool isSuccess = email.SmtpClientSend(strTo);
        //    return isSuccess;
        //}

        //public void SmtpClientSend(string toEmail)
        //{
        //    lock (lockHelper)
        //    {
        //        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(_FromEmail, toEmail, _Subject, _Body);
        //        message.SubjectEncoding = _Encoding;
        //        message.BodyEncoding = _Encoding;
        //        message.IsBodyHtml = true;
        //        message.Priority = MailPriority.High;
        //        SmtpClient client = new SmtpClient(_SmtpServer);
        //        client.UseDefaultCredentials = false;
        //        client.Credentials = new NetworkCredential(_SmtpUserName, _SmtpPassword);
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.Port = Int32.Parse(_SmtpPort);
        //        client.EnableSsl = true;
        //        try
        //        {
        //            client.Send(message);
        //        }
        //        catch
        //        {
        //            return false;
        //        } return true;
        //    }
        //}    

    }
}
