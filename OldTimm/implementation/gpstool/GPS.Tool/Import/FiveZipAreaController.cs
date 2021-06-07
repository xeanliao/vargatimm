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
    public class FiveZipAreaController : ImportAreaController
    {
        protected override void Importting(object data)
        {
            DataTable dt = new DataTable();
            ShpReader shpReader = new ShpReader();
            Dictionary<FileInfo, FileInfo> fileDictionary =
                (Dictionary<FileInfo, FileInfo>)data;
            FiveZipAreaRepository fiveZipRep = new FiveZipAreaRepository();
            FiveZipAreaCoordRepository fiveZipCoordRep =
                new FiveZipAreaCoordRepository();
            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import Z5,State:" +
                    dbfFile.Name.Substring(0, 4));
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                        0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);
                SendStartMessage("Getting Z5 Area from :" + dbfFile.Name);
                List<FiveZipArea> fiveZipAreaList = FiveZipAreaBusiness.GetFiveZipAreas(
                    dt, shpReader, dbfFile.Name.Substring(2, 2));
                SendStartMessage("Current Importing: Z5 Area Total: " +
                    fiveZipAreaList.Count + "File Name:" + dbfFile.FullName);
                try
                {
                    index = fiveZipRep.AddAll(fiveZipAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                }
                catch (Exception ex)
                {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(importResult, fiveZipAreaList.Count,
                    "Z5 Area information be imported ", dbfFile.Name);

                SendStartMessage("Getting Z5 coordinates from :" + shpFile.Name);
                List<FiveZipAreaCoordinate> fiveZipAreaCoordList =
                    FiveZipAreaBusiness.GetFiveZipAreaCoords(
                    shpReader.polygons, ref index);
                SendStartMessage("Current Importing Z5 coordinates Total: " +
                    fiveZipAreaCoordList.Count + " from :" + shpFile.FullName);
                try
                {
                    for (int i = 0; i * 5000 < fiveZipAreaCoordList.Count; i++)
                    {
                        var items = fiveZipAreaCoordList.Skip(i * 5000).Take(5000);
                        fiveZipCoordRep.AddAll(items.ToList());
                    }
                    SetSuccessMessage(resultDict, ref importResult);
                }
                catch (Exception ex)
                {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(importResult, fiveZipAreaCoordList.Count,
                    "Z5 Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
