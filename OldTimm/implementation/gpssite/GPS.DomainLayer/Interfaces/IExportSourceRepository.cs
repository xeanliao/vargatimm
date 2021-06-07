using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Interfaces
{
    public interface IExportSourceRepository<ShapeType>
    {
        IEnumerable<ShapeType> GetExportSource(IExportSourceSpecification spec);
    }
}
