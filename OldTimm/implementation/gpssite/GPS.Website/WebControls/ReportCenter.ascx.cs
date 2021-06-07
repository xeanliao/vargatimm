using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website.WebControls
{
    public partial class ReportCenter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.btnDismissToMonitor.Attributes.Add("onclick", "return onDismissToMonitor(this)");
            }
        }

        protected void btnDismissToMonitor_Click(object sender, EventArgs e)
        {
            string selectedTaskIdList = this.txtSelectedTaskIdList.Value;
            string[] arrTask = selectedTaskIdList.Split(',');
            int id = 0;

            GPS.DataLayer.RepositoryImplementations.TaskRepository taskRep = new GPS.DataLayer.RepositoryImplementations.TaskRepository();
            foreach (string sID in arrTask)
            {
                if (sID.Trim() == "") continue;
                if(int.TryParse(sID, out id) == true)
                    taskRep.MoveReportBackToTask(id);
            }
        }
    }
}