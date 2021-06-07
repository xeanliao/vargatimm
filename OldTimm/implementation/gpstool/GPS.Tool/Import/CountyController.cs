using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using GPS.Tool.Data;
using GPS.Tool.AreaBusiness;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace GPS.Tool.Import
{
    public class CountyController:ImportAreaController
    {
        protected override void Importting(object data)
        {
            DataTable dt = new DataTable();
            Dictionary<FileInfo, FileInfo> fileDictionary =
                (Dictionary<FileInfo, FileInfo>)data;

            CountyRepository countyRep = new CountyRepository();

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                //FileInfo dbfFile = entry.Value;
                //SendStartMessage("Start to Import County,State:" +
                //    dbfFile.Name.Substring(0, 4));
                //ReadArea.ReadFile(dbfFile.Name,
                //    dbfFile.FullName.Substring(0, dbfFile.FullName.LastIndexOf("\\") + 1),
                //    ref dt);

                //SendStartMessage("Getting County Area from :" + dbfFile.Name);
                //List<County> countyList = GetCountys(dt);

                //SendStartMessage("Current Importing: County Area Total: " +
                //    countyList.Count + "File Name:" + dbfFile.FullName);
                //try
                //{
                //    countyRep.AddAll(countyList);
                //    SetSuccessMessage(resultDict, ref importResult);
                //}
                //catch (Exception ex)
                //{
                //    SetFailedMessage(resultDict, errorDict,
                //        ref importResult, dbfFile.FullName, ex.Message);
                //}
                //SendMessage(true, countyList.Count,
                //    "County Area information be imported ", dbfFile.Name);
            }
        }

        //public static List<County> GetCountys(DataTable dt)
        //{
        //    //List<County> countyList = new List<County>();
        //    //foreach (DataRow dr in dt.Rows)
        //    //{
        //    //    County isExist = countyList.Where(c => c.Code == dr["COUNTY"].ToString() &&
        //    //        c.StateCode == dr["STATE"].ToString()).FirstOrDefault();
        //    //    if (isExist == null)
        //    //    {
        //    //        County county = new County();
        //    //        county.Code = dr["COUNTY"].ToString();
        //    //        county.Name = dr["NAME"].ToString();
        //    //        county.StateCode = dr["STATE"].ToString();
        //    //        county.LSAD = dr["LSAD"].ToString();
        //    //        county.LSAD_TRAN = dr["LSAD_TRANS"].ToString();
        //    //        countyList.Add(county);
        //    //    }
        //    //}
        //    //return countyList;
        //}
    }
}
