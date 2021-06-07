using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Data;

namespace GPS.Website
{
    public partial class PreLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                LoadVersionsFromXml();
        }

        public void LoadVersionsFromXml()
        {
            string xmlFile = Server.MapPath("~/VersionList.xml");
            DataSet dsVersion = new DataSet();
            try
            {
                dsVersion.ReadXml(xmlFile);
                dplVersion.DataSource = dsVersion;
                dplVersion.DataTextField = "ConnectName";
                dplVersion.DataValueField = "ConnectURl";
                dplVersion.DataBind();
            }
            catch (Exception ex)
            {
                GPS.Utilities.LogUtils.Error("Web application Unhandle error", ex);
                Response.Write(ex.ToString());
            }
        }


        protected void btnGoLogin_Click(object sender, EventArgs e) 
        {
            string urlStr = dplVersion.SelectedValue;
            if (urlStr != string.Empty)
                Response.Redirect(urlStr + "/login.html");
            else
                ClientScript.RegisterStartupScript(this.GetType(),"alert", "<script language= 'javascript'>alert('DataBase Name is required!');</script>");
        }
    }
}
