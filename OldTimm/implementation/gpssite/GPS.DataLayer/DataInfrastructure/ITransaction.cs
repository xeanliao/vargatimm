using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DataLayer.DataInfrastructure
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
