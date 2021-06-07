using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using GPS.DataLayer;
using GPS.DomainLayer.Entities;

namespace TestWeb
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //User user = new UserRepository().GetUser("jimmyliu", "j1mmyliu");
            //Response.Write(user.Id.ToString());
        }
    }
}
