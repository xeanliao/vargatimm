﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace TIMM.GPS.Model
{
    public class SubMapRecord
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        [DefaultValue(0)]
        public int Classification { get; set; }
        [DefaultValue(0)]
        public int AreaId { get; set; }
        [DefaultValue(false)]
        public bool Value { get; set; }
        [DefaultValue(0)]
        public int SubMapId { get; set; }
        public virtual SubMap SubMap { get; set; }
    }
}