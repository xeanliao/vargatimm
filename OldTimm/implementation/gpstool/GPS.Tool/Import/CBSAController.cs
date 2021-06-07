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
    public class CBSAController : ImportAreaController {
        protected override void Importting(object data) {
            Dictionary<FileInfo, FileInfo> fileDictionary = (Dictionary<FileInfo, FileInfo>)data;

            AreaDataContext dataContext = new AreaDataContext();

            CBSARepository cbsaRep = new CBSARepository();
            cbsaRep.DataContext = dataContext;

            CBSACoordRepository cbsaCoordRep = new CBSACoordRepository();
            cbsaCoordRep.DataContext = dataContext;

            int index = 1;

            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary) {
                DataTable dt = new DataTable();
                ShpReader shpReader = new ShpReader();
                FileInfo shpFile = entry.Key;
                FileInfo dbfFile = entry.Value;
                ShowShape(shpFile.FullName);

                SendStartMessage("Start to Import BG'S,State:" + dbfFile.Name.Substring(0, 4));
                
                ReadArea.ReadFile(shpFile.Name,
                    dbfFile.Name,
                    dbfFile.FullName.Substring(
                    0, dbfFile.FullName.LastIndexOf("\\") + 1),
                    ref shpReader,
                    ref dt);

                SendStartMessage("Getting BG'S Area from :" + dbfFile.Name);
                
                List<MetropolitanCoreArea> cbsaList = CBSABusiness.GetCBSAAreas(dt, shpReader);

                SendStartMessage("Current Importing: BG'S Area Total: " + cbsaList.Count + "File Name:" + dbfFile.FullName);

                try {
                    index = cbsaRep.AddAll(cbsaList);
                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict,
                        ref importResult, dbfFile.FullName, ex.Message);
                }
                
                SendMessage(true, cbsaList.Count, "BG'S Area information be imported ", dbfFile.Name);
                SendStartMessage("Getting BG'S coordinates from :" + shpFile.Name);

                List<MetropolitanCoreAreaCoordinate> cbsaCoordList = CBSABusiness.GetCBSAAreaCoords(shpReader.polygons, ref index);

                SendStartMessage("Current Importing BG'S coordinates Total: " + cbsaCoordList.Count + " from :" + shpFile.FullName);

                try {
                    for (Int32 u = 0; u * 1000 < cbsaCoordList.Count; u++) {
                        IEnumerable<MetropolitanCoreAreaCoordinate> section = cbsaCoordList.Skip(u * 1000).Take(1000);
                        List<MetropolitanCoreAreaCoordinate> coordinates = new List<MetropolitanCoreAreaCoordinate>(section);
                        cbsaCoordRep.AddAll(coordinates);
                    }

                    SetSuccessMessage(resultDict, ref importResult);
                } catch (Exception ex) {
                    SetFailedMessage(resultDict, errorDict, ref importResult, dbfFile.FullName, ex.Message);
                }

                SendMessage(true, cbsaCoordList.Count, "BG'S Area coordinates be imported ", shpFile.Name);
            }
            SendResultMessage();
            WriteErrorLog();
        }
    }
}
