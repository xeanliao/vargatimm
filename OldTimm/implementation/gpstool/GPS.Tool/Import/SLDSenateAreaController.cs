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
    public class SLDSenateAreaController : ImportAreaController {
        protected override void Importting(object data) {
            Dictionary<FileInfo, FileInfo> fileDictionary = (Dictionary<FileInfo, FileInfo>)data;

            AreaDataContext dataContext = new AreaDataContext();

            SenateAreaRepository uSenateRep = new SenateAreaRepository();
            uSenateRep.DataContext = dataContext;

            SenateAreaCoordRepository uSenateCoordRep = new SenateAreaCoordRepository();
            uSenateCoordRep.DataContext = dataContext;

            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary) {
                DataTable dt = new DataTable();
                ShpReader shpReader = new ShpReader();

                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import Upper/Senate,State:" + dbfFile.Name.Substring(0, 4));
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting Upper/Senate Area from:" + dbfFile.Name);
                List<UpperSenateArea> senateAreaList = SLDSenateAreaBusiness.GetSLDSenateAreas(dt, shpReader);

                SendStartMessage("Current Importing: Upper/Senate Area Total:" + senateAreaList.Count + "File Name:" + dbfFile.FullName);
                try {
                    index = uSenateRep.AddAll(senateAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(true, senateAreaList.Count, "Upper/Senate Area information be imported ", dbfFile.Name);

                SendStartMessage("Getting Upper/Senate coordinates from :" + shpFile.Name);
                List<UpperSenateAreaCoordinate> senateCoordList =
                    SLDSenateAreaBusiness.GetSLDSenateAreaCoordinates(
                    shpReader.polygons, ref index);

                SendStartMessage("Current Importing Upper/Senate coordinates Total: " +
                    senateCoordList.Count + " from :" + shpFile.FullName);
                try {
                    for (Int32 s = 0; s * 1000 < senateCoordList.Count; s++) {
                        IEnumerable<UpperSenateAreaCoordinate> section = senateCoordList.Skip(s * 1000).Take(1000);
                        List<UpperSenateAreaCoordinate> coordinates = new List<UpperSenateAreaCoordinate>(section);
                        uSenateCoordRep.AddAll(coordinates);
                    }
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                    throw ex;
                }
                SendMessage(true, senateCoordList.Count,
                    "Upper/Senate Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
