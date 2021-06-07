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
    public class CountyAreaController : ImportAreaController {
        protected override void Importting(Object data) {
            Dictionary<FileInfo, FileInfo> fileDictionary = (Dictionary<FileInfo, FileInfo>)data;

            AreaDataContext dataContext = new AreaDataContext();

            CountyAreaRepository countyAreaRep = new CountyAreaRepository();
            countyAreaRep.DataContext = dataContext;

            CountyAreaCoordRepository countyAreaCoordRep = new CountyAreaCoordRepository();
            countyAreaCoordRep.DataContext = dataContext;

            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary) {
                DataTable dt = new DataTable();
                ShpReader shpReader = new ShpReader();
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                
                ShowShape(shpFile.FullName);
                SendStartMessage("Start to Import County,State:" + dbfFile.Name.Substring(0, 4));

                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting County Area from :" + dbfFile.Name);
                List<CountyArea> countyAreaList = CountyAreaBusiness.GetCountyAreas(dt, shpReader); // append county

                SendStartMessage("Current Importing: County Area Total: " + countyAreaList.Count + "File Name:" + dbfFile.FullName);
                
                try {
                    index = countyAreaRep.AddAll(countyAreaList);
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }

                SendMessage(true, countyAreaList.Count, "County Area information be imported ", dbfFile.Name);
                SendStartMessage("Getting County coordinates from :" + shpFile.Name);

                List<CountyAreaCoordinate> countyAreaCoordList =
                    CountyAreaBusiness.GetCountyAreaCoordinatas(
                    shpReader.polygons, ref index);

                SendStartMessage("Current Importing County coordinates Total: " + countyAreaCoordList.Count + " from :" + shpFile.FullName);

                try {
                    for (Int32 u = 0; u * 1000 < countyAreaCoordList.Count; u++) {
                        IEnumerable<CountyAreaCoordinate> section = countyAreaCoordList.Skip(u * 1000).Take(1000);
                        List<CountyAreaCoordinate> coordinates = new List<CountyAreaCoordinate>(section);
                        countyAreaCoordRep.AddAll(coordinates);
                    }

                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }

                SendMessage(true, countyAreaCoordList.Count, "County Area coordinates be imported ", shpFile.Name);
            }

            SendResultMessage();
            WriteErrorLog();
        }
    }
}
