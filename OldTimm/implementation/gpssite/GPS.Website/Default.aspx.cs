using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.Web;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Interfaces;

namespace GPS.Website
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (null == Session["USER"])
            {
                Response.Redirect("login.html");
            }
            else 
            {
                Response.Redirect("Campaign.aspx");
            }
        }
    }
}
