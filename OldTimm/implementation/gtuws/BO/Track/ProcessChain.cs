using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Track
{
    public class ProcessChain<DataSource> where DataSource : new()
    {
        //private LinkedList<IDataTrackProcess<DataSource>> _links = new LinkedList<IDataTrackProcess<DataSource>>();

        //public void AddLink(LinkedListNode<IDataTrackProcess<DataSource>> link)
        //{
        //    _links.AddLast(link);
        //}

        //public void Process(ref DataSource dataSource)
        //{
        //    foreach (IDataTrackProcess<DataSource> element in _links)
        //    {
        //        element.Process(ref dataSource);
        //    }
        //}
    }
}
