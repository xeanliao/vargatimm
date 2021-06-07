using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class NdAddress
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        public string Description { get; set; }
        [DefaultValue(0)]
        public int Geofence { get; set; }
        [DefaultValue(0.0)]
        public double Latitude { get; set; }
        [DefaultValue(0.0)]
        public double Longitude { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }

        public NdAddress()
        {
            NdAddressCoordinates = new List<NdAddressCoordinate>();
            Locations = new List<Location>();
        }

        public virtual List<NdAddressCoordinate> NdAddressCoordinates { get; set; }

        public virtual List<Location> Locations { get; set; }
    }
}
