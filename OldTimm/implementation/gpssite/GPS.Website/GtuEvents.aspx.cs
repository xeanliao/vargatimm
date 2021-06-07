using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website
{
    public partial class GtuEvents : System.Web.UI.Page
    {
        private int mUserID
        {
            set
            {
                ViewState["userid"] = value;
            }
            get
            {
                return Convert.ToInt32(ViewState["userid"]);
            }
        }

        private List<DAL.gtustatushistory> mGtuEvents
        {
            // because there maybe too much data, here I use session to save traffic
            set
            {
                Session["gtuevents"] = value;
            }
            get
            {
                if (Session["gtuevents"] == null)
                    return null;
                return (List<DAL.gtustatushistory>)Session["gtuevents"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    // clear cache
                    this.mGtuEvents = null;

                    if (Request["userid"] == null)
                    {
                        Response.Write("Invalid request!");
                        Response.End();
                    }

                    mUserID = Convert.ToInt32(Request["userid"]);
                    this.mGtuEvents = DAL.GtuDB.GetGtuLastStatusHistoryByUserID(mUserID);
                    bindGtuEventGrid();

                    // populate EmployeeInfo
                    populateEmployeeInfo();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private void populateEmployeeInfo()
        {
            DAL.timmEntities context = new DAL.timmEntities();
            DAL.user mUser = context.users.Where(it => it.Id == mUserID).SingleOrDefault();

            this.tabEmployee.HeaderText = mUser.FullName;
            this.lblDistributor.Text = mUser.company.Name;
            this.lblCell.Text = "Cell: " + mUser.CellPhone;
            this.lblBirthdate.Text = string.Format("Birthdate: {0:MM/dd/yyyy}", mUser.DateOfBirth);
            this.lblNotes.Text = mUser.Notes;

            if (mUser.Picture != "")
            {
                string pictureUrl = DAL.ConfigUtils.GetConfiguration("PictureUrl");
                this.imgEmployee.ImageUrl = pictureUrl + mUser.Picture;
            }
        }

        private void bindGtuEventGrid()
        {
            this.gridGtuEvents.DataSource = this.mGtuEvents;
            this.gridGtuEvents.DataBind();
        }

        protected void gridGtuEvents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowIndex < 0)
                    return;

                DAL.gtustatushistory gtuEvent = (DAL.gtustatushistory)e.Row.DataItem;
                Label lblEvent = (Label)e.Row.FindControl("lblEventDetail");

                string uniqueID = DAL.GtuDB.GetGtuByID(gtuEvent.GTUId).ShortUniqueID;
                lblEvent.Text = string.Format("{0}: {1}", uniqueID, DAL.Lookups.GtuStatus[gtuEvent.StatusId]);
            }
            catch (Exception ex)
            {
                GPS.Utilities.DBLog.LogError(ex.ToString());
            }
        }


        protected void gridGtuEvents_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gridGtuEvents.PageIndex = e.NewPageIndex;
            this.bindGtuEventGrid();
        }

        protected void gridGtuEvents_Sorting(object sender, GridViewSortEventArgs e)
        {
            /*
            if (e.SortDirection == SortDirection.Ascending)
                dataView.Sort = e.SortExpression + " desc";
            else
                dataView.Sort = e.SortExpression + " asc";
            */
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        { 
            // populate EmployeeForm
            this.pnlEditEmployee.Visible = true;
            btnEdit.Visible = false;
            this.ctlAddEmployee.UserID = this.mUserID;
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                bool bSuccess = this.ctlAddEmployee.SaveEmployee();

                if (bSuccess)
                {
                    this.pnlEditEmployee.Visible = false;
                    btnEdit.Visible = true;
                    populateEmployeeInfo();
                }
            }
            catch (Exception ex)
            {
                GPS.Utilities.DBLog.LogError(ex.ToString());
                lblError.Text = ex.Message;
            }
        }

        protected void btnCancelEmployeeEdit_Click(object sender, EventArgs e)
        {
            this.pnlEditEmployee.Visible = false;
            btnEdit.Visible = true;
            populateEmployeeInfo();
        }
    }
}