using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using GPS.Tool.Data;
using GPS.Tool.AreaBusiness;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace GPS.Tool.Import {
    public class SLDHouseAreaController : ImportAreaController {
        protected override void Importting(object data) {
            Dictionary<FileInfo, FileInfo> fileDictionary = (Dictionary<FileInfo, FileInfo>)data;

            AreaDataContext dataContext = new AreaDataContext();

            LowerHouseAreaRepository houseRep = new LowerHouseAreaRepository();
            houseRep.DataContext = dataContext;

            LowerHouseAreaCoordRepository houseCoordRep = new LowerHouseAreaCoordRepository();
            houseCoordRep.DataContext = dataContext;
            
            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary) {
                DataTable dt = new DataTable();
                ShpReader shpReader = new ShpReader();
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import Lower/House,State:" + dbfFile.Name.Substring(0, 4));

                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting Lower/House Area from :" + dbfFile.Name);

                List<LowerHouseArea> houseAreaList = SLDHouseAreaBusiness.GetSLDHouseAreas(dt, shpReader);

                SendStartMessage("Current Importing: Lower/House Area Total: " + houseAreaList.Count + "File Name:" + dbfFile.FullName);

                try {
                    index = houseRep.AddAll(houseAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }

                SendMessage(true, houseAreaList.Count, "Lower/House Area information be imported ", dbfFile.Name);
                SendStartMessage("Getting Lower/House coordinates from :" + shpFile.Name);

                List<LowerHouseAreaCoordinate> houseAreaCoordList =
                    SLDHouseAreaBusiness.GetSLDHouseAreaCoords(
                    shpReader.polygons, ref index);

                SendStartMessage("Current Importing Lower/House coordinates Total: " + houseAreaCoordList.Count + " from :" + shpFile.FullName);

                try {
                    for (Int32 u = 0; u * 1000 < houseAreaCoordList.Count; u++) {
                        IEnumerable<LowerHouseAreaCoordinate> section = houseAreaCoordList.Skip(u * 1000).Take(1000);
                        List<LowerHouseAreaCoordinate> coordinates = new List<LowerHouseAreaCoordinate>(section);
                        houseCoordRep.AddAll(coordinates);
                    }
                    
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }

                SendMessage(true, houseAreaCoordList.Count, "Lower/House Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
