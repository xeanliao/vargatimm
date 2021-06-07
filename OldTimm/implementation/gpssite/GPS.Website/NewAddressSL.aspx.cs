using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website
{
    public partial class NewAddressSL : System.Web.UI.Page
    {
        protected string mAddressId = "";
        protected string mStreet = "";
        protected string mZipcode = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnDelete.Attributes.Add("onclick", "return confirm('Do you really want to remove it?')");
            }

            if (Request["id"] != null)
            {
                int addressId = Convert.ToInt32(Request["id"]);
                populateAddress(addressId);

                btnDelete.CommandArgument = addressId.ToString();
            }

            btnDelete.Visible = (mAddressId != "");
        }

        private void populateAddress(int addressId)
        {
            mAddressId = "";
            mStreet = "";
            mZipcode = "";

            GPS.DataLayer.MonitorAddressRepository addrRep = new GPS.DataLayer.MonitorAddressRepository();
            GPS.DomainLayer.Entities.MonitorAddresses address = addrRep.GetEntity(addressId);

            if (address != null)
            {
                mAddressId = address.Id.ToString();
                mStreet = address.Address1;
                mZipcode = address.ZipCode;

                this.lblMessage.Text = string.Format("<script>showNewAddress('{0}')</script>", address.Picture);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                int addressId = Convert.ToInt32(btn.CommandArgument);

                GPS.DataLayer.MonitorAddressRepository addressRep = new GPS.DataLayer.MonitorAddressRepository();
                addressRep.DeleteEntity(addressId);

                // close the page, refresh opener
                lblMessage.Text = @"<script>window.opener.document.location.reload(); window.close();</script>";
            }
            catch (Exception ex)
            {
                lblMessage.Text = string.Format("<div style='color:red'>{0}</div>", ex.Message);
                GPS.Utilities.LogUtils.Error("Web application Unhandle error", ex);
            }
        }
    }
}
