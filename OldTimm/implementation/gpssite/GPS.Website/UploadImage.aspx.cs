using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website
{
    public partial class UploadImage : System.Web.UI.Page
    {
        public static string UPLOADEDIMAGE = "uploadedimage";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session[UPLOADEDIMAGE] = "";
                fileTestUpload.Attributes.Add("onchange", string.Format("onSelectedImage('{0}')", btnUpload.ClientID));

                if (Request["img"] != null)
                {
                    this.imgUploaded.ImageUrl = "/pictures/" + Request["img"];
                }
            }
        }

        protected void btnUpload_Click(object sender, System.EventArgs e)// Handles btnUpload.Click
        {
            // when upload button is clicked, save the filename to the session
            if (fileTestUpload.HasFile == false) return;

            string sFileName = fileTestUpload.FileName;
            string sExt = sFileName.Substring(sFileName.LastIndexOf("."));
            string sNewFile = Guid.NewGuid().ToString() + sExt;
            string sPath = Server.MapPath("/pictures/");
            if (System.IO.Directory.Exists(sPath) == false)
                System.IO.Directory.CreateDirectory(sPath);

            fileTestUpload.SaveAs(sPath + sNewFile);
            this.imgUploaded.ImageUrl = "/pictures/" + sNewFile;
            Session[UPLOADEDIMAGE] = sNewFile;

            /*
            // the following code triggers upload completion event in client side
            ClientScript.RegisterStartupScript(Page.GetType(), "Upload Completed", "window.parent.iFrame_OnUploadComplete();", true);
            */
        }
    }
}