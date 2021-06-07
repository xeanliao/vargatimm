using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Security;

namespace GPS.Web
{
    public class SecurityPage : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (LoginMember.CurrentMember == null)
            {
                Response.Redirect("login.html");
            }
            base.OnInit(e);
        }
    }
}
