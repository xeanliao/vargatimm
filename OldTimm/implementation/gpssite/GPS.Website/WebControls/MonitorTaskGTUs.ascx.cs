using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website.WebControls
{
    public partial class MonitorTaskGTUs : System.Web.UI.UserControl
    {
        public int TaskID
        {
            set
            {
                txtTaskID.Value = value.ToString();
                populateUI();
            }
            get
            {
                return Convert.ToInt32(txtTaskID.Value);
            }
        }
        
        public bool ViewOnly
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void populateUI()
        { 
            // Get Distribution Name
            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            DAL.distributionmap dmap = timm.GetDistributionMapByTaskID(TaskID);
            this.lblDMapName.Text = dmap.Name;

            // Get all GTUs for this task
            List<DAL.ViewGtuInTask> gtuList = timm.GetGtuListByTaskID(this.TaskID);
            this.GtuStatusGrid.DataSource = gtuList;
            this.GtuStatusGrid.DataBind();

            DAL.tasktime ttime = DAL.TaskDB.GetTaskTimeByTaskID(TaskID);
            if (ttime != null)
            {
                switch (ttime.TimeType)
                { 
                    case 0:
                        ShowControlButtons(EnumTaskStatus.started);
                        break;
                    case 1:
                        ShowControlButtons(EnumTaskStatus.stopped);
                        break;
                    case 2:
                        ShowControlButtons(EnumTaskStatus.paused);
                        break;
                    default:
                        ShowControlButtons(EnumTaskStatus.pending);
                        break;
                }
            }
            else
                ShowControlButtons(EnumTaskStatus.pending);

            this.GtuStatusGrid.RowDataBound += new GridViewRowEventHandler(GtuStatusGrid_RowDataBound);
        }

        protected void GtuStatusGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowIndex < 0) return;
                DAL.ViewGtuInTask gtu = (DAL.ViewGtuInTask)e.Row.DataItem;
                
                // show Gtu Role, or Gtu color for walker
                Image imgUserRole = (Image)e.Row.FindControl("imgUserRole");
                if (gtu.UserRoleID == 48)
                    imgUserRole.ImageUrl = "~/Images/icons/car_add.png";
                else if (gtu.UserRoleID == 50)
                    imgUserRole.ImageUrl = "~/images/icons/user_green.png";
                else
                    imgUserRole.ImageUrl = "~/drawdot.aspx?hexcolor=" + gtu.UserColor.Replace("#", "");

                // show Gtu-Status-Image
                Image imgGtuStatus = (Image)e.Row.FindControl("imgGtuStatus");
                int iStatusID = DAL.GtuDB.GetGtuCurrentStatusID(gtu.GtuID);
                if(DAL.Lookups.GtuStatus.IndexOfKey(iStatusID)>=0)
                    imgGtuStatus.ToolTip = DAL.Lookups.GtuStatus[iStatusID];

                switch (iStatusID)
                {
                    case 0:
                        imgGtuStatus.ImageUrl = "~/images/blank.png";
                        break;
                    case (int)DAL.GtuStatusEum.Duplicate_path:
                        imgGtuStatus.ImageUrl = "~/images/icons/error.png";
                        break;
                    case (int)DAL.GtuStatusEum.No_GTU_Signal:
                        imgGtuStatus.ImageUrl = "~/images/icons/stop.png";
                        break;
                    default:
                        imgGtuStatus.ImageUrl = "~/images/icons/exclamation.png";
                        break;
                }
            }
            catch (Exception ex)
            {
                GPS.Utilities.DBLog.LogError(ex.ToString());
            }
        }

        protected string getStatusImage(int iStatusID)
        {
            switch (iStatusID)
            { 
                case 0:
                    return "/images/blank.png";

                case (int)DAL.GtuStatusEum.Duplicate_path:
                    return "/images/icons/error.png";

                case (int)DAL.GtuStatusEum.No_GTU_Signal:
                    return "/images/icons/stop.png";

                default:
                    return "/images/icons/exclamation.png";
            }
        }

        protected void imgPlay_Click(object sender, EventArgs e)
        {
            int iTaskID = this.TaskID;
            DAL.TaskDB.StartTask(iTaskID);
            ShowControlButtons(EnumTaskStatus.started);
        }

        protected void imgPause_Click(object sender, EventArgs e)
        {
            int iTaskID = this.TaskID;
            DAL.TaskDB.PauseTask(iTaskID);
            ShowControlButtons(EnumTaskStatus.paused);
        }

        protected void imgStop_Click(object sender, EventArgs e)
        {
            int iTaskID = this.TaskID;
            DAL.TaskDB.StopTask(iTaskID);
            ShowControlButtons(EnumTaskStatus.stopped);
        }

        private void ShowControlButtons(EnumTaskStatus tskStatus)
        {
            if (ViewOnly)
            {
                imgPlay.Visible = false;
                imgPause.Visible = false;
                imgStop.Visible = false;
            }
            else
            {
                imgPlay.Visible = (tskStatus == EnumTaskStatus.pending | tskStatus == EnumTaskStatus.paused);
                imgPause.Visible = (tskStatus == EnumTaskStatus.started);
                imgStop.Visible = (tskStatus == EnumTaskStatus.started | tskStatus == EnumTaskStatus.paused);
            }
            lblTaskStatus.Text = tskStatus.ToString();
        }
    }

    public enum EnumTaskStatus : int
    { 
        pending = -1,
        started = 0,
        paused = 2,
        stopped = 1
    }
}