using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using GPS.Tool.Data;

namespace GPS.Tool.Import
{
    public struct StructThreadParamter
    {
        public Dictionary<FileInfo, FileInfo> fileDictionary;
        public DataTable dt;
        public ShpReader shpReader;

        public StructThreadParamter(Dictionary<FileInfo, FileInfo> pFileDictionary,
            DataTable pDt,
            ShpReader pShpReader)
        {
            this.fileDictionary = pFileDictionary;
            this.dt = pDt;
            this.shpReader = pShpReader;
        }
    }
}
