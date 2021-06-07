using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.Web;

namespace GPS.Website
{
    public partial class NewControlCenter : SecurityPage
    {
        protected string m_BinMap;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_BinMap = System.Web.Configuration.WebConfigurationManager.AppSettings["BinMapKey"];
        }
    }
}