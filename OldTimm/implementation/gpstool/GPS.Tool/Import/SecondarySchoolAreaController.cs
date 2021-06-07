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
    public class SecondarySchoolAreaController : ImportAreaController {
        protected override void Importting(object data) {
            Dictionary<FileInfo, FileInfo> fileDictionary = (Dictionary<FileInfo, FileInfo>)data;

            AreaDataContext dataContext = new AreaDataContext();

            SecondarySchoolAreaRepository secSchoolRep = new SecondarySchoolAreaRepository();
            secSchoolRep.DataContext = dataContext;
            
            SecondarySchoolAreaCoordRepository secSchoolCoordRep = new SecondarySchoolAreaCoordRepository();
            secSchoolCoordRep.DataContext = dataContext;
            
            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary) {
                DataTable dt = new DataTable();
                ShpReader shpReader = new ShpReader();
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);

                SendStartMessage("Start to Import School Districts Secondary,State:" + dbfFile.Name.Substring(0, 4));
                
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting School Districts Secondary Area from :" + dbfFile.Name);

                List<SecondarySchoolArea> secSchoolAreaList =
                    SecondarySchoolAreaBusiness.GetSecondarySchoolAreas(
                    dt, shpReader, dbfFile.Name.Substring(2, 2));

                SendStartMessage("Current Importing: School Districts Secondary Area Total: " + secSchoolAreaList.Count + "File Name:" + dbfFile.FullName);
                
                try {
                    index = secSchoolRep.AddAll(secSchoolAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }
                
                SendMessage(true, secSchoolAreaList.Count, "School Districts Secondary Area information be imported ", dbfFile.Name);
                SendStartMessage("Getting School Districts Secondary coordinates from :" + shpFile.Name);

                List<SecondarySchoolAreaCoordinate> secSchoolAreaCoordList =
                    SecondarySchoolAreaBusiness.GetSecondarySchoolAreaCoords(
                    shpReader.polygons, ref index);

                SendStartMessage("Current Importing School Districts Secondary coordinates Total: " + secSchoolAreaCoordList.Count + " from :" + shpFile.FullName);
                
                try {
                    for (Int32 u = 0; u * 1000 < secSchoolAreaCoordList.Count; u++) {
                        IEnumerable<SecondarySchoolAreaCoordinate> section = secSchoolAreaCoordList.Skip(u * 1000).Take(1000);
                        List<SecondarySchoolAreaCoordinate> coordinates = new List<SecondarySchoolAreaCoordinate>(section);
                        secSchoolCoordRep.AddAll(coordinates);
                    }
                    
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }
                SendMessage(true, secSchoolAreaCoordList.Count,
                    "School Districts Secondary Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
