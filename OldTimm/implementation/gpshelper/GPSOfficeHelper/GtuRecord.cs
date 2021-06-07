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
    public class GtuRecord
    {
        /// <summary>
        /// The Unique Id of the GTU.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string UniqueId;
        /// <summary>
        /// The model of the GTU.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string Model;
        /// <summary>
        /// The status of the GTU.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("1")]
        [DataMember]
        public string Enabled;
        /// <summary>
        /// The user id that will use this GTU.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        [DataMember]
        public string UserId;
    }
}
