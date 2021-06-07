using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace GPS.Website.WebControls
{
    public partial class AdminGtuToEmployeeControl : System.Web.UI.UserControl
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
                Session["gtulist"] = value;
            }
            get
            {
                if (Session["gtulist"] == null) return null;
                return (List<DAL.ViewGtuInTask>)Session["gtulist"];
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
                foreach (DAL.company c in distrabutors)
                    this.dropDistributor.Items.Add(new ListItem(c.Name, c.Id.ToString()));

                this.mCompanyList = distrabutors;
                // LoadGtuEmployeeMap;
                this.Reload();

                string sScript = string.Format("javascript:document.getElementById('{0}').style.display='none'; return false;", pnlConnect.ClientID);
                btnCancelConnect.Attributes.Add("onclick", sScript);
            }
        }

        protected void dropDistributor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dropDistributor.SelectedIndex <= 0)
            {
                this.dropEmployee.Items.Clear();
                return;
            }

            int iDistributor = Convert.ToInt32(dropDistributor.SelectedValue);
            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            List<DAL.user> userList = timm.GetUsersByCompanyID(iDistributor);

            this.dropEmployee.DataSource = userList;
            this.dropEmployee.DataBind();

            if (userList.Count > 0)
            {
                // display employeeRole
                DAL.user u = userList[this.dropEmployee.SelectedIndex];
                lblRole.Text = DAL.Lookups.EmployeeRoles[u.Role];
            }
            else
                lblRole.Text = "";
        }

        protected void dropEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Change Role when employee is changed
            int iUserID = Convert.ToInt32(this.dropEmployee.SelectedValue);
            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            DAL.user u = timm.GetUserByID(iUserID);
            lblRole.Text = DAL.Lookups.EmployeeRoles[u.Role];
        }

        protected void btnSaveMapping_Click(object sender, EventArgs e)
        {
            try
            {
                // make sure the Gtu and the user and not inside a mapping
                int iGtuID = Convert.ToInt32(txtGtuID.Value);
                int iUserID = Convert.ToInt32(dropEmployee.SelectedValue);

                DAL.GtuDB.AssignGtuToEmployee(iGtuID, iUserID, this.taskID, "#" + txtColor.Text);
                Reload();
                clearMappingForm();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        public void Reload()
        {
            DAL.TimmDomainService timm = new DAL.TimmDomainService();

            // Get Gtus in mapping
            List<DAL.ViewGtuInTask> gtuList = timm.GetGtuListByTaskID(this.taskID);

            DAL.task t = DAL.TaskDB.GetTaskByID(taskID);
            int iUserID = Convert.ToInt32(t.AuditorId);
            List<DAL.gtu> gtus = timm.GetGtuListByUserID_NotAssigned(iUserID);

            foreach (DAL.gtu g in gtus)
            {
                DAL.ViewGtuInTask gtuEmployee = new DAL.ViewGtuInTask();
                gtuEmployee.GtuID = g.Id;
                gtuEmployee.UniqueID = g.ShortUniqueID;
                gtuList.Add(gtuEmployee);
            }
            //}
            this.mGtuList = gtuList;

            GtuEmployeeGrid.DataSource = gtuList;
            GtuEmployeeGrid.DataBind();

            IsAssignAllEnable(gtuList);
        }

        private System.Drawing.Color GetColorFromHex(string hexColor)
        {
            try
            {
                if (hexColor.StartsWith("#"))
                    hexColor = hexColor.Substring(1);

                int iRed = Convert.ToInt32(hexColor.Substring(0, 2), 16);
                int iGreen = Convert.ToInt32(hexColor.Substring(2, 2), 16);
                int iBlue = Convert.ToInt32(hexColor.Substring(4, 2), 16);
                return System.Drawing.Color.FromArgb(iRed, iGreen, iBlue);
            }
            catch (Exception)
            {
                return System.Drawing.Color.White;
            }
        }

        protected void GtuEmployeeGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex < 0) return;
            Label lblDistributorName = (Label)e.Row.FindControl("lblDistributorName");
            Label lblEmployeeRole = (Label)e.Row.FindControl("lblEmployeeRole");

            DAL.ViewGtuInTask g = (DAL.ViewGtuInTask)e.Row.DataItem;
            if (g.CompanyId != 0)
            {
                DAL.company oCompany = mCompanyList.Where(it => it.Id == g.CompanyId).FirstOrDefault();
                if (oCompany != null)
                    lblDistributorName.Text = oCompany.Name;
            }

            if (g.UserRoleID != null)
                if (g.UserRoleID > 0)
                    lblEmployeeRole.Text = DAL.Lookups.EmployeeRoles[g.UserRoleID];

            Panel pnlGtuColor = (Panel)e.Row.FindControl("pnlGtuColor");
            pnlGtuColor.Visible = (g.TaskId != 0);
            if (g.TaskId > 0)
                pnlGtuColor.BackColor = this.GetColorFromHex(g.UserColor);

            /*
                        DropDownList companyDropDown = (DropDownList)e.Row.FindControl("companyDropDown");
                        companyDropDown.DataSource = this.mCompanyList;
                        companyDropDown.DataBind();
                        companyDropDown.Items.Insert(0, new ListItem("Distributor", "0"));

                        //popuplate existing value
                        DAL.ViewGtuInTask g = (DAL.ViewGtuInTask)e.Row.DataItem;
                        if (g.CompanyId != 0)
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
                            if (DAL.Lookups.EmployeeRoles.IndexOfKey(g.UserRoleID) >= 0)
                                lblRole.Text = DAL.Lookups.EmployeeRoles[g.UserRoleID];
                        }
            */

            if (g.UserID == 0)
            {
                ImageButton imgConnect = (ImageButton)e.Row.FindControl("btnConnect");
                imgConnect.CommandArgument = e.Row.RowIndex.ToString();
                imgConnect.Visible = true; //(empDropDown.SelectedIndex >= 0);
            }
            else
            {
                ImageButton imgDisconnect = (ImageButton)e.Row.FindControl("btnDisconnect");
                imgDisconnect.CommandArgument = e.Row.RowIndex.ToString();
                imgDisconnect.Visible = true;
            }
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
            if (e.CommandName == "Sort") return;

            int iRowIndex = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = this.GtuEmployeeGrid.Rows[iRowIndex];
            int iGtuID = Convert.ToInt32(this.GtuEmployeeGrid.DataKeys[row.RowIndex].Value);

            if (e.CommandName == "disconnect")
            {
                // get the current row
                HtmlInputHidden rowEmployeeId = (HtmlInputHidden)row.FindControl("rowEmployeeID");
                int iUserID = Convert.ToInt32(rowEmployeeId.Value);

                DAL.GtuDB.DisconectGtuToEmployee(iGtuID, iUserID);
                Reload();

                if (iGtuID.ToString() == txtGtuID.Value)
                    this.clearMappingForm();
            }

            if (e.CommandName == "connect")
            {
                this.txtGtuID.Value = iGtuID.ToString();
                lblGtuUniqueID.Text = row.Cells[0].Text;
                pnlConnect.Visible = true;
            }

        }

        private void clearMappingForm()
        {
            txtGtuID.Value = "0";
            lblGtuUniqueID.Text = "";
            dropDistributor.SelectedIndex = 0;
            dropEmployee.Items.Clear();
            txtColor.Text = "";
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

                DAL.TimmDomainService timm = new DAL.TimmDomainService();
                timm.AssignGtuToEmployee(gtuID, userID, this.taskID);
                //DAL.GtuDB.SetGtuUserMapping(gtuID, userID, this.taskID);
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
                    assignedUsers.Add(Convert.ToInt32(empDropDown.SelectedValue));
            }
            return assignedUsers;
        }

        protected string ShowShortGtuNumber(string sUniqueID)
        {
            return sUniqueID.Substring(Math.Max(0, sUniqueID.Length - 6));
        }

        //is btnAssignAll button enabled.
        private void IsAssignAllEnable(List<DAL.ViewGtuInTask> gtuList)
        {
            btnAssignAll.Enabled = true;
            foreach (DAL.ViewGtuInTask g in gtuList)
            {
                if (g.UserID != 0 && g.UserID != null)
                {
                    btnAssignAll.Enabled = false;
                    break;
                }
            }
        }

        //bug 444 assign the whole bag of GTU w/o walkers or drivers to a map
        protected void btnAssignAll_Click(object sender, EventArgs e)
        {
            try
            {
                DAL.TimmDomainService timm = new DAL.TimmDomainService();

                ////get fake company [Don't use Fake Company]
                //string fakecompany = System.Configuration.ConfigurationManager.AppSettings["FakeCompanyName"];
                //
                //DAL.company distrabutor = timm.GetCompanies().Where(o => o.Name == fakecompany).FirstOrDefault();
                ////get fake company's employee
                //int iDistributor = distrabutor.Id;
                ////List<DAL.user> userList = timm.GetUsersByCompanyID(iDistributor);
                //List<DAL.user> userList = timm.GetAvailableUsersByCompanyID(iDistributor);

                //colorPick panel's color enum
                List<string> colorPanel = new List<string>(){
                "7F7F7F", "880015", "ED1C24", "FF7F27","FFF200","22B14C","00A2E8","3F48CC","A349A4",
                "C3C3C3", "B97A57", "FFAEC9","FFC90E","EFE4B0","B5E61D","99D9EA","7092BE","C8BFE7"
                };

                int i = 0;
                int j = 0;                

                foreach (DAL.ViewGtuInTask vgt in mGtuList)
                {
                    //int iGtuID = Convert.ToInt32(txtGtuID.Value);
                    //int iUserID = Convert.ToInt32(dropEmployee.SelectedValue);
                    int iGtuID = vgt.GtuID;

                    //int iUserID = userList[i].Id;
                    var gtu = timm.GetGtuByID(iGtuID);

                    if (!gtu.UserId.HasValue) continue;

                    int iUserID = gtu.UserId.Value;                    

                    string strColor;

                    if (j < colorPanel.Count)//
                    {
                        strColor = colorPanel[j];
                    }
                    else
                    {
                        // Random Color
                        var random = new Random();

                        strColor =
                            Convert.ToString(random.Next(0, 255), 16) +
                            Convert.ToString(random.Next(0, 255), 16) +
                            Convert.ToString(random.Next(0, 255), 16);
                    }

                    DAL.GtuDB.AssignGtuToEmployee(iGtuID, iUserID, this.taskID, "#" + strColor);

                    i++;
                    j++;
                    if (j > mGtuList.Count)
                        j = 0;
                }
                Reload();
                clearMappingForm();
            }

            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }

            //Reload();
        }
    }
}