using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DataLayer
{
    public interface ISubMapRepository
    {
        GPS.DomainLayer.Entities.SubMap GetEntity(int id);
    }
}
