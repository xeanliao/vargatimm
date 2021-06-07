using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;

namespace GPSOfficeHelper
{
    public partial class GPSOfficeHelper : ServiceBase
    {
        public GPSOfficeHelper()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //ExcelHelper helper = new ExcelHelper();
            //if (helper.WriteAreaRecord("aa.xls", new AreaRecord[] {
            //    new AreaRecord(){ Code="aaa", Total ="ttt", Penetration ="ppp"},
            //    new AreaRecord(){Code = "aaa1",Total ="ttt1", Penetration="ppp1"}
            //}))
            //{
            //    File.Create("e:\\write - yes.txt");
            //}
            //else
            //{
            //    File.Create("e:\\write - no.txt");
            //}
        }

        protected override void OnStop()
        {
            //ExcelHelper helper = new ExcelHelper();
            //AreaRecord[] records = helper.ReadAreaRecords("aa.xls");
            //if (records.Count() > 0 && records[1].Total == "ttt1")
            //{
            //    File.Create("e:\\read - yes.txt");
            //}
            //else
            //{
            //    File.Create("e:\\read - no.txt");
            //}
        }
    }
}
