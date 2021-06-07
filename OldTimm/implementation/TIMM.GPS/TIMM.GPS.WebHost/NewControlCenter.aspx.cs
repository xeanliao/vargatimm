using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TIMM.GPS.WebHost
{
    public partial class NewControlCenter : System.Web.UI.Page
    {
        protected string m_BinMap;
        protected void Page_Load(object sender, EventArgs e)
        {
            m_BinMap = System.Web.Configuration.WebConfigurationManager.AppSettings["BingMapKey"];
        }
    }
}