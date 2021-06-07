using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website.Print
{
    public partial class Footer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ltlCampaign.Text = Request.QueryString["campaign"];
            ltlPreparedOn.Text = Request.QueryString["on"];
            ltlPreparedFor.Text = Request.QueryString["for"];
            ltlPreparedBy.Text = Request.QueryString["by"];

            this.imgVargaLogo.ImageUrl = WebUtil.GetAbsoluteUrl(this.Request, "Images/vargainc-logo.png");
            this.imgTimmLogo.ImageUrl = WebUtil.GetAbsoluteUrl(this.Request, "Images/timm-logo.png");
        }

    }
}
