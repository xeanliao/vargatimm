using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Common
{
    public interface IArea
    {
        String Id { get; set; }
        String Name { get; set; }
        //Classifications Classification { get; set; }
        //String State { get; set; }
        //Dictionary<String, String> Attributes { get; set; }
        List<ICoordinate> Locations { get; set; }
        //IGPSColor FillColor { get; set; }
        //IGPSColor LineColor { get; set; }
        //bool IsExportData { get; set; }
        //List<List<String>> Relation { get; set; }
        //Double Latitude { get; set; }
        //Double Longitude { get; set; }
        bool IsEnabled { get; set; }
        String Description { get; set; }
        //IArea DeepClone(bool isDeepCopy);
    }
}
