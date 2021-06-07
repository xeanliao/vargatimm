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
    public class ElementarySchoolAreaController : ImportAreaController {
        protected override void Importting(object data) {
            Dictionary<FileInfo, FileInfo> fileDictionary = (Dictionary<FileInfo, FileInfo>)data;

            AreaDataContext dataContext = new AreaDataContext();

            ElementarySchoolAreaRepository eleSchoolRep = new ElementarySchoolAreaRepository();
            eleSchoolRep.DataContext = dataContext;

            ElementarySchoolAreaCoordRepository eleSchoolCoordRep = new ElementarySchoolAreaCoordRepository();
            eleSchoolCoordRep.DataContext = dataContext;

            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary) {
                DataTable dt = new DataTable();
                ShpReader shpReader = new ShpReader();
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;

                ShowShape(shpFile.FullName);
                
                SendStartMessage("Start to Import School Districts Elementary,State:" + dbfFile.Name.Substring(0, 4));

                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting School Districts Elementary Area from :" + dbfFile.Name);
                List<ElementarySchoolArea> eleSchoolAreaList =
                    ElementarySchoolAreaBusiness.GetElementarySchoolAreas(
                    dt, shpReader, dbfFile.Name.Substring(2, 2));

                SendStartMessage("Current Importing: School Districts Elementary Area Total: " + eleSchoolAreaList.Count + "File Name:" + dbfFile.FullName);
                try {
                    index = eleSchoolRep.AddAll(eleSchoolAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }
                
                SendMessage(true, eleSchoolAreaList.Count, "School Districts Elementary Area information be imported ", dbfFile.Name);
                SendStartMessage("Getting School Districts Elementary coordinates from :" + shpFile.Name);

                List<ElementarySchoolAreaCoordinate> eleSchoolAreaCoordList =
                    ElementarySchoolAreaBusiness.GetElementarySchoolAreaCoords(
                    shpReader.polygons, ref index);

                SendStartMessage("Current Importing School Districts Elementary coordinates Total: " + eleSchoolAreaCoordList.Count + " from :" + shpFile.FullName);
                
                try {
                    for (Int32 u = 0; u * 1000 < eleSchoolAreaCoordList.Count; u++) {
                        IEnumerable<ElementarySchoolAreaCoordinate> section = eleSchoolAreaCoordList.Skip(u * 1000).Take(1000);
                        List<ElementarySchoolAreaCoordinate> coordinates = new List<ElementarySchoolAreaCoordinate>(section);
                        eleSchoolCoordRep.AddAll(coordinates);
                    }
                    
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(true, eleSchoolAreaCoordList.Count, "School Districts Elementary Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
