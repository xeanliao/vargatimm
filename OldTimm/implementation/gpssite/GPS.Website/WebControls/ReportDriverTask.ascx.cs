﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.DomainLayer.Entities;
using GPS.DataLayer;

namespace GPS.Website.WebControls
{
    public partial class ReportDriverTask : System.Web.UI.UserControl
    {
        public int tId = 33;
        public IList<decimal> speedList = new List<decimal>();
        public IList<decimal> groundList = new List<decimal>();
        public IList<double> stopList = new List<double>();
        public IList<decimal> speedListYear = new List<decimal>();
        public IList<decimal> groundListYear = new List<decimal>();
        public IList<double> stopListYear = new List<double>();
        public IList<decimal> speedListAll = new List<decimal>();
        public IList<decimal> groundListAll = new List<decimal>();
        public IList<double> stopListAll = new List<double>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                if (CurrentUser != null)
                {
                    this.lblDriverName.Text = CurrentUser.FullName + ":";
                    tId = Convert.ToInt32(this.Request.QueryString["tid"]);
                    //IList<decimal> speedList = null;
                    //IList<decimal> groundList = null;
                    //IList<double> stopList = null;
                    //IList<decimal> speedListYear = null;
                    //IList<decimal> groundListYear = null;
                    //IList<double> stopListYear = null;
                    //IList<decimal> speedListAll = null;
                    //IList<decimal> groundListAll = null;
                    //IList<double> stopListAll = null;
                    using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
                    {
                        speedList = ws.Repositories.UserRepository.GetSpeedByTaskidUserid(tId, CurrentUser.Id);
                        groundList = ws.Repositories.UserRepository.GetGroundByTaskidUserid(tId, CurrentUser.Id);
                        stopList = ws.Repositories.UserRepository.GetStopByTaskidUserid(tId, CurrentUser.Id);
                        speedListYear = ws.Repositories.UserRepository.GetSpeedByUseridYear(CurrentUser.Id);
                        groundListYear = ws.Repositories.UserRepository.GetGroundByUseridYear(CurrentUser.Id);
                        stopListYear = ws.Repositories.UserRepository.GetStopByUseridYear(CurrentUser.Id);
                        speedListAll = ws.Repositories.UserRepository.GetSpeedByUseridAll(CurrentUser.Id);
                        groundListAll = ws.Repositories.UserRepository.GetGroundByUseridAll(CurrentUser.Id);
                        stopListAll = ws.Repositories.UserRepository.GetStopByUseridAll(CurrentUser.Id);

                        this.avgSpeedN.Text = speedList[0].ToString();
                        this.highSpeedN.Text = speedList[1].ToString();
                        this.lowSpeedN.Text = speedList[2].ToString();
                        this.avgSpeedYTD.Text = speedListYear[0].ToString();
                        this.highSpeedYTD.Text = speedListYear[1].ToString();
                        this.lowSpeedYTD.Text = speedListYear[2].ToString();
                        this.avgSpeedLF.Text = speedListAll[0].ToString();
                        this.highSpeedLF.Text = speedListAll[1].ToString();
                        this.lowSpeedLF.Text = speedListAll[2].ToString();

                        this.avgGroundN.Text = groundList[0].ToString();
                        this.highGroundN.Text = groundList[1].ToString();
                        this.lowGroundN.Text = groundList[2].ToString();
                        this.avgGroundYTD.Text = groundListYear[0].ToString();
                        this.highGroundYTD.Text = groundListYear[1].ToString();
                        this.lowGroundYTD.Text = groundListYear[2].ToString();
                        this.avgGroundLF.Text = groundListAll[0].ToString();
                        this.highGroundLF.Text = groundListAll[1].ToString();
                        this.lowGroundLF.Text = groundListAll[2].ToString();

                        this.avgStopN.Text = stopList[0].ToString();
                        this.highStopN.Text = stopList[1].ToString();
                        this.lowStopN.Text = stopList[2].ToString();
                        this.avgStopYTD.Text = stopListYear[0].ToString();
                        this.highStopYTD.Text = stopListYear[1].ToString();
                        this.lowStopYTD.Text = stopListYear[2].ToString();
                        this.avgStopLF.Text = stopListAll[0].ToString();
                        this.highStopLF.Text = stopListAll[1].ToString();
                        this.lowStopLF.Text = stopListAll[2].ToString();
                        //--test data-----------------------------------------------
                        //this.avgSpeedN.Text = "5";
                        //this.highSpeedN.Text = "7";
                        //this.lowSpeedN.Text = "3";
                        //this.avgSpeedYTD.Text = "5.5";
                        //this.highSpeedYTD.Text = "6";
                        //this.lowSpeedYTD.Text = "5";
                        //this.avgSpeedLF.Text = "9";
                        //this.highSpeedLF.Text = "15";
                        //this.lowSpeedLF.Text = "3";

                        //this.avgGroundN.Text = "11";
                        //this.highGroundN.Text = "12";
                        //this.lowGroundN.Text = "10";
                        //this.avgGroundYTD.Text = "14";
                        //this.highGroundYTD.Text = "16";
                        //this.lowGroundYTD.Text = "12";
                        //this.avgGroundLF.Text = "20";
                        //this.highGroundLF.Text = "30";
                        //this.lowGroundLF.Text = "10";

                        //this.avgStopN.Text = "2.5";
                        //this.highStopN.Text = "4";
                        //this.lowStopN.Text = "1";
                        //this.avgStopYTD.Text = "2.5";
                        //this.highStopYTD.Text = "4";
                        //this.lowStopYTD.Text = "1";
                        //this.avgStopLF.Text = "3";
                        //this.highStopLF.Text = "5";
                        //this.lowStopLF.Text = "1";
                    }
                    //calculate the summary data


                    //((this.Page.FindControl("avgSpeedND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highSpeedND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowSpeedND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("avgSpeedYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highSpeedYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowSpeedYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("avgSpeedAD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highSpeedAD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowSpeedAD")) as Label).Text = "aaaaaaa";

                    //((this.Page.FindControl("avgGroundND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highGroundND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowGroundND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("avgGroundYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highGroundYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowGroundYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("avgGroundAD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highGroundAD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowGroundAD")) as Label).Text = "aaaaaaa";

                    //((this.Page.FindControl("avgStopND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highStopND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowStopND")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("avgStopYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highStopYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowStopYD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("avgStopAD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("highStopAD")) as Label).Text = "aaaaaaa";
                    //((this.Page.FindControl("lowStopAD")) as Label).Text = "aaaaaaa";
                   
                }
            }

        }

        public User CurrentUser
        {
            get;
            set;
        }

    }
}