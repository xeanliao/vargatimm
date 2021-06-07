using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.RepositoryInterfaces.BulkRepositories
{
    public interface IBulkRepository
    {
        NHibernate.IStatelessSession Session { set; }
    }
}
