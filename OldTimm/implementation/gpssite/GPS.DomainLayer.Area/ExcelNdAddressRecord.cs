using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace GPS.DomainLayer.Area
{
    /// <summary>
    /// A <see cref="ExcelNdAddressRecord"/> represents an nd-address record in a excel file.
    /// This class is used in combination with the FileHelpers library.
    /// </summary>
    [DelimitedRecord("|")]
    public class ExcelNdAddressRecord
    {
        /// <summary>
        /// The street address value of the nd-address record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        public string Street;
        /// <summary>
        /// The zip code value of the nd-address record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        public string ZipCode;
        /// <summary>
        /// The geofence value of the nd-address record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue(500)]
        public int Geofence = 500;
        /// <summary>
        /// The description value of the nd-address record.
        /// </summary>
        [FieldOptional]
        [FieldNullValue("")]
        public string Description = "";

        public ExcelNdAddressRecord(string sAddress)
        {
            string[] addressInfo = null;
            if(sAddress.IndexOf("\t") > 0)
                addressInfo = sAddress.Split('\t');
            else if(sAddress.IndexOf("|") > 0)
                addressInfo = sAddress.Split('|');
            else if (sAddress.IndexOf(",") > 0)
                addressInfo = sAddress.Split(',');

            this.Street = addressInfo[0].Trim();
            this.ZipCode = addressInfo[1].Trim();

            if (addressInfo.Length > 2)
                this.Geofence = Convert.ToInt32(addressInfo[2].Trim());

            if (addressInfo.Length > 3)
                this.Description = addressInfo[3].Trim();
        }
    }
}
