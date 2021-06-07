﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;

namespace GPS.Web
{
    partial class ThreeZIP
    {
        /// <summary>
        /// the postcode id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// the postcode
        /// </summary>
        public int Postcode { get; set; }

        /// <summary>
        /// the postcode name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the LSAD
        /// </summary>
        public Classifications Classifiction { get; set; }

        /// <summary>
        /// the LSAD description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the Area
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// the Perimeter
        /// </summary>
        public string Perimeter { get; set; }

        /// <summary>
        /// One 3-zip area can corresponds to one or several 5-zip areas.
        /// </summary>
        public List<FiveZIP> fiveZIPs { get; set; }
    }
}
