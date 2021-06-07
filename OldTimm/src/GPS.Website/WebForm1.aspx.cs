using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterHiddenField("__EVENTTARGET", this.RadioButton1.ClientID);
            Page.RegisterHiddenField("__EVENTARGUMENT", this.RadioButton1.ClientID);
        }

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
