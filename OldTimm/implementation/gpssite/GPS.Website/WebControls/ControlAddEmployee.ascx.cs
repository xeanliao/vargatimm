using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website.WebControls
{
    public partial class ControlAddEmployee : System.Web.UI.UserControl
    {
        private int mUserID
        {
            set { ViewState["userid"] = value; }
            get 
            { 
                return Convert.ToInt32(ViewState["userid"]); 
            }
        }

        public bool AllowNameChange
        {
            set
            {
                ViewState["enablenamechange"] = value;
                distributorDropDown.Enabled = value;
                txtFullName.Enabled = value;
                employeeRoleDropDown.Enabled = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DAL.timmEntities context = new DAL.timmEntities();
                List<DAL.company> companyList = context.companies.ToList();
                foreach(DAL.company c in companyList)
                    this.distributorDropDown.Items.Add(new ListItem(c.Name, c.Id.ToString()) );

                // populate employeeRoleDropDown
                SortedList<int, string> roles = DAL.Lookups.EmployeeRoles;
                foreach(int iRoleID in roles.Keys)
                    this.employeeRoleDropDown.Items.Add(new ListItem(roles[iRoleID], iRoleID.ToString()));
            }
        }

        public int UserID
        {
            set
            {
                mUserID = value;
                DAL.timmEntities context = new DAL.timmEntities();
                DAL.user u = context.users.Where(it => it.Id == value).FirstOrDefault();
                if (u == null) return;

                // populate information of current user
                distributorDropDown.SelectedValue = u.CompanyId.ToString();
                this.employeeRoleDropDown.SelectedValue = u.Role.ToString();

                txtFullName.Text = u.FullName;
                txtCellPhone.Text = u.CellPhone;
                if (u.DateOfBirth != null)
                {
                    txtBirthYear.Text = u.DateOfBirth.Value.Year.ToString();
                    this.birthMonthDropDown.SelectedValue = u.DateOfBirth.Value.Month.ToString();
                    this.birthDayDropDown.SelectedValue = u.DateOfBirth.Value.Day.ToString();
                }
                this.txtNotes.Text = u.Notes;
            }
        }

        public void ClearForm()
        {
            distributorDropDown.SelectedIndex = 0;
            this.employeeRoleDropDown.SelectedIndex = 0;

            txtFullName.Text = "";
            txtCellPhone.Text = "";
            txtBirthYear.Text = "";
            this.txtNotes.Text = "";
        }

        private void ValidInput()
        {
            if(distributorDropDown.SelectedIndex == 0)
                throw new Exception("Please select a distributor");

            if (employeeRoleDropDown.SelectedIndex == 0)
                throw new Exception("Please select a role");

            if (txtFullName.Text.Trim() == "")
                throw new Exception("Please input first name and last name");

            if (birthMonthDropDown.SelectedIndex > 0 
                && birthDayDropDown.SelectedIndex > 0 
                && txtBirthYear.Text.Trim() != "")
            {
                string sBirthday = string.Format("{0}/{1}/{2}", this.birthMonthDropDown.SelectedValue, this.birthDayDropDown.SelectedValue, txtBirthYear.Text);
                DateTime birthday = new DateTime();
                if (DateTime.TryParse(sBirthday, out birthday) == false)
                {
                    throw new Exception("Invalid birthday");
                }

                if (birthday.Year < 1900 | birthday.Year > DateTime.Today.Year)
                    throw new Exception("Birthday is out of range");
            }
        }

        public bool SaveEmployee()
        {
            try
            {
                this.ValidInput();

                DAL.timmEntities context = new DAL.timmEntities();

                int iUserID = mUserID;
                DAL.user u = new DAL.user();
                if (mUserID != 0)
                    u = context.users.Where(it => it.Id == iUserID).FirstOrDefault();

                u.CompanyId = Convert.ToInt32(distributorDropDown.SelectedValue);
                u.Role = Convert.ToInt32(this.employeeRoleDropDown.SelectedValue);
                u.FullName = txtFullName.Text.Trim();
                u.CellPhone = txtCellPhone.Text;
                if (birthMonthDropDown.SelectedIndex > 0
                    && birthDayDropDown.SelectedIndex > 0
                    && txtBirthYear.Text.Trim() != "")
                {
                    u.DateOfBirth = Convert.ToDateTime(string.Format("{0}/{1}/{2}", this.birthMonthDropDown.SelectedValue, this.birthDayDropDown.SelectedValue, txtBirthYear.Text));
                }
                else
                    u.DateOfBirth = null;
                u.Notes = txtNotes.Text;

                // save the file
                if (this.photoFileUpload.HasFile)
                {
                    string sFile = this.photoFileUpload.FileName;
                    int ipos = sFile.IndexOf(".");
                    if (ipos < 0)
                        throw new Exception("Invalid photo file");
                    string sFileExt = sFile.Substring(ipos);

                    string sGuid = Guid.NewGuid().ToString();
                    string pictureRoot = DAL.ConfigUtils.GetConfiguration("PictureRoot");
                    string sPath = pictureRoot + "/" + sGuid + sFileExt;

                    this.photoFileUpload.SaveAs(sPath);
                    u.Picture = sGuid + sFileExt;
                }

                if (iUserID == 0)
                {
                    u.UserCode = txtFullName.Text.Replace(" ", ".");
                    u.UserName = Guid.NewGuid().ToString();
                    u.Password = Guid.NewGuid().ToString();
                    u.Enabled = false;  // employee cannot access the software.
                }

                // save the Employee
                if (iUserID == 0)
                    context.AddTousers(u);

                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                if (ex.InnerException != null)
                    lblMessage.Text = ex.InnerException.Message;
                return false;
            }
        }

    }
}