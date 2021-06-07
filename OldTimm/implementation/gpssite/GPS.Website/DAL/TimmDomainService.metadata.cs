
namespace GPS.Website.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies distributionmapMetadata as the class
    // that carries additional metadata for the distributionmap class.
    [MetadataTypeAttribute(typeof(distributionmap.distributionmapMetadata))]
    public partial class distributionmap
    {

        // This class allows you to attach custom attributes to properties
        // of the distributionmap class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class distributionmapMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private distributionmapMetadata()
            {
            }

            public int ColorB { get; set; }

            public int ColorG { get; set; }

            public int ColorR { get; set; }

            public string ColorString { get; set; }

            public int CountAdjustment { get; set; }

            public int Id { get; set; }

            public string Name { get; set; }

            public int Penetration { get; set; }

            public double Percentage { get; set; }

            public int SubMapId { get; set; }

            public EntityCollection<task> tasks { get; set; }

            public int Total { get; set; }

            public int TotalAdjustment { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies taskMetadata as the class
    // that carries additional metadata for the task class.
    [MetadataTypeAttribute(typeof(task.taskMetadata))]
    public partial class task
    {

        // This class allows you to attach custom attributes to properties
        // of the task class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class taskMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private taskMetadata()
            {
            }

            public Nullable<int> AuditorId { get; set; }

            public Nullable<DateTime> Date { get; set; }

            public distributionmap distributionmap { get; set; }

            public int DmId { get; set; }

            public string Email { get; set; }

            public int Id { get; set; }

            public string Name { get; set; }

            public Nullable<int> Status { get; set; }

            public string Telephone { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies userMetadata as the class
    // that carries additional metadata for the user class.
    [MetadataTypeAttribute(typeof(user.userMetadata))]
    public partial class user
    {

        // This class allows you to attach custom attributes to properties
        // of the user class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class userMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private userMetadata()
            {
            }

            public bool Enabled { get; set; }

            public string FullName { get; set; }

            public int Id { get; set; }

            public string Password { get; set; }

            public int Role { get; set; }

            public string UserCode { get; set; }

            public string UserName { get; set; }
        }
    }
}
