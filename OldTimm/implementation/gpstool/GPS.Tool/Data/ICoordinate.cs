using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public interface ICoordinate
    {
        double Latitude { get; }
        double Longitude { get; }
    }
    public interface IShapeCoordinate : ICoordinate
    {
        int ShapeId { get; }
    }
    public interface ICoordinateArea
    {
        double Latitude { get; }
        double Longitude { get; }
        double MinLongitude { get; set; }
        double MaxLongitude { get; set; }
        double MinLatitude { get; }
        double MaxLatitude { get; }
    }
}
