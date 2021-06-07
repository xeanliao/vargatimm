using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.Web;

namespace GPS.Website
{
    public partial class AdminDistributorCompany : SecurityPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                //DAL.timmEntities timm = new DAL.timmEntities();
                //this.DistributorGrid.DataSource = timm.companies;
                //this.DistributorGrid.DataBind();

                string[,] stateList = DAL.Lookups.USStates;
                int iStates = stateList.GetLength(0);
                for (int i = 0; i < iStates; i++)
                {
                    this.companyStateDropDown.Items.Add(new ListItem(stateList[i, 1], stateList[i, 0]));
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string sName = txtSearchCompany.Text.Trim();
            DAL.company[] distributorList = DAL.DistributorDB.GetDistributorByName(sName);

            this.listSearchResult.DataSource = distributorList;
            this.listSearchResult.DataBind();
        }

        protected void listSearchResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sID = listSearchResult.SelectedValue;
            int id = Convert.ToInt32(sID);
            DAL.company distributor = DAL.DistributorDB.GetDistributorByID(id);
            populateDistributor(distributor);
        }

        private void populateDistributor(DAL.company distributor)
        {
            if (distributor == null)
                distributor = new DAL.company();

            txtDistributorID.Value = distributor.Id.ToString();
            txtDistributionName.Text = distributor.Name;
            txtAddress1.Text = distributor.Address1;
            txtAddress2.Text = distributor.Address2;
            txtCity.Text = distributor.City;
            companyStateDropDown.SelectedValue = distributor.State;
            txtZip.Text = distributor.ZipCode;
        }

        // insert or update distributorCompany
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int id = 0;
                if (txtDistributorID.Value != "" & txtDistributorID.Value != "0")
                    id = Convert.ToInt32(txtDistributorID.Value);

                DAL.timmEntities context = new DAL.timmEntities();
                DAL.company distributor = null;

                if (id == 0)
                {
                    if (context.companies.Where(it => it.Name == txtDistributionName.Text).Count() > 0)
                        throw new Exception("Company name cannot be duplicated!");
                    distributor = new DAL.company();
                }
                else
                    distributor = context.companies.Where(c => c.Id == id).FirstOrDefault();

                distributor.Name = txtDistributionName.Text;
                distributor.Address1 = txtAddress1.Text;
                distributor.Address2 = txtAddress2.Text;
                distributor.City = txtCity.Text;
                distributor.State = companyStateDropDown.SelectedValue;
                distributor.ZipCode = txtZip.Text;

                if (id == 0)
                    context.AddTocompanies(distributor);
                context.SaveChanges();

                lblMessage.Text = "<script>alert('saved')</script>";
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            populateDistributor(null);
        }
    }
}