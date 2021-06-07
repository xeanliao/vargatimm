using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using System.Runtime.Serialization;

namespace GPSOfficeHelper
{
    public class Record { }

    [DelimitedRecord("|")]
    [DataContract]
    public class AreaRecord
    {
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string Code;
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string Total;
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string Penetration;
    }
}
