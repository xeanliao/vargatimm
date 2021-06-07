using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using FileHelpers.DataLink;

namespace GPS.DataLayer.ValueObjects
{
    /// <summary>
    /// A <see cref="CsvAreaRecord"/> represents an area record in a Csv file.
    /// This class is used in combination with the FileHelpers library.
    /// </summary>
    [DelimitedRecord(",")]
    public class CsvAreaRecord {
        /// <summary>
        /// The Code value of the area record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        public string Code;
        /// <summary>
        /// The Total value of the area record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        public string Total;
        /// <summary>
        /// The Penetration value of the area record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        public string Penetration;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CsvAreaRecord() { }

        /// <summary>
        /// Construct an instance with specified code, total, and penetration.
        /// </summary>
        /// <param name="code">The Code value of the record.</param>
        /// <param name="total">The Total value of the record.</param>
        /// <param name="penetration">The Penetration value of the record.</param>
        public CsvAreaRecord(string code, string total, string penetration) {
            this.Code = code;
            this.Total = total;
            this.Penetration = penetration;
        }
    }
}
