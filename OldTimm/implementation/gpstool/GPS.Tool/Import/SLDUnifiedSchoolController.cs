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
    public class SLDUnifiedSchoolController : ImportAreaController {
        protected override void Importting(object data) {
            Dictionary<FileInfo, FileInfo> fileDictionary = (Dictionary<FileInfo, FileInfo>)data;

            AreaDataContext dataContext = new AreaDataContext();

            UnifiedSchoolAreaRepository uniSchoolRep = new UnifiedSchoolAreaRepository();
            uniSchoolRep.DataContext = dataContext;

            UnifiedSchoolAreaCoordinateRepository uniSchoolCoordRep = new UnifiedSchoolAreaCoordinateRepository();
            uniSchoolCoordRep.DataContext = dataContext;

            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary) {
                DataTable dt = new DataTable();
                ShpReader shpReader = new ShpReader();
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import School Unified,State:" + dbfFile.Name.Substring(0, 4));
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting School Unified Area from :" + dbfFile.Name);
                List<UnifiedSchoolArea> uSchoolAreaList =
                    UnifiedSchoolAreaBusiness.GetUnifiedSchoolAreas(
                    dt, shpReader, dbfFile.Name.Substring(2, 2));

                SendStartMessage("Current Importing: School Unified Area Total: " + uSchoolAreaList.Count + "File Name:" + dbfFile.FullName);

                try {
                    index = uniSchoolRep.AddAll(uSchoolAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                    throw ex;
                }
                SendMessage(true, uSchoolAreaList.Count, "School Unified Area information be imported ", dbfFile.Name);

                SendStartMessage("Getting School Unified coordinates from :" + shpFile.Name);
                List<UnifiedSchoolAreaCoordinate> uSchoolCoordList =
                    UnifiedSchoolAreaBusiness.GetUnifiedSchoolAreaCoords(
                    shpReader.polygons, ref index);

                SendStartMessage("Current Importing School Unified coordinates Total: " + uSchoolCoordList.Count + " from :" + shpFile.FullName);

                try {
                    for (Int32 u = 0; u * 1000 < uSchoolCoordList.Count; u++) {
                        IEnumerable<UnifiedSchoolAreaCoordinate> section = uSchoolCoordList.Skip(u * 1000).Take(1000);
                        List<UnifiedSchoolAreaCoordinate> coordinates = new List<UnifiedSchoolAreaCoordinate>(section);
                        uniSchoolCoordRep.AddAll(coordinates);
                    }

                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                    throw ex;
                }
                SendMessage(true, uSchoolCoordList.Count, "School Unified Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
