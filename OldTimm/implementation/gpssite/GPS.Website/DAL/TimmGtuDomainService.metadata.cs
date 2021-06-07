
namespace GPS.Website.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies gtuMetadata as the class
    // that carries additional metadata for the gtu class.
    [MetadataTypeAttribute(typeof(gtu.gtuMetadata))]
    public partial class gtu
    {

        // This class allows you to attach custom attributes to properties
        // of the gtu class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class gtuMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private gtuMetadata()
            {
            }

            public gtubag gtubag { get; set; }

            public Nullable<int> GTUBagId { get; set; }

            public int Id { get; set; }

            public bool IsEnabled { get; set; }

            public string Model { get; set; }

            public string UniqueID { get; set; }

            public Nullable<int> UserId { get; set; }
        }
    }
}
