using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GPS.Tool.Data;
using GPS.Tool.Import.Reader;

namespace GPS.Tool.Import
{
    class CRouteController : ImportAreaController
    {
        protected override void Importting(object data)
        {
            Dictionary<FileInfo, FileInfo> fileDictionary =
                (Dictionary<FileInfo, FileInfo>)data;
            SendStartMessage("Start to Import CRoutes:");
            foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            {
                CRouteReader reader = new CRouteReader(entry.Value.FullName, entry.Key.FullName);
                reader.Open();
                int length = reader.Length;
                int num = 10;
                SendStartMessage("Current Importing: PreminumCRoute'S Area Total: " +
                    length + " File Name:" + entry.Value.FullName);
                for (int i = 0; i * num < length; i++)
                {
                    try
                    {
                        List<PremiumCRoute> cRoutes = reader.Read(num);
                        PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
                        dataContext.PremiumCRoutes.InsertAllOnSubmit(cRoutes);
                        dataContext.SubmitChanges();
                        SendStartMessage(string.Format("Current Importing: {0} of {1} File: {2} CRoute areas.", (i + 1) * num, length, entry.Value.Name));
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
