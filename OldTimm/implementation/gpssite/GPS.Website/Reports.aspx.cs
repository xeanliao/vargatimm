using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.Web;
using GPS.DomainLayer.Entities;
using GPS.DataLayer;
using GPS.Website.AppFacilities;
using GPS.Website.TransferObjects;
using GPS.Website.WebControls;
using System.IO;
using ExpertPdf.HtmlToPdf;

namespace GPS.Website {
    public partial class Reports : SecurityPage
    {
        public int cId = 1697481821;
        public string UrlBase = System.Configuration.ConfigurationManager.AppSettings["UrlBase"];
        //public string campaignName = "";

        private string mReportUrl = "";
        private string mReportRoot = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            mReportUrl = System.Configuration.ConfigurationManager.AppSettings["reportUrl"];
            mReportRoot = System.Configuration.ConfigurationManager.AppSettings["reportRoot"]; ;

            cId = Convert.ToInt32(this.Request.QueryString["cid"]);
            if (!this.Page.IsPostBack)
            {
                GPS.DomainLayer.Entities.Campaign cam = null;
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    cam = ws.Repositories.CampaignRepository.GetEntity(cId);
                }
                if (cam == null) return;
                User u = null;
                using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                {
                    u = ws.Repositories.UserRepository.GetUser(cam.CreatorName); 
                }
                string userCode = null != u ? u.UserCode : string.Empty;
                camName.Text = GPS.DomainLayer.Entities.Campaign.ConstructCompositeName(cam.Date, cam.ClientCode, cam.AreaDescription, userCode, cam.Sequence);
                
                BindWalker();
                BindDriver();
                BindAuditor();
            }

        }


        private void  BindWalker()
        {            
            IList<User> walkerList = null;
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                walkerList = ws.Repositories.UserRepository.GetWalkersByCampaignid(cId);               
            }

            if (walkerList != null)
            {
                this.dgWalker.DataSource = walkerList;
                this.dgWalker.DataBind();

                //calculate summary data
                decimal avgSpeedNWTotal = 0;
                decimal highSpeedNWTotal = 0;
                decimal lowSpeedNWTotal = 0;
                decimal avgSpeedYWTotal = 0;
                decimal highSpeedYWTotal = 0;
                decimal lowSpeedYWTotal = 0;
                decimal avgSpeedAWTotal = 0;
                decimal highSpeedAWTotal = 0;
                decimal lowSpeedAWTotal = 0;

                decimal avgGroundNWTotal = 0;
                decimal highGroundNWTotal = 0;
                decimal lowGroundNWTotal = 0;
                decimal avgGroundYWTotal = 0;
                decimal highGroundYWTotal = 0;
                decimal lowGroundYWTotal = 0;
                decimal avgGroundAWTotal = 0;
                decimal highGroundAWTotal = 0;
                decimal lowGroundAWTotal = 0;

                double avgStopNWTotal = 0;
                double highStopNWTotal = 0;
                double lowStopNWTotal = 0;
                double avgStopYWTotal = 0;
                double highStopYWTotal = 0;
                double lowStopYWTotal = 0;
                double avgStopAWTotal = 0;
                double highStopAWTotal = 0;
                double lowStopAWTotal = 0;

                int i = 0;
                foreach (RepeaterItem item in this.dgAuditor.Items)
                {
                    ReportWalker task = item.FindControl("ReportWalker") as ReportWalker;
                    if (task.speedList.Count != 0)
                    {
                        avgSpeedNWTotal = avgSpeedNWTotal + task.speedList[0];
                        if (highSpeedNWTotal < task.speedList[1])
                        {
                            highSpeedNWTotal = task.speedList[1];
                        }
                        if (lowSpeedNWTotal > task.speedList[2])
                        {
                            lowSpeedNWTotal = task.speedList[2];
                        }
                    }
                    if (task.speedListYear.Count != 0)
                    {
                        avgSpeedYWTotal = avgSpeedYWTotal + task.speedListYear[0];
                        if (highSpeedYWTotal < task.speedListYear[1])
                        {
                            highSpeedYWTotal = task.speedListYear[1];
                        }
                        if (lowSpeedYWTotal > task.speedListYear[2])
                        {
                            lowSpeedYWTotal = task.speedListYear[2];
                        }
                    }
                    if (task.speedListAll.Count != 0)
                    {
                        avgSpeedAWTotal = avgSpeedAWTotal + task.speedListAll[0];
                        if (highSpeedAWTotal < task.speedListAll[1])
                        {
                            highSpeedAWTotal = task.speedListAll[1];
                        }
                        if (lowSpeedAWTotal > task.speedListAll[2])
                        {
                            lowSpeedAWTotal = task.speedListAll[2];
                        }
                    }
                    if (task.groundList.Count != 0)
                    {
                        avgGroundNWTotal = avgGroundNWTotal + task.groundList[0];
                        if (highGroundNWTotal < task.groundList[1])
                        {
                            highGroundNWTotal = task.groundList[1];
                        }
                        if (lowGroundNWTotal > task.groundList[2])
                        {
                            lowGroundNWTotal = task.groundList[2];
                        }
                    }
                    if (task.groundListYear.Count != 0)
                    {
                        avgGroundYWTotal = avgGroundYWTotal + task.groundListYear[0];
                        if (highGroundYWTotal < task.groundListYear[1])
                        {
                            highGroundYWTotal = task.groundListYear[1];
                        }
                        if (lowGroundYWTotal > task.groundListYear[2])
                        {
                            lowGroundYWTotal = task.groundListYear[2];
                        }
                    }
                    if (task.groundListAll.Count != 0)
                    {
                        avgGroundAWTotal = avgGroundAWTotal + task.groundListAll[0];
                        if (highGroundAWTotal < task.groundListAll[1])
                        {
                            highGroundAWTotal = task.groundListAll[1];
                        }
                        if (lowGroundAWTotal > task.groundListAll[2])
                        {
                            lowGroundAWTotal = task.groundListAll[2];
                        }
                    }
                    if (task.stopList.Count != 0)
                    {
                        avgStopNWTotal = avgStopNWTotal + task.stopList[0];
                        if (highStopNWTotal < task.stopList[1])
                        {
                            highStopNWTotal = task.stopList[1];
                        }
                        if (lowStopNWTotal > task.stopList[2])
                        {
                            lowStopNWTotal = task.stopList[2];
                        }
                    }
                    if (task.stopListYear.Count != 0)
                    {
                        avgStopYWTotal = avgStopYWTotal + task.stopListYear[0];
                        if (highStopYWTotal < task.stopListYear[1])
                        {
                            highStopYWTotal = task.stopListYear[1];
                        }
                        if (lowStopYWTotal > task.stopListYear[2])
                        {
                            lowStopYWTotal = task.stopListYear[2];
                        }
                    }
                    if (task.stopListAll.Count != 0)
                    {
                        avgStopAWTotal = avgStopAWTotal + task.stopListAll[0];
                        if (highStopAWTotal < task.stopListAll[1])
                        {
                            highStopAWTotal = task.stopListAll[1];
                        }
                        if (lowStopAWTotal > task.stopListAll[2])
                        {
                            lowStopAWTotal = task.stopListAll[2];
                        }
                    }
                    i++;
                }
                avgSpeedNW.Text = i==0?"0":(avgSpeedNWTotal / i).ToString();
                highSpeedNW.Text = highSpeedNWTotal.ToString();
                lowSpeedNW.Text = lowSpeedNWTotal.ToString();
                avgSpeedYW.Text = i == 0 ? "0" : (avgSpeedYWTotal / i).ToString();
                highSpeedYW.Text = highSpeedYWTotal.ToString();
                lowSpeedYW.Text = lowSpeedYWTotal.ToString();
                avgSpeedAW.Text = i == 0 ? "0" : (avgSpeedAWTotal / i).ToString();
                highSpeedAW.Text = highSpeedAWTotal.ToString();
                lowSpeedAW.Text = lowSpeedAWTotal.ToString();

                avgGroundNW.Text = i == 0 ? "0" : (avgGroundNWTotal / i).ToString();
                highGroundNW.Text = highGroundNWTotal.ToString();
                lowGroundNW.Text = lowGroundNWTotal.ToString();
                avgGroundYW.Text = i == 0 ? "0" : (avgGroundYWTotal / i).ToString();
                highGroundYW.Text = highGroundYWTotal.ToString();
                lowGroundYW.Text = lowGroundYWTotal.ToString();
                avgGroundAW.Text = i == 0 ? "0" : (avgGroundAWTotal / i).ToString();
                highGroundAW.Text = highGroundAWTotal.ToString();
                lowGroundAW.Text = lowGroundAWTotal.ToString();

                avgStopNW.Text = i == 0 ? "0" : (avgStopNWTotal / i).ToString();
                highStopNW.Text = highStopNWTotal.ToString();
                lowStopNW.Text = lowStopNWTotal.ToString();
                avgStopYW.Text = i == 0 ? "0" : (avgStopYWTotal / i).ToString();
                highStopYW.Text = highStopYWTotal.ToString();
                lowStopYW.Text = lowStopYWTotal.ToString();
                avgStopAW.Text = i == 0 ? "0" : (avgStopAWTotal / i).ToString();
                highStopAW.Text = highStopAWTotal.ToString();
                lowStopAW.Text = lowStopAWTotal.ToString();

            }
        }
        private void BindDriver()
        {
            IList<User> driverList = null;
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                driverList = ws.Repositories.UserRepository.GetDriversByCampaignid(cId);
            }

            if (driverList != null)
            {
                this.dgDriver.DataSource = driverList;
                this.dgDriver.DataBind();

                //calculate summary data
                decimal avgSpeedNDTotal = 0;
                decimal highSpeedNDTotal = 0;
                decimal lowSpeedNDTotal = 0;
                decimal avgSpeedYDTotal = 0;
                decimal highSpeedYDTotal = 0;
                decimal lowSpeedYDTotal = 0;
                decimal avgSpeedADTotal = 0;
                decimal highSpeedADTotal = 0;
                decimal lowSpeedADTotal = 0;

                decimal avgGroundNDTotal = 0;
                decimal highGroundNDTotal = 0;
                decimal lowGroundNDTotal = 0;
                decimal avgGroundYDTotal = 0;
                decimal highGroundYDTotal = 0;
                decimal lowGroundYDTotal = 0;
                decimal avgGroundADTotal = 0;
                decimal highGroundADTotal = 0;
                decimal lowGroundADTotal = 0;

                double avgStopNDTotal = 0;
                double highStopNDTotal = 0;
                double lowStopNDTotal = 0;
                double avgStopYDTotal = 0;
                double highStopYDTotal = 0;
                double lowStopYDTotal = 0;
                double avgStopADTotal = 0;
                double highStopADTotal = 0;
                double lowStopADTotal = 0;

                int i = 0;
                foreach (RepeaterItem item in this.dgAuditor.Items)
                {
                    ReportDriver task = item.FindControl("ReportDriver") as ReportDriver;
                    if (task.speedList.Count != 0)
                    {
                        avgSpeedNDTotal = avgSpeedNDTotal + task.speedList[0];
                        if (highSpeedNDTotal < task.speedList[1])
                        {
                            highSpeedNDTotal = task.speedList[1];
                        }
                        if (lowSpeedNDTotal > task.speedList[2])
                        {
                            lowSpeedNDTotal = task.speedList[2];
                        }
                    }
                    if (task.speedListYear.Count != 0)
                    {
                        avgSpeedYDTotal = avgSpeedYDTotal + task.speedListYear[0];
                        if (highSpeedYDTotal < task.speedListYear[1])
                        {
                            highSpeedYDTotal = task.speedListYear[1];
                        }
                        if (lowSpeedYDTotal > task.speedListYear[2])
                        {
                            lowSpeedYDTotal = task.speedListYear[2];
                        }
                    }
                    if (task.speedListAll.Count != 0)
                    {
                        avgSpeedADTotal = avgSpeedADTotal + task.speedListAll[0];
                        if (highSpeedADTotal < task.speedListAll[1])
                        {
                            highSpeedADTotal = task.speedListAll[1];
                        }
                        if (lowSpeedADTotal > task.speedListAll[2])
                        {
                            lowSpeedADTotal = task.speedListAll[2];
                        }
                    }
                    if (task.groundList.Count != 0)
                    {
                        avgGroundNDTotal = avgGroundNDTotal + task.groundList[0];
                        if (highGroundNDTotal < task.groundList[1])
                        {
                            highGroundNDTotal = task.groundList[1];
                        }
                        if (lowGroundNDTotal > task.groundList[2])
                        {
                            lowGroundNDTotal = task.groundList[2];
                        }
                    }
                    if (task.groundListYear.Count != 0)
                    {
                        avgGroundYDTotal = avgGroundYDTotal + task.groundListYear[0];
                        if (highGroundYDTotal < task.groundListYear[1])
                        {
                            highGroundYDTotal = task.groundListYear[1];
                        }
                        if (lowGroundYDTotal > task.groundListYear[2])
                        {
                            lowGroundYDTotal = task.groundListYear[2];
                        }
                    }
                    if (task.groundListAll.Count != 0)
                    {
                        avgGroundADTotal = avgGroundADTotal + task.groundListAll[0];
                        if (highGroundADTotal < task.groundListAll[1])
                        {
                            highGroundADTotal = task.groundListAll[1];
                        }
                        if (lowGroundADTotal > task.groundListAll[2])
                        {
                            lowGroundADTotal = task.groundListAll[2];
                        }
                    }
                    if (task.stopList.Count != 0)
                    {
                        avgStopNDTotal = avgStopNDTotal + task.stopList[0];
                        if (highStopNDTotal < task.stopList[1])
                        {
                            highStopNDTotal = task.stopList[1];
                        }
                        if (lowStopNDTotal > task.stopList[2])
                        {
                            lowStopNDTotal = task.stopList[2];
                        }
                    }
                    if (task.stopListYear.Count != 0)
                    {
                        avgStopYDTotal = avgStopYDTotal + task.stopListYear[0];
                        if (highStopYDTotal < task.stopListYear[1])
                        {
                            highStopYDTotal = task.stopListYear[1];
                        }
                        if (lowStopYDTotal > task.stopListYear[2])
                        {
                            lowStopYDTotal = task.stopListYear[2];
                        }
                    }
                    if (task.stopListAll.Count != 0)
                    {
                        avgStopADTotal = avgStopADTotal + task.stopListAll[0];
                        if (highStopADTotal < task.stopListAll[1])
                        {
                            highStopADTotal = task.stopListAll[1];
                        }
                        if (lowStopADTotal > task.stopListAll[2])
                        {
                            lowStopADTotal = task.stopListAll[2];
                        }
                    }
                    i++;
                }
                avgSpeedND.Text = i == 0 ? "0" : (avgSpeedNDTotal / i).ToString();
                highSpeedND.Text = highSpeedNDTotal.ToString();
                lowSpeedND.Text = lowSpeedNDTotal.ToString();
                avgSpeedYD.Text = i == 0 ? "0" : (avgSpeedYDTotal / i).ToString();
                highSpeedYD.Text = highSpeedYDTotal.ToString();
                lowSpeedYD.Text = lowSpeedYDTotal.ToString();
                avgSpeedAD.Text = i == 0 ? "0" : (avgSpeedADTotal / i).ToString();
                highSpeedAD.Text = highSpeedADTotal.ToString();
                lowSpeedAD.Text = lowSpeedADTotal.ToString();

                avgGroundND.Text = i == 0 ? "0" : (avgGroundNDTotal / i).ToString();
                highGroundND.Text = highGroundNDTotal.ToString();
                lowGroundND.Text = lowGroundNDTotal.ToString();
                avgGroundYD.Text = i == 0 ? "0" : (avgGroundYDTotal / i).ToString();
                highGroundYD.Text = highGroundYDTotal.ToString();
                lowGroundYD.Text = lowGroundYDTotal.ToString();
                avgGroundAD.Text = i == 0 ? "0" : (avgGroundADTotal / i).ToString();
                highGroundAD.Text = highGroundADTotal.ToString();
                lowGroundAD.Text = lowGroundADTotal.ToString();

                avgStopND.Text = i == 0 ? "0" : (avgStopNDTotal / i).ToString();
                highStopND.Text = highStopNDTotal.ToString();
                lowStopND.Text = lowStopNDTotal.ToString();
                avgStopYD.Text = i == 0 ? "0" : (avgStopYDTotal / i).ToString();
                highStopYD.Text = highStopYDTotal.ToString();
                lowStopYD.Text = lowStopYDTotal.ToString();
                avgStopAD.Text = i == 0 ? "0" : (avgStopADTotal / i).ToString();
                highStopAD.Text = highStopADTotal.ToString();
                lowStopAD.Text = lowStopADTotal.ToString();
            }
        }
        private void BindAuditor()
        {
            IList<User> auditorList = null;
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                auditorList = ws.Repositories.UserRepository.GetAuditorsByCampaignid(cId);
            }

            if (auditorList != null)
            {
                this.dgAuditor.DataSource = auditorList;
                this.dgAuditor.DataBind();

                //calculate summary data
                decimal avgSpeedNATotal = 0;
                decimal highSpeedNATotal = 0;
                decimal lowSpeedNATotal = 0;
                decimal avgSpeedYATotal = 0;
                decimal highSpeedYATotal = 0;
                decimal lowSpeedYATotal = 0;
                decimal avgSpeedAATotal = 0;
                decimal highSpeedAATotal = 0;
                decimal lowSpeedAATotal = 0;

                decimal avgGroundNATotal = 0;
                decimal highGroundNATotal = 0;
                decimal lowGroundNATotal = 0;
                decimal avgGroundYATotal = 0;
                decimal highGroundYATotal = 0;
                decimal lowGroundYATotal = 0;
                decimal avgGroundAATotal = 0;
                decimal highGroundAATotal = 0;
                decimal lowGroundAATotal = 0;

                double avgStopNATotal = 0;
                double highStopNATotal = 0;
                double lowStopNATotal = 0;
                double avgStopYATotal = 0;
                double highStopYATotal = 0;
                double lowStopYATotal = 0;
                double avgStopAATotal = 0;
                double highStopAATotal = 0;
                double lowStopAATotal = 0;

                int i = 0;
                foreach (RepeaterItem item in this.dgAuditor.Items)
                {
                    ReportAuditor task = item.FindControl("ReportAuditor") as ReportAuditor;
                    if (task.speedList.Count != 0)
                    {
                        avgSpeedNATotal = avgSpeedNATotal + task.speedList[0];
                        if (highSpeedNATotal < task.speedList[1])
                        {
                            highSpeedNATotal = task.speedList[1];
                        }
                        if (lowSpeedNATotal > task.speedList[2])
                        {
                            lowSpeedNATotal = task.speedList[2];
                        }
                    }
                    if (task.speedListYear.Count != 0)
                    {
                        avgSpeedYATotal = avgSpeedYATotal + task.speedListYear[0];
                        if (highSpeedYATotal < task.speedListYear[1])
                        {
                            highSpeedYATotal = task.speedListYear[1];
                        }
                        if (lowSpeedYATotal > task.speedListYear[2])
                        {
                            lowSpeedYATotal = task.speedListYear[2];
                        }
                    }
                    if (task.speedListAll.Count != 0)
                    {
                        avgSpeedAATotal = avgSpeedAATotal + task.speedListAll[0];
                        if (highSpeedAATotal < task.speedListAll[1])
                        {
                            highSpeedAATotal = task.speedListAll[1];
                        }
                        if (lowSpeedAATotal > task.speedListAll[2])
                        {
                            lowSpeedAATotal = task.speedListAll[2];
                        }
                    }
                    if (task.groundList.Count != 0)
                    {
                        avgGroundNATotal = avgGroundNATotal + task.groundList[0];
                        if (highGroundNATotal < task.groundList[1])
                        {
                            highGroundNATotal = task.groundList[1];
                        }
                        if (lowGroundNATotal > task.groundList[2])
                        {
                            lowGroundNATotal = task.groundList[2];
                        }
                    }
                    if (task.groundListYear.Count != 0)
                    {
                        avgGroundYATotal = avgGroundYATotal + task.groundListYear[0];
                        if (highGroundYATotal < task.groundListYear[1])
                        {
                            highGroundYATotal = task.groundListYear[1];
                        }
                        if (lowGroundYATotal > task.groundListYear[2])
                        {
                            lowGroundYATotal = task.groundListYear[2];
                        }
                    }
                    if (task.groundListAll.Count != 0)
                    {
                        avgGroundAATotal = avgGroundAATotal + task.groundListAll[0];
                        if (highGroundAATotal < task.groundListAll[1])
                        {
                            highGroundAATotal = task.groundListAll[1];
                        }
                        if (lowGroundAATotal > task.groundListAll[2])
                        {
                            lowGroundAATotal = task.groundListAll[2];
                        }
                    }
                    if (task.stopList.Count != 0)
                    {
                        avgStopNATotal = avgStopNATotal + task.stopList[0];
                        if (highStopNATotal < task.stopList[1])
                        {
                            highStopNATotal = task.stopList[1];
                        }
                        if (lowStopNATotal > task.stopList[2])
                        {
                            lowStopNATotal = task.stopList[2];
                        }
                    }
                    if (task.stopListYear.Count != 0)
                    {
                        avgStopYATotal = avgStopYATotal + task.stopListYear[0];
                        if (highStopYATotal < task.stopListYear[1])
                        {
                            highStopYATotal = task.stopListYear[1];
                        }
                        if (lowStopYATotal > task.stopListYear[2])
                        {
                            lowStopYATotal = task.stopListYear[2];
                        }
                    }
                    if (task.stopListAll.Count != 0)
                    {
                        avgStopAATotal = avgStopAATotal + task.stopListAll[0];
                        if (highStopAATotal < task.stopListAll[1])
                        {
                            highStopAATotal = task.stopListAll[1];
                        }
                        if (lowStopAATotal > task.stopListAll[2])
                        {
                            lowStopAATotal = task.stopListAll[2];
                        }
                    }
                    i++;
                }
                avgSpeedNA.Text = i == 0 ? "0" : (avgSpeedNATotal / i).ToString();
                highSpeedNA.Text = highSpeedNATotal.ToString();
                lowSpeedNA.Text = lowSpeedNATotal.ToString();
                avgSpeedYA.Text = i == 0 ? "0" : (avgSpeedYATotal / i).ToString();
                highSpeedYA.Text = highSpeedYATotal.ToString();
                lowSpeedYA.Text = lowSpeedYATotal.ToString();
                avgSpeedAA.Text = i == 0 ? "0" : (avgSpeedAATotal / i).ToString();
                highSpeedAA.Text = highSpeedAATotal.ToString();
                lowSpeedAA.Text = lowSpeedAATotal.ToString();

                avgGroundNA.Text = i == 0 ? "0" : (avgGroundNATotal / i).ToString();
                highGroundNA.Text = highGroundNATotal.ToString();
                lowGroundNA.Text = lowGroundNATotal.ToString();
                avgGroundYA.Text = i == 0 ? "0" : (avgGroundYATotal / i).ToString();
                highGroundYA.Text = highGroundYATotal.ToString();
                lowGroundYA.Text = lowGroundYATotal.ToString();
                avgGroundAA.Text = i == 0 ? "0" : (avgGroundAATotal / i).ToString();
                highGroundAA.Text = highGroundAATotal.ToString();
                lowGroundAA.Text = lowGroundAATotal.ToString();

                avgStopNA.Text = i == 0 ? "0" : (avgStopNATotal / i).ToString();
                highStopNA.Text = highStopNATotal.ToString();
                lowStopNA.Text = lowStopNATotal.ToString();
                avgStopYA.Text = i == 0 ? "0" : (avgStopYATotal / i).ToString();
                highStopYA.Text = highStopYATotal.ToString();
                lowStopYA.Text = lowStopYATotal.ToString();
                avgStopAA.Text = i == 0 ? "0" : (avgStopAATotal / i).ToString();
                highStopAA.Text = highStopAATotal.ToString();
                lowStopAA.Text = lowStopAATotal.ToString();
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string name = camName.Text;
            string htmlPath = SaveHtml(name);
            string pdfPath = ConvertHtmlToPdf(htmlPath, name);
            OutputPdf(pdfPath);
        }



        private string SaveHtml(string campaignName)
        {
            divToolbar.Visible = false;
            string name = campaignName + ".html";
            name = Server.UrlEncode(name).Replace('+', '_').Replace('%', '_');
            string path = mReportRoot + "Print/" + name;
            StreamWriter sw = File.CreateText(path);
            HtmlTextWriter hwriter = new HtmlTextWriter(sw);
            base.Render(hwriter);
            hwriter.Close();
            sw.Close();
            return mReportUrl + "Print/" + name;
        }

        private string ConvertHtmlToPdf(string htmlPath, string campaignName)
        {
            string name = campaignName + ".pdf";
            name = Server.UrlEncode(name).Replace('+', '_').Replace('%', '_');

            string pdfPath = mReportRoot + "Print/" + name;
            try
            {
                //if (string.IsNullOrEmpty(htmlPath) || string.IsNullOrEmpty(pdfPath))
                //    return string.Empty;
                //Process p = new Process();
                //string str = System.Web.HttpContext.Current.Server.MapPath("~/Print/wkhtmltopdf-0.8.3.exe");
                //if (!System.IO.File.Exists(str))
                //    return string.Empty;
                //p.StartInfo.FileName = str;
                //p.StartInfo.Arguments = "--margin-left 5 --margin-right 5 --margin-top 15 \"" + htmlPath + "\" " + pdfPath;
                //p.StartInfo.UseShellExecute = false;
                //p.StartInfo.RedirectStandardInput = true;
                //p.StartInfo.RedirectStandardOutput = true;
                //p.StartInfo.RedirectStandardError = true;
                //p.StartInfo.CreateNoWindow = true;
                //p.Start();
                //p.WaitForExit(30000);


                PdfConverter pdfConverter = new PdfConverter();
                pdfConverter.AvoidImageBreak = true;
                pdfConverter.LicenseKey = System.Configuration.ConfigurationManager.AppSettings["PdfConverterKey"];

                //pdfConverter.PageWidth = 982;
                //pdfConverter.
                pdfConverter.PdfDocumentInfo.AuthorName = "Harry Duan";
                pdfConverter.PdfDocumentInfo.CreatedDate = DateTime.Now;
                pdfConverter.PdfDocumentInfo.Subject = "Sample PDF created by ExpertPDF";

                //pdfConverter.PdfSecurityOptions.KeySize = EncryptionKeySize.EncryptKey128Bit;
                //pdfConverter.PdfSecurityOptions.UserPassword = "Casey";
                //pdfConverter.PdfDocumentOptions.FitHeight = false;
                //pdfConverter.PdfDocumentOptions.FitWidth = false;
                //pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;
                pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Best;
                pdfConverter.PdfDocumentOptions.EmbedFonts = false;
                pdfConverter.PdfDocumentOptions.ShowFooter = true;
                pdfConverter.PdfDocumentOptions.ShowHeader = false;
                pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
                pdfConverter.PdfDocumentOptions.LeftMargin = 10;
                pdfConverter.PdfDocumentOptions.RightMargin = 10;
                pdfConverter.PdfDocumentOptions.TopMargin = 35;

                pdfConverter.PdfFooterOptions.DrawFooterLine = true;

                //pdfConverter.PdfFooterOptions.AddHtmlToPdfArea(new HtmlToPdfArea(FooterUrl));
                pdfConverter.PdfFooterOptions.ShowPageNumber = false;
                pdfConverter.PdfFooterOptions.AddTextArea(new TextArea(265, 3,
                                                                       "Page &p; of &P;",
                                                                       new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 6)));

                pdfConverter.SavePdfFromUrlToFile(htmlPath, pdfPath);
            }
            catch (Exception ex)
            {
                GPS.Utilities.LogUtils.Error("Web application Unhandle error", ex);
                HttpContext.Current.Response.Write(ex);
            }
            return name;
        }

        private void OutputPdf(string pdfPath)
        {
            //string url = Request.MapPath(pdfPath.Substring(Server.MapPath("~/").Length));
            //string url = pdfPath.Substring(pdfPath.ToLower().IndexOf("\\print\\") + 7);
            Response.Redirect(mReportUrl + "Print/" + pdfPath);
        }


    }
}
