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
    public class BgAreaController:ImportAreaController
    {
        protected override void Importting(object data)
        {
            DataTable dt = new DataTable();
            ShpReader shpReader = new ShpReader();
            Dictionary<FileInfo, FileInfo> fileDictionary =
                (Dictionary<FileInfo, FileInfo>)data;
            BGAreaRepository bgRep = new BGAreaRepository();
            BGAreaCoordRepository bgCoordRep = new BGAreaCoordRepository();
            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import BG'S,State:" +
                    dbfFile.Name.Substring(0, 4));
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting BG'S Area from :" + dbfFile.Name);
                List<BlockGroup> bgAreaList = BGAreaBusiness.GetBGAreas(dt,shpReader);

                SendStartMessage("Current Importing: BG'S Area Total: " +
                    bgAreaList.Count + "File Name:" + dbfFile.FullName);
                try
                {
                    index = bgRep.AddAll(bgAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                }
                catch (Exception ex)
                {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(true, bgAreaList.Count,
                    "BG'S Area information be imported ", dbfFile.Name);

                SendStartMessage("Getting BG'S coordinates from :" + shpFile.Name);
                List<BlockGroupCoordinate> bgAreaCoordList =
                    BGAreaBusiness.GetBGAreaCoords(shpReader.polygons, ref index);
                SendStartMessage("Current Importing BG'S coordinates Total: " +
                    bgAreaCoordList.Count + " from :" + shpFile.FullName);
                try{
                    for (int i = 0; i * 5000 < bgAreaCoordList.Count; i++)
                    {
                        var items = bgAreaCoordList.Skip(i * 5000).Take(5000);
                        bgCoordRep.AddAll(items.ToList());
                    }
                    SetSuccessMessage(resultDict, ref importResult);
                }
                catch (Exception ex)
                {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(true, bgAreaCoordList.Count,
                    "BG'S Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
