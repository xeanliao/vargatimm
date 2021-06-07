using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.Web;

namespace GPS.Website
{
    public partial class AdminGtuBagToAuditor : SecurityPage
    {
        private List<DAL.gtubag> LeftGtuBags
        {
            get
            {
                if (ViewState["leftGtuBags"] == null) return null;
                return (List<DAL.gtubag>)ViewState["leftGtuBags"];
            }
            set
            {
                ViewState["leftGtuBags"] = value;
            }
        }

        private List<DAL.gtubag> RightGtuBags
        {
            get
            {
                if (ViewState["rightGtuBags"] == null) return null;
                return (List<DAL.gtubag>)ViewState["rightGtuBags"];
            }
            set
            {
                ViewState["rightGtuBags"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                // get all auditors
                initDropDownAuditor();

                refreshGridLeft();
                refreshRightGrid();
            }
        }

        private void initDropDownAuditor()
        {
            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            List<DAL.user> userList = timm.GetAuditors();

            dropDownAuditor.Items.Add("-- Auditor -- ");
            foreach (DAL.user u in userList)
                dropDownAuditor.Items.Add(new ListItem(u.FullName, u.Id.ToString()));
        }

        protected void dropDownAuditor_SelectedIndexChanged(object sender, EventArgs e)
        {
        
            this.gridLeft.SelectedIndex = -1;
            this.LeftGtuBags = null;
            refreshGridLeft();

            this.gridRight.SelectedIndex = -1;
            this.RightGtuBags = null;
            refreshRightGrid();
        }

        private void refreshGridLeft()
        {
            if (this.LeftGtuBags == null)
            {
                // get un-assigned gtuBag
                DAL.TimmDomainService timm = new DAL.TimmDomainService();
                this.LeftGtuBags = timm.GetUnassignedGtuBags();
            }

            this.gridLeft.DataSource = this.LeftGtuBags;
            this.gridLeft.DataBind();
        }

        private void refreshRightGrid()
        {
            if (this.RightGtuBags == null)
            {
                // refresh RightGrid
                if (this.dropDownAuditor.SelectedIndex == 0)
                {
                    this.RightGtuBags = null;
                }
                else
                {
                    DAL.TimmDomainService timm = new DAL.TimmDomainService();
                    int iUserID = Convert.ToInt32(this.dropDownAuditor.SelectedValue);
                    this.RightGtuBags = timm.GetGtuBagsByAuditor(iUserID);
                }
            }

            this.gridRight.DataSource = this.RightGtuBags;
            this.gridRight.DataBind();
        }

        protected void gridLeft_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex < 0) return;

            DAL.gtubag bag = (DAL.gtubag)e.Row.DataItem;
            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            Label lblGtuCount = (Label)e.Row.FindControl("lblGtuCount");
            int iGtuCount = timm.GetGtuCountInBag(bag.Id);
            lblGtuCount.Text = iGtuCount.ToString() + " GTUs";

            if (e.Row.RowIndex != gridLeft.SelectedIndex)
            {
                e.Row.Attributes.Add("onclick", string.Format("changeRow({0}, '{1}', '{2}');",
                    e.Row.RowIndex, txtLeftGridRow.ClientID, btnChangeLeftGridRow.ClientID));
            }
            else
            {
                if(iGtuCount == 0)
                    btnArrowRight.Attributes.Add("onclick", "return confirm('This bag has no GTU, do you really want to assign it?')");
                else
                    btnArrowRight.Attributes.Add("onclick", "");
            }

        }

        protected void gridRight_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowIndex < 0)return;

            DAL.gtubag bag = (DAL.gtubag)e.Row.DataItem;

            if (e.Row.RowIndex != gridRight.SelectedIndex)
                e.Row.Attributes.Add("onclick", string.Format("changeRow({0}, '{1}', '{2}');",
                    e.Row.RowIndex, txtRightGridRow.ClientID, btnChangeRightGridRow.ClientID));

            DAL.TimmDomainService timm = new DAL.TimmDomainService();
            Label lblGtuCount = (Label)e.Row.FindControl("lblGtuCount");
            lblGtuCount.Text = timm.GetGtuCountInBag(bag.Id) + " GTUs";
        }

        protected void btnChangeLeftGridRow_Click(object sender, EventArgs e)
        {
            this.gridLeft.SelectedIndex = Convert.ToInt32(txtLeftGridRow.Value);
            refreshGridLeft();
        }

        protected void btnChangeRightGridRow_Click(object sender, EventArgs e)
        {
            this.gridRight.SelectedIndex = Convert.ToInt32(txtRightGridRow.Value);
            refreshRightGrid();
        }

        protected void btnArrowRight_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (dropDownAuditor.SelectedIndex == 0)
                {
                    lblInfo.Text = "please select an auditor first";
                    return;
                }

                int iIndex = this.gridLeft.SelectedIndex;
                if (iIndex < 0) return;
                DAL.gtubag bag = this.LeftGtuBags[iIndex];

                if (this.RightGtuBags == null)
                    this.RightGtuBags = new List<DAL.gtubag>();
                this.RightGtuBags.Add(bag);
                refreshRightGrid();

                this.LeftGtuBags.RemoveAt(iIndex);
                this.gridLeft.SelectedIndex = -1;
                refreshGridLeft();
            }
            catch (Exception ex)
            {
                this.lblInfo.Text = ex.Message;
            }
        }

        protected void btnArrowLeft_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (dropDownAuditor.SelectedIndex == 0)
                {
                    lblInfo.Text = "please select an auditor first";
                    return;
                }

                int iIndex = this.gridRight.SelectedIndex;
                if (iIndex < 0) return;
                DAL.gtubag bag = this.RightGtuBags[iIndex];

                // add the selected one to left-side
                if (this.LeftGtuBags == null)
                    this.LeftGtuBags = new List<DAL.gtubag>();
                this.LeftGtuBags.Add(bag);
                this.refreshGridLeft();

                // remove the selected one from right-side
                this.RightGtuBags.RemoveAt(iIndex);
                this.gridRight.SelectedIndex = -1;
                refreshRightGrid();
            }
            catch (Exception ex)
            {
                this.lblInfo.Text = ex.Message;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DAL.TimmDomainService timm = new DAL.TimmDomainService();

                // remove gtubag from Auditor
                List<DAL.gtubag> bagsLeft = this.LeftGtuBags;
                foreach (DAL.gtubag bag in bagsLeft)
                {
                    timm.RemoveGtuBagFromAuditor(bag);
                }

                // add gtubag to auditor
                int iUserID = Convert.ToInt32(dropDownAuditor.SelectedValue);
                List<DAL.gtubag> bagsRight = this.RightGtuBags;
                foreach (DAL.gtubag bag in bagsRight)
                {
                    bag.UserId = iUserID;
                    timm.AssignGtuBagToAuditor(bag);
                }
                lblInfo.Text = string.Format("<script type='text/javascript'>alert('{0}');</script>", "Saved");
            }
            catch (Exception ex)
            {
                lblInfo.Text = string.Format("<script type='text/javascript'>alert('{0}');</script>", ex.Message);
            }
        }
    }
}