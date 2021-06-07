using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FileHelpers;
using GPS.Utilities.OfficeHelpers;

namespace GPS.Utilities.IO
{
    public class FileOperator
    {
        public AreaRecord[] GetAreaRecords(string fileName)
        {
            ExcelHelperClient client = new ExcelHelperClient();
           
            return client.ReadAreaRecord(fileName);
        }

        public T[] ReadFile<T>(string fileName)
        {
            if (GetFileType(fileName) == FileTypes.Excel)
            {
                //return ReadExcelFile<T>(fileName);
                return null;
            }
            else
            {
                return ReadCsvFile<T>(fileName) as T[];
            }
        }

        //private object[] ReadExcelFile<T>(string fileName)
        //{
        //    switch (typeof(T))
        //    { 

        //    }
        //    //ExcelHelperClient client = new ExcelHelperClient();
        //    //return client.ReadRecords(fileName);
        //    return null;
        //}

        private object[] ReadCsvFile<T>(string fileName)
        {
            try
            {
                FileHelperEngine engine = new FileHelperEngine(typeof(T));
                return engine.ReadFile(fileName);
            }
            catch
            {
                return null;
            }
        }
        public bool WriteFile<T>(string fileName, object[] records)
        {
            if (GetFileType(fileName) != FileTypes.Excel)
            {
                return WriteCsvFile<T>(fileName, records);
            }
            return false;
        }
        public bool WriteFile<T>(string fileName, string templateFile, object[] records)
        {
            if (GetFileType(fileName) == FileTypes.Excel)
            {
                return WriteExcelFile<T>(fileName, templateFile, records);
            }
            return false;
        }

        private bool WriteExcelFile<T>(string fileName, string templateFile, object[] records)
        {
            AreaRecord[] rds = new AreaRecord[records.Length];
            for (int i = 0; i < records.Length; i++)
            {
                rds[i] = records[i] as AreaRecord;
            }

            ExcelHelperClient client = new ExcelHelperClient();
            return client.WriteAreaRecord(fileName, templateFile, rds);
        }

        private bool WriteCsvFile<T>(string fileName, object[] records)
        {
            try
            {
                FileHelperEngine engine = new FileHelperEngine(typeof(T));
                engine.WriteFile(fileName, records);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public FileTypes GetFileType(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (string.Equals(ext, ".xls"))
            {
                return FileTypes.Excel;
            }
            else
            {
                return FileTypes.Csv;
            }
        }

        public enum FileTypes
        {
            Excel,
            Csv
        }
    }
}
