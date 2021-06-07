using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CDO;

namespace GPS.Website
{
    public partial class EmailTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
            msg.Subject = "Warning";
            msg.HTMLBody = "This gtu's location is out of the boundary of the distribution map!";
            msg.From = "\"jliu\"<jliu@chuwasoft.com>";
            try
            {
                msg.Send();
            }
            catch (Exception ex) { 
            }
        }
    }
}
