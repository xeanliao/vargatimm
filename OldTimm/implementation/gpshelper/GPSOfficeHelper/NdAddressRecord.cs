using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using System.Runtime.Serialization;

namespace GPSOfficeHelper
{
    [DataContract]
    [DelimitedRecord("|")]
    public class NdAddressRecord
    {
        /// <summary>
        /// The street address value of the nd-address record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string Street;
        /// <summary>
        /// The zip code value of the nd-address record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string ZipCode;
        /// <summary>
        /// The geofence value of the nd-address record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue(500)]
        [DataMember]
        public int Geofence;
        /// <summary>
        /// The description value of the nd-address record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string Description;
    }
}
