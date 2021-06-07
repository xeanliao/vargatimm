using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class DistributionMapRepository : RepositoryBase, GPS.DataLayer.IDistributionMapRepository
    {
        public DistributionMapRepository() { }

        public DistributionMapRepository(ISession session) : base(session) { }

        public DistributionMap GetEntity(int id)
        {
            return InternalSession.Get<DistributionMap>(id);
        }

        public IEnumerable<DistributionMap> GetDistributionMaps(int submapId)
        {
            return InternalSession.Linq<DistributionMap>().Where(d => d.SubMapId == submapId);
        }

        public void UpdateDistributionMap(DistributionMap dm)
        {
            base.Update(dm);
        }

        public void AddDistributionMap(DistributionMap dm)
        {
            base.Insert(dm);
        }
             
        public void AddDistributionMaps(IList<DistributionMap> dms)
        {
            IEnumerable<DistributionMap> dmlist = this.GetDistributionMaps(dms[0].SubMapId);
            foreach (DistributionMap dm in dmlist)
            {
                base.Delete(dm);
            }

            if (null != dms)
            {
                foreach (DistributionMap d in dms)
                {
                    InternalSession.Save(d);
                }
                InternalSession.Flush();
            }
        }

        #region IDistributionMapRepository Members

        void IDistributionMapRepository.DeleteDistributionMap(DistributionMap distributionMap)
        {
            base.Delete(distributionMap);
        }

        #endregion
    }
}
