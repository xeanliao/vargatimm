using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.Common;

namespace GPS.Tool.Import.Reader
{
    abstract class ImportReader<T>
    {
        DbfReader dbfReader;
        ShpReader shpReader;
        DbDataReader reader;
        int _index;
        public ImportReader(string dbfFilePath, string shpFilePath)
        {
            dbfReader = new DbfReader(dbfFilePath);
            shpReader = new ShpReader(shpFilePath);
        }

        public void Open()
        {
            _index = 0;
            dbfReader.Open();
            shpReader.Open();
            reader = dbfReader.ExecuteReader();
        }

        public List<T> Read()
        {
            return Read(1);
        }

        public int Length
        {
            get
            {
                return shpReader.polygons.Count;
            }
        }

        public List<T> Read(int line)
        {
            List<T> ret = new List<T>();
            int count = _index + line;
            while (_index < count && reader.Read())
            {
                ret.Add(GetInstance(reader, shpReader.Read()));
                _index++;
            }
            return ret;
        }

        public abstract T GetInstance(DbDataReader reader, ShpReader.Polygon polygon);

        public void Close()
        {
            dbfReader.Close();
            shpReader.Close();
        }

    }
}
