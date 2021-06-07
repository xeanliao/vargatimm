using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace GPS.Website
{
    public partial class TaskMonitorView : System.Web.UI.Page
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

        protected string Message { get; set; }

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

        private int? DecryptTaskId()
        {
            try
            {
                var query = Page.Request.Url.Query.Substring(1);
                query = HttpUtility.UrlDecode(query, Encoding.UTF8);
                byte[] queryByte = Convert.FromBase64String(query);
                //can not modified desKey and desIV these are random number
                byte[] desKey = { 51, 227, 53, 142, 160, 207, 169, 116 };
                byte[] desIV = { 235, 167, 205, 175, 6, 231, 7, 207 };

                using (DESCryptoServiceProvider desc = new DESCryptoServiceProvider())
                {
                    MemoryStream ms = new MemoryStream(queryByte);
                    CryptoStream encStream = new CryptoStream(ms, desc.CreateDecryptor(desKey, desIV), CryptoStreamMode.Read);
                    StreamReader reader = new StreamReader(encStream, Encoding.UTF8);
                    query = reader.ReadLine();
                }

                if (query.StartsWith("taskid=", StringComparison.CurrentCultureIgnoreCase))
                {
                    int index = query.IndexOf("&");
                    if (index > -1)
                    {
                        query = query.Substring(7, index);
                    }
                    else
                    {
                        query = query.Substring(7);
                    }

                    int id;
                    if (int.TryParse(query, out id))
                    {
                        return id;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        protected string m_BinMap;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_BinMap = System.Web.Configuration.WebConfigurationManager.AppSettings["BinMapKey"];


            var taskId = DecryptTaskId();
            if (!taskId.HasValue)
            {
                Response.Clear();
                throw new HttpException(404, "Are you sure you're in the right place?");
            }
            int iTaskID = taskId.Value;
            mTaskID = iTaskID;

            //check task status
            DAL.tasktime ttime = DAL.TaskDB.GetTaskTimeByTaskID(iTaskID);
            if (ttime != null)
            {
                switch (ttime.TimeType)
                {
                    case 0:
                        //started
                        mainContent.Visible = true;
                        break;
                    case 1:
                        //stopped
                        Message = "<br />The task is completed! Can't be monitored any more. ";
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "Message", "$(function() {$('.ui-dialog-titlebar-close').remove();$('#message').dialog('open');});", true);
                        mainContent.Visible = false;
                        return;
                        break;
                    case 2:
                        //paused
                        Message = "<br />The task is paused! Please check back later";
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "Message", "$(function() {$('#message').dialog('open');});", true);
                        mainContent.Visible = true;
                        break;
                    default:
                        //pending
                        Message = "<br />The task is pending! Please check back later";
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "Message", "$(function() {$('.ui-dialog-titlebar-close').remove();$('#message').dialog('open');});", true);
                        mainContent.Visible = false;
                        return;
                        break;
                }
            }
            else
            {
                Message = "<br />The task is pending! Please check back later";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Message", "$(function() {$('.ui-dialog-titlebar-close').remove();$('#message').dialog('open');});", true);
                mainContent.Visible = false;
                return;
            }

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
            if (auditorGtu != null)
                this.lblAuditorGtu.Text = auditorGtu.ShortUniqueID;

            // Get all Tasks belong to this Auditor
            ctlMonitorTaskGTUs.TaskID = this.mTaskID;

            // populate UnassignedGtuGrid
            populateUnAssignedGtuGrid();

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

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // Refresh status
            ctlMonitorTaskGTUs.TaskID = mTaskID;
        }
    }
}