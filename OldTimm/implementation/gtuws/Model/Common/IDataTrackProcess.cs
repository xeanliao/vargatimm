using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Device.Track;

namespace GTU.ModelLayer.Common
{
    public interface IDataTrackProcess
    {
        bool Process(TrackItem trackItem);
    }

    //public interface IDataTrackProcess<DataSource>
    //{
    //    bool Process(ref DataSource dataSource);
    //}
}
