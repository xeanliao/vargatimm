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
    public class TrkAreaController:ImportAreaController
    {
        protected override void Importting(object data)
        {
            DataTable dt = new DataTable();
            ShpReader shpReader = new ShpReader();
            Dictionary<FileInfo, FileInfo> fileDictionary =
                (Dictionary<FileInfo, FileInfo>)data;
            TRKAreaRepository trkRep = new TRKAreaRepository();
            TRKAreaCoordinateRepository trkCoordRep = 
                new TRKAreaCoordinateRepository();
            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import Tract,State:" +
                    dbfFile.Name.Substring(0, 4));
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting Tract Area from :" + dbfFile.Name);
                List<Tract> trkList = TRKAreaBusiness.GetTRKAreas(dt,shpReader);
                SendStartMessage("Current Importing: Tract Area Total: " +
                    trkList.Count + "File Name:" + dbfFile.FullName);
                try{
                    index = trkRep.AddAll(trkList);
                    SetSuccessMessage(resultDict, ref importResult);
                }
                catch (Exception ex)
                {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(true, trkList.Count,
                    "Tract Area information be imported ", dbfFile.Name);

                SendStartMessage("Getting Tract coordinates from :" + shpFile.Name);
                List<TractCoordinate> trkCoordList =
                    TRKAreaBusiness.GetTRKAreaCoords(shpReader.polygons, ref index);
                SendStartMessage("Current Importing Tract coordinates Total: " +
                    trkCoordList.Count + " from :" + shpFile.FullName);
                try{
                    for (int i = 0; i * 5000 < trkCoordList.Count; i++)
                    {
                        var items = trkCoordList.Skip(i * 5000).Take(5000);
                        trkCoordRep.AddAll(items.ToList());
                    }
                    SetSuccessMessage(resultDict, ref importResult);
                }
                catch (Exception ex)
                {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(true, trkCoordList.Count,
                    "Tract Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
