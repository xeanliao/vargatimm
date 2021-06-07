using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FileHelpers.DataLink;
using System.IO;

namespace GPSOfficeHelper
{
    class ExcelHelper : IExcelHelper
    {
        public string TestHello(string text)
        {
            return "WCF Hello: " + text;
        }

        public GtuRecord[] ReadGtuRecord(string fileName)
        {
            return ReadRecords<GtuRecord>(fileName) as GtuRecord[];
        }

        public AreaRecord[] ReadAreaRecords(string fileName)
        {
            return ReadRecords<AreaRecord>(fileName) as AreaRecord[];
        }

        public object[] ReadRecords<T>(string fileName)
        {
            try
            {
                ExcelStorage storage = new ExcelStorage(typeof(T));
                storage.FileName = fileName;
                return storage.ExtractRecords();
            }
            catch (Exception ex)
            {
                StreamWriter sw = File.AppendText("err.txt");
                sw.WriteLine(ex.Message);
                sw.Close();
                return null;
            }
        }

        public bool WriteRecords(string fileName, string templateFile, object[] records)
        {
            try
            {
                if (records.Length > 0)
                {
                    ExcelStorage storage = new ExcelStorage(records[0].GetType());
                    storage.FileName = fileName;
                    storage.TemplateFile = templateFile;
                    storage.InsertRecords(records);
                }
                return true;
            }
            catch (Exception ex)
            {
                StreamWriter sw = File.AppendText("err.txt");
                sw.WriteLine("");
                sw.WriteLine("Write file: " + fileName);
                sw.WriteLine(ex.Message);
                sw.Close();
                return false;
            }
        }
    }
}
