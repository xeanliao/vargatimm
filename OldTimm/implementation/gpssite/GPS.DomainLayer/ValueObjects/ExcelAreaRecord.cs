using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using FileHelpers.DataLink;

namespace GPS.DataLayer.ValueObjects
{
    /// <summary>
    /// A <see cref="ExcelAreaRecord"/> represents an area record in an MS Excel file.
    /// This class is used in combination with the FileHelpers library.
    /// </summary>
    [DelimitedRecord("|")]
    public class ExcelAreaRecord : CsvAreaRecord {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ExcelAreaRecord() { }

        /// <summary>
        /// Construct an instance with specified code, total, and penetration.
        /// </summary>
        /// <param name="code">The Code value of the record.</param>
        /// <param name="total">The Total value of the record.</param>
        /// <param name="penetration">The Penetration value of the record.</param>
        public ExcelAreaRecord(string code, string total, string penetration) :
            base(code, total, penetration) { }
    }
}
