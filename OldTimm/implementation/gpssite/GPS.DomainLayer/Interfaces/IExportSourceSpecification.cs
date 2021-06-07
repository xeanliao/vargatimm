using System;

namespace GPS.DomainLayer.Interfaces
{
    public interface IExportSourceSpecification
    {
        System.Collections.Generic.IEnumerable<Int32> SelectedBlockGroupIds { get; }
        System.Collections.Generic.IEnumerable<Int32> SelectedCRouteIds { get; }
        System.Collections.Generic.IEnumerable<Int32> SelectedFiveZipIds { get; }
        System.Collections.Generic.IEnumerable<Int32> SelectedThreeZipIds { get; }
        System.Collections.Generic.IEnumerable<Int32> SelectedTractIds { get; }
        System.Collections.Generic.IEnumerable<Int32> DeselectedBlockGroupIds { get; }
        System.Collections.Generic.IEnumerable<Int32> DeselectedCRouteIds { get; }
        System.Collections.Generic.IEnumerable<Int32> DeselectedFiveZipIds { get; }
        System.Collections.Generic.IEnumerable<Int32> DeselectedTractIds { get; }
    }
}
