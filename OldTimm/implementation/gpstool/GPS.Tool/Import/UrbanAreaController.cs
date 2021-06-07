using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Transactions;
using GPS.Tool.Data;
using GPS.Tool.AreaBusiness;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace GPS.Tool.Import {
    public class UrbanAreaController : ImportAreaController {
        protected override void Importting(object data) {
            DataTable dt = new DataTable();
            ShpReader shpReader = new ShpReader();
            Dictionary<FileInfo, FileInfo> fileDictionary = (Dictionary<FileInfo, FileInfo>)data;

            AreaDataContext dataContext = new AreaDataContext();

            UrbanAreaRepository urbanAreaRep = new UrbanAreaRepository();
            urbanAreaRep.DataContext = dataContext;

            UrbanAreaCoordinateRepository urbanAreaCoordRep = new UrbanAreaCoordinateRepository();
            urbanAreaCoordRep.DataContext = dataContext;

            int index = 1;
            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary) {
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import Urban,State:" + dbfFile.Name.Substring(0, 4));
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);
                SendStartMessage("Getting Urban Area from :" + dbfFile.Name);
                List<UrbanArea> urbanAreaList = UrbanAreaBusiness.GetUrbanAreas(dt, shpReader);

                SendStartMessage("Current Importing: Urban Area Total: " + urbanAreaList.Count + "File Name:" + dbfFile.FullName);
                try {
                    index = urbanAreaRep.AddAll(urbanAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(true, urbanAreaList.Count, "Urban Area information be imported ", dbfFile.Name);

                SendStartMessage("Getting Urban coordinates from :" + shpFile.Name);
                List<UrbanAreaCoordinate> urbanAreaCoordList = UrbanAreaBusiness.GetUrbanAreaCoords(shpReader.polygons, ref index);

                SendStartMessage("Current Importing Urban coordinates Total: " + urbanAreaCoordList.Count + " from :" + shpFile.FullName);
                try {
                    for (Int32 u = 0; u * 10000 < urbanAreaCoordList.Count(); u++) {
                        IEnumerable<UrbanAreaCoordinate> urbans = urbanAreaCoordList.Skip(u * 10000).Take(10000);
                        urbanAreaCoordRep.AddAll(new List<UrbanAreaCoordinate>(urbans));
                    }
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                    throw ex;
                }
                SendMessage(true, urbanAreaCoordList.Count,
                    "Urban Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
