using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GPS.Tool.Data;
using GPS.Tool.Import.Reader;

namespace GPS.Tool.Import
{
    class ZIPController : ImportAreaController
    {
        protected override void Importting(object data)
        {
            Dictionary<FileInfo, FileInfo> fileDictionary =
                (Dictionary<FileInfo, FileInfo>)data;
            SendStartMessage("Start to Import ZIP:");
            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                ZIPReader reader = new ZIPReader(entry.Value.FullName, entry.Key.FullName);
                reader.Open();
                int length = reader.Length;
                int num = 5;
                SendStartMessage("Current Importing: ZIP'S Area Total: " +
                    length + "File Name:" + entry.Value.FullName);
                for (int i = 0; i * num < length; i++)
                {
                    List<PremiumZip> zips = reader.Read(num);
                    PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
                    dataContext.PremiumZips.InsertAllOnSubmit(zips);
                    dataContext.SubmitChanges();
                    SendStartMessage(string.Format("Current Importing: {0} of {1} File: {2} ZIP Areas.", (i + 1) * num, length, entry.Value.Name));
                    SetSuccessMessage(resultDict, ref importResult);
                }
                reader.Close();
            }
            SendResultMessage();
        }
    }
}
