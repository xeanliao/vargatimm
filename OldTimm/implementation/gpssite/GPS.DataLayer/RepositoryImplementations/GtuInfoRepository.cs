using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class GtuInfoRepository : RepositoryBase
    {
        public GtuInfoRepository() { }

        //public GtuInfoRepository(ISession session) : base(session) { }


        public void AddGtuInfo(IEnumerable<Gtuinfo> gtuInfoList)
        {
            if (null != gtuInfoList)
            {
                foreach (Gtuinfo g in gtuInfoList)
                {
                    g.dtReceivedTime = g.dtSendTime;
                    InternalSession.Save(g);
                }
                InternalSession.Flush();
            }
        }

    }
}
