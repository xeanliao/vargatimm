// Copyright (c) 2006-2007 Christopher Pietschmann (http://PietschSoft.com).
// This source is subject to the Microsoft Reference License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/referencelicense.mspx.
// All other rights reserved.

using System;
using System.Data;
using System.Configuration;
using System.Runtime.Serialization;
using System.ComponentModel;
using PietschSoft.VE.Converters;

namespace PietschSoft.VE
{
    /// <summary>
    /// Contains the information about a custom pushpin on the map.
    /// </summary>
    [TypeConverter(typeof(GenericConverter))]
    public class Pushpin
    {
        private string _Id;

        /// <summary>
        /// The ID of the pushpin that will be added to the map. The id value must be unique for each pushpin on a map control.
        /// </summary>
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private LatLong _Location;

        /// <summary>
        /// The location of the pushpin, specified as a VELatLong object.
        /// </summary>
        public LatLong Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        private string _Icon_url;

        /// <summary>
        /// The URL that points to the file you want to use as an icon. Optional.
        /// </summary>
        public string Icon_url
        {
            get { return _Icon_url; }
            set { _Icon_url = value; }
        }

        private string _Title;

        /// <summary>
        /// The string to display in the title field of the enhanced preview. Optional.
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _Details;

        /// <summary>
        /// The string to display in the details field of the enhanced preview. Optional.
        /// </summary>
        public string Details
        {
            get { return _Details; }
            set { _Details = value; }
        }

        private string _IconStyle;

        /// <summary>
        /// A cascading style sheet class name that defines the look of the icon. Optional.
        /// </summary>
        public string IconStyle
        {
            get { return _IconStyle; }
            set { _IconStyle = value; }
        }

        private string _TitleStyle;

        /// <summary>
        /// A cascading style sheet class name that defines the look of the title field of the enhanced preview. Optional.
        /// </summary>
        public string TitleStyle
        {
            get { return _TitleStyle; }
            set { _TitleStyle = value; }
        }

        private string _DetailsStyle;

        /// <summary>
        /// A cascading style sheet class name that defines the look of the description field of the enhanced preview. Optional.
        /// </summary>
        public string DetailsStyle
        {
            get { return _DetailsStyle; }
            set { _DetailsStyle = value; }
        }

        private bool _ShowDetailOnMouseOver = true;

        /// <summary>
        /// Specifies whether the pushpin's enhanced preview displays when the user pauses on the pushpin. Default is True.
        /// </summary>
        public bool ShowDetailOnMouseOver
        {
            get { return _ShowDetailOnMouseOver; }
            set { _ShowDetailOnMouseOver = value; }
        }

        private string _OnMouseOverCallback;

        /// <summary>
        /// Specifies the JavaScript function to call when the user pauses on the pushpin.
        /// </summary>
        public string OnMouseOverCallback
        {
            get { return _OnMouseOverCallback; }
            set { _OnMouseOverCallback = value; }
        }

        /// <summary>
        /// Initializes a new instance of the VEPushpin class.
        /// </summary>
        /// <param name="id">The ID of the pushpin that will be added to the map. The id value must be unique for each pushpin on a map control.</param>
        public Pushpin(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the VEPushpin class.
        /// </summary>
        /// <param name="id">The ID of the pushpin that will be added to the map. The id value must be unique for each pushpin on a map control.</param>
        /// <param name="location">The location of the pushpin, specified as a VELatLong object.</param>
        public Pushpin(string id, LatLong location)
        {
            this.Id = id;
            this.Location = location;
        }

        /// <summary>
        /// Initializes a new instance of the VEPushpin class.
        /// </summary>
        /// <param name="id">The ID of the pushpin that will be added to the map. The id value must be unique for each pushpin on a map control.</param>
        /// <param name="location">The location of the pushpin, specified as a VELatLong object.</param>
        /// <param name="icon_url">The URL that points to the file you want to use as an icon.</param>
        public Pushpin(string id, LatLong location, string icon_url)
        {
            this.Id = id;
            this.Location = location;
            this.Icon_url = icon_url;
        }

        /// <summary>
        /// Initializes a new instance of the VEPushpin class.
        /// </summary>
        /// <param name="id">The ID of the pushpin that will be added to the map. The id value must be unique for each pushpin on a map control.</param>
        /// <param name="location">The location of the pushpin, specified as a VELatLong object.</param>
        /// <param name="icon_url">The URL that points to the file you want to use as an icon.</param>
        /// <param name="title">The string to display in the title field of the enhanced preview.</param>
        public Pushpin(string id, LatLong location, string icon_url, string title)
        {
            this.Id = id;
            this.Location = location;
            this.Icon_url = icon_url;
            this.Title = title;
        }

        /// <summary>
        /// Initializes a new instance of the VEPushpin class.
        /// </summary>
        /// <param name="id">The ID of the pushpin that will be added to the map. The id value must be unique for each pushpin on a map control.</param>
        /// <param name="location">The location of the pushpin, specified as a VELatLong object.</param>
        /// <param name="icon_url">The URL that points to the file you want to use as an icon.</param>
        /// <param name="title">The string to display in the title field of the enhanced preview.</param>
        /// <param name="details">The string to display in the details field of the enhanced preview.</param>
        public Pushpin(string id, LatLong location, string icon_url, string title, string details)
        {
            this.Id = id;
            this.Location = location;
            this.Icon_url = icon_url;
            this.Title = title;
            this.Details = details;
        }

        /// <summary>
        /// Initializes a new instance of the VEPushpin class.
        /// </summary>
        /// <param name="id">The ID of the pushpin that will be added to the map. The id value must be unique for each pushpin on a map control.</param>
        /// <param name="location">The location of the pushpin, specified as a VELatLong object.</param>
        /// <param name="icon_url">The URL that points to the file you want to use as an icon.</param>
        /// <param name="title">The string to display in the title field of the enhanced preview.</param>
        /// <param name="details">The string to display in the details field of the enhanced preview.</param>
        /// <param name="iconStyle">A cascading style sheet class name that defines the look of the icon. </param>
        /// <param name="titleStyle">A cascading style sheet class name that defines the look of the title field of the enhanced preview.</param>
        /// <param name="detailsStyle">A cascading style sheet class name that defines the look of the description field of the enhanced preview.</param>
        public Pushpin(string id, LatLong location, string icon_url, string title, string details, string iconStyle, string titleStyle, string detailsStyle)
        {
            this.Id = id;
            this.Location = location;
            this.Icon_url = icon_url;
            this.Title = title;
            this.Details = details;
            this.IconStyle = iconStyle;
            this.TitleStyle = titleStyle;
            this.DetailsStyle = DetailsStyle;
        }
    }
}