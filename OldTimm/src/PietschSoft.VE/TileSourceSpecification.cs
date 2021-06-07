// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using PietschSoft.VE.Converters;

namespace PietschSoft.VE
{
    /// <summary>
    /// Contains information about a custom map tile source.
    /// </summary>
    [TypeConverter(typeof(GenericConverter))]
    public class TileSourceSpecification
    {
        public TileSourceSpecification() { }

        private List<LatLongRectangle> _Bounds;
        /// <summary>
        /// Specifies the geographic bounds over which the layer is displayed.
        /// </summary>
        public List<LatLongRectangle> Bounds
        {
            get { return _Bounds; }
            set { _Bounds = value; }
        }

        private string _Id;
        /// <summary>
        /// Specifies a unique identifier for the layer. Each layer on a map must have a unique Identifier.
        /// </summary>
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private int _MaxZoom;
        /// <summary>
        /// Specifies the maximum zoom level at which to display the custom tile source.
        /// </summary>
        public int MaxZoom
        {
            get { return _MaxZoom; }
            set { _MaxZoom = value; }
        }

        private int _MinZoom;
        /// <summary>
        /// Specifies the minimum zoom level at which to display the custom tile source.
        /// </summary>
        public int MinZoom
        {
            get { return _MinZoom; }
            set { _MinZoom = value; }
        }

        private int _NumServers = 1;
        /// <summary>
        /// Specifies the number of servers on which the tiles are hosted.
        /// </summary>
        public int NumServers
        {
            get { return _NumServers; }
            set { _NumServers = value; }
        }

        private string _TileSource;
        /// <summary>
        /// Specifies the location of the tiles.
        /// </summary>
        public string TileSource
        {
            get { return _TileSource; }
            set { _TileSource = value; }
        }

        private string _GetTilePath;
        /// <summary>
        /// When viewing a map in 2D mode, this property specifies the function that determines the correct file names for your tiles.
        /// </summary>
        public string GetTilePath
        {
            get { return _GetTilePath; }
            set { _GetTilePath = value; }
        }

    }
}
