using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.Web;

namespace GPS.Website
{
    public partial class AdminGtuToBag : SecurityPage
    {
        private string GTUPACK = "GTU Bag";

        private List<DAL.gtu> LeftGtuList
        {
            set
            {
                ViewState["LeftGtuList"] = value;
            }
            get
            {
                if (ViewState["LeftGtuList"] == null) return null;
                return (List<DAL.gtu>)ViewState["LeftGtuList"];
            }
        }

        private List<DAL.gtu> RightGtuList
        {
            set
            {
                ViewState["RightGtuList"] = value;
            }
            get
            {
                if (ViewState["RightGtuList"] == null) return null;
                return (List<DAL.gtu>)ViewState["RightGtuList"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                // populate GtuBags
                initDropDownGtuBag();

                populateGtuList();
            }
        }

        private void initDropDownGtuBag()
        {
            GPS.Website.DAL.TimmDomainService timm = new DAL.TimmDomainService();
            this.dropDownBagList.Items.Add(GTUPACK);
            foreach (DAL.gtubag bag in timm.GetGtuBags())
            {
                this.dropDownBagList.Items.Add(bag.Id.ToString());
            }
        }

        protected void dropDownBagList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // populate GTU List
            populateGtuList();
        }

        private void populateGtuList()
        {
            // get unallocated gtus
            DAL.TimmDomainService timmDB = new DAL.TimmDomainService();
            List<DAL.gtu> unallocatedGtuList = timmDB.GetGtusNotInBag();
            this.LeftGtuList = unallocatedGtuList;

            this.gridLeft.DataSource = unallocatedGtuList;
            this.gridLeft.DataBind();

            // populate allocated Gtu list
            List<DAL.gtu> gtuListRight = new List<DAL.gtu>();
            if (dropDownBagList.SelectedIndex > 0)
            {
                int iBagID = Convert.ToInt32(dropDownBagList.SelectedValue);
                gtuListRight = timmDB.GetGtusByBagID(iBagID);
            }

            this.RightGtuList = gtuListRight;
            gridRight.DataSource = gtuListRight;
            gridRight.DataBind();

            // enable or disable arrow buttons
            btnLeftArrow.Enabled = (dropDownBagList.SelectedIndex > 0);
            btnRightArrow.Enabled = (dropDownBagList.SelectedIndex > 0);
        }

        protected void gridLeft_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string txtID = this.txtGtuIndexLeft.ClientID;
            string btnID = this.btnChangeGtuIndexLeft.ClientID;

            string sOnClick = string.Format("changeRowIndex({0}, '{1}', '{2}')", e.Row.RowIndex, txtID, btnID);
            e.Row.Attributes.Add("onclick", sOnClick);
        }

        protected void gridRight_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string txtID = this.txtGtuIndexRight.ClientID;
            string btnID = this.btnChangeGtuIndexRight.ClientID;

            string sOnClick = string.Format("changeRowIndex({0}, '{1}', '{2}')", e.Row.RowIndex, txtID, btnID);
            e.Row.Attributes.Add("onclick", sOnClick);
        }

        // select gtu in left-unallocated gtu list
        protected void btnChangeGtuIndexLeft_Click(object sender, EventArgs e)
        {
            this.gridLeft.SelectedIndex = Convert.ToInt32(txtGtuIndexLeft.Value);
            this.refreshLeftGrid();
        }

        // select gtu in right-allocated gtu
        protected void btnChangeGtuIndexRigth_Click(object sender, EventArgs e)
        {
            this.gridRight.SelectedIndex = Convert.ToInt32(this.txtGtuIndexRight.Value);
            refreshRightGrid();
        }

        private void refreshLeftGrid()
        {
            if (this.LeftGtuList == null)
            {
                // get un-assigned gtuBag
                DAL.TimmDomainService timm = new DAL.TimmDomainService();
                this.LeftGtuList = timm.GetGtusNotInBag();
            }

            this.gridLeft.DataSource = this.LeftGtuList;
            this.gridLeft.DataBind();
        }

        private void refreshRightGrid()
        {
            if (this.RightGtuList == null)
            {
                // refresh RightGrid
                if (this.dropDownBagList.SelectedIndex == 0)
                {
                    this.RightGtuList = null;
                }
                else
                {
                    DAL.TimmDomainService timm = new DAL.TimmDomainService();
                    int iBagID = Convert.ToInt32(this.dropDownBagList.SelectedValue);
                    this.RightGtuList = timm.GetGtusByBagID(iBagID);
                }
            }

            this.gridRight.DataSource = this.RightGtuList;
            this.gridRight.DataBind();
        }


        // remove Gtu from GtuBag
        protected void btnLeftArrow_Click(object sender, ImageClickEventArgs e)
        {
            if (dropDownBagList.SelectedIndex == 0)
                return;

            // get selected unassigned GTU
            int iIndex = this.gridRight.SelectedIndex;
            DAL.gtu g = this.RightGtuList[iIndex];

            if (this.LeftGtuList == null)
                this.LeftGtuList = new List<DAL.gtu>();
            this.LeftGtuList.Add(g);
            this.refreshLeftGrid();

            this.RightGtuList.RemoveAt(iIndex);
            this.gridRight.SelectedIndex = -1;
            refreshRightGrid();
        }

        // Add Gtu to GtuBag
        protected void btnRightArrow_Click(object sender, ImageClickEventArgs e)
        {
            if (dropDownBagList.SelectedIndex == 0)
                return;

            // get the selected Gtu
            int iIndex = this.gridLeft.SelectedIndex;
            DAL.gtu g = this.LeftGtuList[iIndex];

            // Add it to RightGrid
            if (this.RightGtuList == null)
                this.RightGtuList = new List<DAL.gtu>();
            this.RightGtuList.Add(g);
            refreshRightGrid();

            // Remove it from LeftGrid
            this.LeftGtuList.RemoveAt(iIndex);
            this.gridLeft.SelectedIndex = -1;
            refreshLeftGrid();
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            populateGtuList();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DAL.TimmDomainService timm = new DAL.TimmDomainService();

                // remove Gtu from Bag
                for (int i = 0; i < this.gridLeft.Rows.Count; i++)
                {
                    int iGtuID = Convert.ToInt32(gridLeft.DataKeys[i].Value);
                    DAL.gtu g = timm.GetGtuByID(iGtuID);
                    timm.RemoveGtuFromBag(g);
                }

                // Add Gtu to bag
                int iBagId = Convert.ToInt32(dropDownBagList.SelectedValue);
                for (int i = 0; i < this.gridRight.Rows.Count; i++)
                {
                    int iGtuID = Convert.ToInt32(gridRight.DataKeys[i].Value);
                    DAL.gtu g = timm.GetGtuByID(iGtuID);

                    g.GTUBagId = iBagId;
                    timm.AddGtuToBag(g);
                }

                // re-populate
                populateGtuList();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

    }
}