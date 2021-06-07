using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace GPS.DomainLayer.Area.Addresses
{
    /// <summary>
    /// A <see cref="AddressRecord"/> represents an area record in a Csv file.
    /// This class is used in combination with the FileHelpers library.
    /// </summary>
    [DelimitedRecord(",")]
    public class AddressRecord
    {
        /// <summary>
        /// the street line of address record
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        public string StreetLine;
        /// <summary>
        /// the postalcode of address record
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        public string PostalCode;
    }
}
