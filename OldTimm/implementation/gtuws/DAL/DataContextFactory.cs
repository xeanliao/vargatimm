using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.DataLayer
{
    public class DataContext
    {
        public static GTUTrackDataContext _dataContext = null;

        public static GTUTrackDataContext CreateDataContext()
        {
            if (_dataContext == null)
                return new GTUTrackDataContext(ConnectionString.GPSTrackingConnectionString);
            else
                return _dataContext;
        }
    }
}
