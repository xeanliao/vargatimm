using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DataLayer.RepositoryInterfaces
{
    public interface IRepository
    {
        NHibernate.ISession Session { set; }
    }
}
