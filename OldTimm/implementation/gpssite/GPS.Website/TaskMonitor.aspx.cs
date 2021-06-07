using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using GPS.Web;

namespace GPS.Website
{
    public partial class TaskMonitor : SecurityPage
    {
        protected int mTaskID
        {
            get
            {
                return Convert.ToInt32(ViewState["taskid"]);
            }
            set
            {
                ViewState["taskid"] = value;
            }
        }

        protected string m_BinMap;

        private int mDistributionMapID
        {
            set
            {
                ViewState["dmapid"] = value;
            }
            get
            {
                return Convert.ToInt32(ViewState["dmapid"]);
            }
        }

        private int mAuditorID
        {
            set { ViewState["auditorid"] = value; }
            get { return Convert.ToInt32(ViewState["auditorid"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_BinMap = System.Web.Configuration.WebConfigurationManager.AppSettings["BinMapKey"];

            if(!IsPostBack)
            {
                int iTaskID = Convert.ToInt32(Request["taskid"]);
                mTaskID = iTaskID;
                string url = Page.Request.Url.OriginalString;
                url = url.Substring(0, url.Length - Page.Request.Url.Query.Length - 16);
                txtPreviewUrl.Text = string.Format("{0}TaskMonitorView.aspx?{1}", url, EncryptTaskId(iTaskID));

                //uploadImage.Attributes.Add("onchange", string.Format("onUploadImage({0})", btnUpload.ClientID));

                // Get the Task, and related ditributionMapId
                GPS.Website.DAL.TimmDomainService timm = new Website.DAL.TimmDomainService();
                DAL.task t = timm.GetTaskByID(iTaskID);
                this.mDistributionMapID = t.DmId;

                GPS.Website.DAL.distributionmap dmap = timm.GetDistributionMapByDMapID(mDistributionMapID);
                lblBreadcrumb.Text = DAL.TaskDB.GetCampaignDisplayByTaskID(iTaskID) + " > " + dmap.Name;

                // from task to get Auditor
                DAL.user auditor = timm.GetTaskAuditor(iTaskID);
                this.lblAuditorName.Text = auditor.FullName;
                this.mAuditorID = auditor.Id;

                // Get Auditor's GTU
                DAL.gtu auditorGtu = DAL.GtuDB.GetGtuByUserID(auditor.Id);
                if(auditorGtu != null)
                    this.lblAuditorGtu.Text = auditorGtu.ShortUniqueID;

                // Get all Tasks belong to this Auditor
                ctlMonitorTaskGTUs.TaskID = this.mTaskID;

                // Get MonitorAddress list for this task
                populateMonitorAddressList();

                // populate UnassignedGtuGrid
                populateUnAssignedGtuGrid();
                btnAssignGtu.Attributes.Add("onclick", string.Format("return showAssignGtu('{0}')", mTaskID));
            }

            
        }

        private string EncryptTaskId(int id)
        {
            var query = string.Format("taskid={0}", id);
            byte[] desKey = { 51, 227, 53, 142, 160, 207, 169, 116 };
            byte[] desIV = { 235, 167, 205, 175, 6, 231, 7, 207 };

            using (DESCryptoServiceProvider desc = new DESCryptoServiceProvider())
            {
                MemoryStream ms = new MemoryStream();
                CryptoStream encStream = new CryptoStream(ms, desc.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write);
                StreamWriter writer = new StreamWriter(encStream, Encoding.UTF8);
                writer.WriteLine(query);
                writer.Close();
                encStream.Close();
                query = Convert.ToBase64String(ms.ToArray());
            }
            return HttpUtility.UrlEncode(query);
        }

        private void populateUnAssignedGtuGrid()
        {
            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            List<DAL.gtu> gtuList = timm.GetGtuListByUserID_NotAssigned(this.mAuditorID);

            this.UnassignedGtuGrid.DataSource = gtuList;
            this.UnassignedGtuGrid.DataBind();
        }

        protected string ShowShortGtuNumber(string sUniqueID)
        {
            return sUniqueID.Substring(Math.Max(0, sUniqueID.Length - 6));
        }

        private void populateMonitorAddressList()
        {
            List<DAL.monitoraddress> addressList = DAL.MonitorAddressDB.GetMonitorAddressListByDMapID(this.mDistributionMapID);
            this.AddressGrid.DataSource = addressList;
            this.AddressGrid.DataBind();
        }

        private void populateMonitorAddressForm(DAL.monitoraddress addr)
        {
            if (addr == null)
                addr = new DAL.monitoraddress();

            //txtAddressLine1.Text, txtZip.Text, sImage, mDistributionMapID    
            txtMonitorAddressID.Value = addr.Id.ToString();
            txtAddressLine1.Text = addr.Address;
            txtZip.Text = addr.ZipCode;

            bool bHasPicture = false;
            if (addr.Picture != null)
                if (addr.Picture.Length > 0)
                    bHasPicture = true;

            this.btnDeletePicture.Visible = bHasPicture;
            if (bHasPicture)
            {
                monitorAddressImageFrame.Attributes.Add("src", "UploadImage.aspx?img=" + addr.Picture);
                lblPictureUploadTime.Text = string.Format("Uploaded: {0:MM/dd/yyyy}", addr.PictureUploadTime);
            }
            else
            {
                monitorAddressImageFrame.Attributes.Add("src", "UploadImage.aspx");
                lblPictureUploadTime.Text = "";
            }

            if (addr.Id == 0)
                btnSave.Text = "Add Location";
            else
                btnSave.Text = "Save Changes";

            txtNotes.Text = addr.Notes;
        }

        protected void AddressGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex < 0) return;
            DAL.monitoraddress addr = (DAL.monitoraddress)e.Row.DataItem;

            ImageButton imgAdd = (ImageButton)e.Row.FindControl("btnAddPhoto");
            ImageButton imgRemove = (ImageButton)e.Row.FindControl("btnRemovePhoto");
            bool bHasImage = false;
            if (addr.Picture != null)
                if (addr.Picture.Length > 0)
                    bHasImage = true;

            imgAdd.Visible = !bHasImage;
            imgRemove.Visible = bHasImage;
        }

        protected void AddressGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int iAddressID = Convert.ToInt32(e.CommandArgument);

            string cmd = e.CommandName;
            switch (cmd)
            { 
                case "del":
                    DAL.MonitorAddressDB.DeleteMonitorAddress(iAddressID);
                    populateMonitorAddressList();
                    ScriptManager.RegisterClientScriptBlock(this.updatePanelMain, this.Page.GetType(), 
                        "refreshLocations", "<script>callSilverToRefreshLocations();</script>", false);

                    // when the one displayed is the one that user wants to delete, clear the form
                    if (iAddressID.ToString() == txtMonitorAddressID.Value)
                        populateMonitorAddressForm(null);
                    break;

                case "edt":
                case "addphoto":
                    DAL.monitoraddress addr = DAL.MonitorAddressDB.GetMonitorAddressByID(iAddressID);
                    populateMonitorAddressForm(addr);
                    break;

                case "removephoto":
                    DAL.MonitorAddressDB.RemoveMonitorAddressPhoto(iAddressID);
                    populateMonitorAddressList();
                    break;
            }
        }

        protected void btnSaveLocation_Click(object sender, EventArgs e)
        {
            try
            {
                string sImage = (string)Session[GPS.Website.UploadImage.UPLOADEDIMAGE];
                int iAddressID = Convert.ToInt32(txtMonitorAddressID.Value);
                DAL.MonitorAddressDB.SaveMonitorAddress(iAddressID, txtAddressLine1.Text, txtZip.Text, sImage, mDistributionMapID, txtNotes.Text.Trim(), mTaskID);

                // refresh address list and clean Form
                populateMonitorAddressList();
                populateMonitorAddressForm(null);

                // Added the new added monitorAddress to map
                ScriptManager.RegisterClientScriptBlock(this.updatePanelMain, this.Page.GetType(),
                    "refreshLocations", "<script>callSilverToRefreshLocations();</script>", false);
            }
            catch (Exception ex)
            {
                this.lblLocationError.Text = ex.Message;
            }
        }

        protected void btnCancelLocation_Click(object sender, EventArgs e)
        {
            populateMonitorAddressForm(null);
        }

        protected void btnDeletePicture_Clicked(object sender, EventArgs e)
        {
            if (this.txtMonitorAddressID.Value != "" && this.txtMonitorAddressID.Value != "0")
            {
                int iAddressID = Convert.ToInt32(this.txtMonitorAddressID.Value);
                DAL.MonitorAddressDB.RemoveMonitorAddressPhoto(iAddressID);
                populateMonitorAddressList();

                DAL.monitoraddress addr = DAL.MonitorAddressDB.GetMonitorAddressByID(iAddressID);
                populateMonitorAddressForm(addr);
            }
        }

        protected void btnAssignGTU_Click(object sender, EventArgs e)
        {
            // refresh status
            ctlMonitorTaskGTUs.TaskID = mTaskID;
        }

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // Refresh status
            ctlMonitorTaskGTUs.TaskID = mTaskID;
        }
    }
}