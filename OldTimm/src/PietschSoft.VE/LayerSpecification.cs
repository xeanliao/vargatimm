// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System.ComponentModel;
using PietschSoft.VE.Converters;

namespace PietschSoft.VE
{
    /// <summary>
    /// Contains the information about a map layer. Map layers can be created from GeoRSS XML files, custom map tiles, or from any public Live Maps (http://maps.live.com) collection.
    /// </summary>
    [TypeConverter(typeof(GenericConverter))]
    public class LayerSpecification
    {
        public LayerSpecification() { }

        private LayerType _Type = LayerType.GeoRSS;
        /// <summary>
        /// A LayerType enumeration that specifies the source of the layer data. Default is LayerType.GeoRSS
        /// </summary>
        public LayerType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private string _IconUrl;
        /// <summary>
        /// The location of the image to use for the pushpin icons on the layer.
        /// </summary>
        public string IconUrl
        {
            get { return _IconUrl; }
            set { _IconUrl = value; }
        }

        private string _Id;
        /// <summary>
        /// A string that represents a unique ID for the layer.
        /// </summary>
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _LayerSource;
        /// <summary>
        /// Specifies the location of the layer data.
        /// </summary>
        public string LayerSource
        {
            get { return _LayerSource; }
            set { _LayerSource = value; }
        }

        public HTTPMethod _Method = HTTPMethod.GET;
        /// <summary>
        /// Specifies the HTTPMethod to use.
        /// </summary>
        public HTTPMethod Method
        {
            get { return _Method; }
            set { _Method = value; }
        }

        private string _Callback;
        /// <summary>
        /// Specifies the JavaScript function to call when the VELayer object is added to the map.
        /// </summary>
        public string Callback
        {
            get { return _Callback; }
            set { _Callback = value; }
        }
        
        private double _Opacity = 1.0;
        /// <summary>
        /// Specifies the opacity of a tile layer.
        /// This property only affects the opacity of tile layers (LayerType enumeration set to TileSource).
        /// Use a value between 0.0 (transparent) to 1.0 (opaque) to define the opacity of the layer on the map.
        /// </summary>
        public double Opacity
        {
            get { return _Opacity; }
            set { _Opacity = value; }
        }
        
        private int _ZIndex = 0;
        /// <summary>
        /// Specifies the z-index of a tile layer on the map.
        /// This property only affects the z-index of tile layers (LayerType enumeration set to TileSource).
        /// </summary>
        public int ZIndex
        {
            get { return _ZIndex; }
            set { _ZIndex = value; }
        }

    }
}
