using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GPS.Tool.Data;
using GPS.Tool.Import.Reader;

namespace GPS.Tool.Import
{
    class FiveZipImporter : ImportAreaController
    {
        protected override void Importting(object data)
        {
            Dictionary<FileInfo, FileInfo> fileDictionary =
                (Dictionary<FileInfo, FileInfo>)data;
            SendStartMessage("Start to Import ZIP:");
            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                FiveZipReader reader = new FiveZipReader(entry.Value.FullName, entry.Key.FullName);
                reader.Open();
                int length = reader.Length;
                int num = 5;
                SendStartMessage("Current Importing:Five ZIP'S Area Total: " +
                    length + "File Name:" + entry.Value.FullName);
                for (int i = 0; i * num < length; i++)
                {
                    try
                    {
                        List<FiveZipArea> zips = reader.Read(num);
                        AreaDataContext dataContext = new AreaDataContext();
                        dataContext.FiveZipAreas.InsertAllOnSubmit(zips);
                        dataContext.SubmitChanges();
                        SendStartMessage(string.Format("Current Importing: {0} of {1} File: {2} Five ZIP Areas.", (i + 1) * num, length, entry.Value.Name));
                        SetSuccessMessage(resultDict, ref importResult);
                    }
                    catch
                    {
                    }
                }
                reader.Close();
            }
            SendResultMessage();
        }
    }
}
