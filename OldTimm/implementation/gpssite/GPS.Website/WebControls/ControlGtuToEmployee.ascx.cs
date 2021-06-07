using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website.WebControls
{
    public partial class ControlGtuToEmployee : System.Web.UI.UserControl
    {
        private List<DAL.company> mCompanyList
        {
            set
            {
                ViewState["companyList"] = value;
            }
            get
            {
                return (List<DAL.company>)ViewState["companyList"];
            }
        }

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

        private List<DAL.ViewGtuInTask> mGtuList
        {
            set
            {
                ViewState["gtulist"] = value;
            }
            get
            {
                if (ViewState["gtulist"] == null) return null;
                return (List<DAL.ViewGtuInTask>)ViewState["gtulist"];
            }
        }

        // by default we only display Assigned Gtus
        public bool AssignedGtus
        {
            set
            {
                ViewState["AssignedGtus"] = value;
            }
            get
            {
                if (ViewState["AssignedGtus"] == null)
                    return true;

                return Convert.ToBoolean(ViewState["AssignedGtus"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["taskId"] == null)
                    return;
                taskID = Convert.ToInt32(Request["taskid"]);

                // Get all Distributor
                DAL.TimmDomainService timm = new DAL.TimmDomainService();
                List<DAL.company> distrabutors = timm.GetCompanies();
                mCompanyList = distrabutors;

                // LoadGtuEmployeeMap;
                this.Reload();
            }
        }

        public void Reload()
        {
            DAL.TimmDomainService timm = new DAL.TimmDomainService();

            // Get all Gtus
            List<DAL.ViewGtuInTask> gtuList = null;
            if (AssignedGtus)
            {
                gtuList = timm.GetGtuListByTaskID(this.taskID);
            }
            else
            {
                gtuList = new List<DAL.ViewGtuInTask>();
            }
            //{
                DAL.task t = DAL.TaskDB.GetTaskByID(taskID);
                int iUserID = Convert.ToInt32(t.AuditorId);
                List<DAL.gtu> gtus = timm.GetGtuListByUserID_NotAssigned(iUserID);

                // gtuList = new List<DAL.GtuInTask>();
                foreach (DAL.gtu g in gtus)
                {
                    DAL.ViewGtuInTask gtuEmployee = new DAL.ViewGtuInTask();
                    gtuEmployee.GtuID = g.Id;
                    gtuEmployee.UniqueID = DAL.GtuDB.GetShortGtuNo(g.UniqueID);
                    gtuList.Add(gtuEmployee);
                }
            //}
                this.mGtuList = gtuList;

            GtuEmployeeGrid.DataSource = gtuList;
            GtuEmployeeGrid.DataBind();
        }

        protected void GtuEmployeeGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex < 0) return;

            DropDownList companyDropDown = (DropDownList)e.Row.FindControl("companyDropDown");
            companyDropDown.DataSource = this.mCompanyList;
            companyDropDown.DataBind();
            companyDropDown.Items.Insert(0, new ListItem("Distributor", "0"));

            //popuplate existing value
            DAL.ViewGtuInTask g = (DAL.ViewGtuInTask)e.Row.DataItem;
            if(g.CompanyId != 0)
            {
                companyDropDown.SelectedValue = g.CompanyId.ToString();
            }

            // Get employees under this company
            DropDownList empDropDown = (DropDownList)e.Row.FindControl("employeeDropDown");
            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            List<DAL.user> userList = timm.GetUsersByCompanyID(g.CompanyId);

            if (userList.Count > 0)
            {
                empDropDown.DataSource = userList;
                empDropDown.DataBind();
                empDropDown.SelectedValue = g.UserID.ToString();

                // populate employeeRole
                Label lblRole = (Label)e.Row.FindControl("lblEmployeeRole");
                if(DAL.Lookups.EmployeeRoles.IndexOfKey(g.UserRoleID) >= 0)
                    lblRole.Text = DAL.Lookups.EmployeeRoles[g.UserRoleID];
            }

            ImageButton imgButton = (ImageButton)e.Row.FindControl("btnDisconnect");
            imgButton.CommandArgument = e.Row.RowIndex.ToString();
            imgButton.Visible = (empDropDown.SelectedIndex >= 0);
        }

        protected void GtuEmployeeGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            //e.SortExpression
            List<DAL.ViewGtuInTask> gtuList = mGtuList;
            if (e.SortExpression.ToLower() == "uniqueid")
            {
                if (Convert.ToString(ViewState["orderByGtu"]) == "a")
                {
                    ViewState["orderByGtu"] = "d";
                    mGtuList = gtuList.OrderByDescending(it => it.UniqueID).ToList();
                }
                else
                {
                    ViewState["orderByGtu"] = "a";
                    mGtuList = gtuList.OrderBy(it => it.UniqueID).ToList();
                }
            }

            GtuEmployeeGrid.DataSource = mGtuList;
            GtuEmployeeGrid.DataBind();
        }

        protected void CompanyDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the current row
            DropDownList companyDropDown = (DropDownList)sender;
            GridViewRow row = (GridViewRow)companyDropDown.NamingContainer;
            DropDownList empDropDown = (DropDownList)row.FindControl("employeeDropDown");
            Label lblRole = (Label)row.FindControl("lblEmployeeRole");

            if (companyDropDown.SelectedIndex <= 0)
            {
                empDropDown.Items.Clear();
                lblRole.Text = "";
            }
            else
            {
                // Get employees under this company
                int iCompanyId = Convert.ToInt32(companyDropDown.SelectedValue);
                DAL.TimmDomainService timm = new DAL.TimmDomainService();
                List<DAL.user> userList = timm.GetUsersByCompanyID(iCompanyId);
                empDropDown.DataSource = userList;
                empDropDown.DataBind();

                // display employeeRole
                if (userList.Count > 0)
                {
                    DAL.user u = userList[empDropDown.SelectedIndex];
                    lblRole.Text = DAL.Lookups.EmployeeRoles[u.Role];
                }
                else
                {
                    lblRole.Text = "";
                }
            }
        }

        protected void employeeRoleDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList empDropDown = (DropDownList)sender;
            GridViewRow row = (GridViewRow)empDropDown.NamingContainer;

            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            int iUserID = Convert.ToInt32(empDropDown.SelectedValue);
            DAL.user u = timm.GetUserByID(iUserID);

            Label lblRole = (Label)row.FindControl("lblEmployeeRole");
            lblRole.Text = DAL.Lookups.EmployeeRoles[u.Role];
        }

        protected void GtuEmployeeGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "disconnect")
            {
                // get the current row
                int iRowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = this.GtuEmployeeGrid.Rows[iRowIndex];

                DropDownList companyDropDown = (DropDownList)row.FindControl("companyDropDown");
                DropDownList empDropDown = (DropDownList)row.FindControl("employeeDropDown");
                Label lblRole = (Label)row.FindControl("lblEmployeeRole");

                companyDropDown.SelectedIndex = 0;
                empDropDown.Items.Clear();
                lblRole.Text = "";
            }
        }

        public void Save()
        {
            /*
            foreach (GridViewRow row in this.GtuEmployeeGrid.Rows)
            {
                int gtuID = Convert.ToInt32(this.GtuEmployeeGrid.DataKeys[row.RowIndex].Value);
                DropDownList empDropDown = (DropDownList)row.FindControl("employeeDropDown");

                int userID = 0; // no employee is selected, un-assigning
                if (empDropDown.Items.Count > 0)
                    userID = Convert.ToInt32(empDropDown.SelectedValue);

                //DAL.TimmDomainService timm = new DAL.TimmDomainService();
                //timm.AssignGtuToEmployee(gtuID, userID, this.taskID);
                DAL.GtuDB.SetGtuUserMapping(gtuID, userID, this.taskID);
            }
            */
        }

        public List<int> GetAssignedUsers()
        {
            List<int> assignedUsers = new List<int>();
            foreach (GridViewRow row in this.GtuEmployeeGrid.Rows)
            {
                DropDownList empDropDown = (DropDownList)row.FindControl("employeeDropDown");
                if (empDropDown.Items.Count > 0)
                    assignedUsers.Add( Convert.ToInt32(empDropDown.SelectedValue) );
            }
            return assignedUsers;
        }

        protected string ShowShortGtuNumber(string sUniqueID)
        {
            return sUniqueID.Substring(Math.Max(0, sUniqueID.Length - 6));
        }
    }
}