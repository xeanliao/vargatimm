using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Collections;
using GPS.Web;

namespace GPS.Website
{
    public partial class AdminGtuToEmployee : SecurityPage
    {
        private int taskID
        {
            set
            {
                ViewState["TaskID"] = value;
            }
            get
            {
                return Convert.ToInt32(ViewState["TaskID"]);
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["taskId"] == null)
                    return;
                taskID = Convert.ToInt32(Request["taskid"]);

                // get Auditor
                DAL.TimmDomainService timm = new DAL.TimmDomainService();
                DAL.task t = timm.GetTaskByID(taskID);
                int iUserID = Convert.ToInt32(t.AuditorId);

                lblBreadcrumb.Text = DAL.TaskDB.GetCampaignDisplayByTaskID(taskID) + " > " + t.Name;

                // display no-signal Gtus
                List<DAL.gtu> noSigmalGtus = DAL.GtuDB.GetGtus_NoSignal(iUserID);
                this.gridNoSignalGtus.DataSource = noSigmalGtus;
                this.gridNoSignalGtus.DataBind();
            }
        }

        protected void btnCancelEmployeeAdd_Click(object sender, EventArgs e)
        {
            this.ctlAddEmployee.ClearForm();
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            this.ctlAddEmployee.SaveEmployee();
            this.ctlAddEmployee.ClearForm();
        }

        // show short gtuNumber in GridView
        protected string ShowShortGtuNumber(string sUniqueID)
        {
            return sUniqueID.Substring(Math.Max(0, sUniqueID.Length - 6));
        }

    }
}