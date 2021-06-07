using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GPS.Tool.Import.Reader;
using GPS.Tool.Data;

namespace GPS.Tool.Import
{
    class CBSAImporter : ImportAreaController
    {
        protected override void Importting(object data)
        {
            Dictionary<FileInfo, FileInfo> fileDictionary =
    (Dictionary<FileInfo, FileInfo>)data;
            SendStartMessage("Start to Import CSBA areas:");
            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                CBSAReader reader = new CBSAReader(entry.Value.FullName, entry.Key.FullName);
                reader.Open();
                int length = reader.Length;
                int num = 10;
                SendStartMessage("Current Importing: CBSA'S Area Total: " +
                    length + "File Name:" + entry.Value.FullName);
                for (int i = 0; i * num < length; i++)
                {
                    List<MetropolitanCoreArea> cRoutes = reader.Read(num);
                    AreaDataContext dataContext = new AreaDataContext();
                    dataContext.MetropolitanCoreAreas.InsertAllOnSubmit(cRoutes);
                    dataContext.SubmitChanges();
                    SendStartMessage(string.Format("Current Importing: {0} of {1} File: {2} CBSA areas.", (i + 1) * num, length, entry.Value.Name));
                    SetSuccessMessage(resultDict, ref importResult);
                }
                reader.Close();
            }
            SendResultMessage();
        }
    }
}
