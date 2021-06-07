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
    public class TaskGtuInfoMapping
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        [DefaultValue(0)]
        public int TaskId { get; set; }
        [DefaultValue(0)]
        public int GTUId { get; set; }
        public string UserColor { get; set; }

        public virtual Task DistributionMap { get; set; }
        public List<GtuInfo> GtuInfos { get; set; }
    }
}
