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
    public class Task
    {
        [DefaultValue(0)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        [DefaultValue(0)]
        public int Status { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        [DefaultValue(0)]
        public int DistributionMapId { get; set; }
        public virtual DistributionMap DistributionMap { get; set; }
        public List<TaskGtuInfoMapping> TaskGtuInfoMappings { get; set; }
        [DefaultValue(0)]
        public int AuditorId { get; set; }
        public User Auditor { get; set; }
    }
}
