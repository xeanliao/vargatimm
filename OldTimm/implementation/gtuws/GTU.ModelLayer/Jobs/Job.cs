using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Common;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GTU.ModelLayer.Jobs
{
    [Serializable]
    public class Job
    {
        public String JobID { get; set; }
        public String StarTime { get; set; }
        public String EndTime { get; set; }
        public Coordinate[] ValidArea { get; set; }
        public Coordinate[] InvalidArea { get; set; }
        public Address[] InvalidAddress { get; set; }
        public String Socket { get; set; }

        public Job()
        {
            JobID = String.Empty;
            StarTime = String.Empty;
            EndTime = String.Empty;
        }

        //#region IXmlSerializable Members

        //public System.Xml.Schema.XmlSchema GetSchema()
        //{
        //    throw new NotImplementedException();
        //}

        //public void ReadXml(System.Xml.XmlReader reader)
        //{
        //    //throw new NotImplementedException();
        //    reader.Read();

        //}

        //public void WriteXml(System.Xml.XmlWriter writer)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion
    }
}
