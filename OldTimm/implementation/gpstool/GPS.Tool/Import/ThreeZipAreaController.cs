using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using GPS.Tool.Data;
using GPS.Tool.AreaBusiness;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace GPS.Tool.Import
{
    public class ThreeZipAreaController:ImportAreaController
    {
        protected override void Importting(object data)
        {
            DataTable dt = new DataTable();
            ShpReader shpReader = new ShpReader();
            Dictionary<FileInfo, FileInfo> fileDictionary =
                (Dictionary<FileInfo, FileInfo>)data;

            ThreeZipAreaRepository threeZipRep = new ThreeZipAreaRepository();
            ThreeZipCoordinateRepository threeZipCoordRep = 
                new ThreeZipCoordinateRepository();

            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import Z3,State:" +
                    dbfFile.Name.Substring(0, 4));
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                        0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);
                SendStartMessage("Getting Z3 Area from :" + dbfFile.Name);
                List<ThreeZipArea> threeZipAreaList = ThreeZipBusiness.GetThreeZipAreas(
                    dt, shpReader, dbfFile.Name.Substring(2, 2));
                SendStartMessage("Current Importing: Z3 Area Total: " +
                    threeZipAreaList.Count + "File Name:" + dbfFile.FullName);
                try
                {
                    index = threeZipRep.AddAll(threeZipAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                }
                catch (Exception ex)
                {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(importResult, threeZipAreaList.Count,
                    "Z3 Area information be imported ", dbfFile.Name);

                SendStartMessage("Getting Z3 coordinates from :" + shpFile.Name);
                List<ThreeZipAreaCoordinate> threeZipAreaCoordList =
                    ThreeZipBusiness.GetThreeZipAreaCoords(
                    shpReader.polygons, ref index);
                SendStartMessage("Current Importing Z3 coordinates Total: " +
                    threeZipAreaCoordList.Count + " from :" + shpFile.FullName);
                try
                {
                    for(int i =0; i * 5000 < threeZipAreaCoordList.Count; i ++)
                    {
                        var items = threeZipAreaCoordList.Skip(i * 5000).Take(5000);
                        threeZipCoordRep.AddAll(items.ToList());
                    }
                    
                    SetSuccessMessage(resultDict, ref importResult);
                }
                catch (Exception ex)
                {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(importResult, threeZipAreaCoordList.Count,
                    "Z3 Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
