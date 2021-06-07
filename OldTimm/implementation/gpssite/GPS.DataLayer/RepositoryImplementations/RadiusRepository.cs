using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class RadiusRepository : RepositoryBase, IRadiusRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RadiusRepository() { }

        public RadiusRepository(ISession session) : base(session) { }

        public void InsertEntityList(List<Radiuse> entityList)
        {
            foreach (Radiuse r in entityList)
            {
                InternalSession.Save(r);
            }
            InternalSession.Flush();
        }

        public void DeleteByCampaign(int campaignId)
        {
            const string queryFormat = "select distinct r from Address a join a.Radiuses r where a.Campaign.Id = :campaignId";

            IList<Radiuse> items = InternalSession.CreateQuery(queryFormat).SetInt32("campaignId", campaignId).List<Radiuse>();

            if (null != items)
            {
                foreach (Radiuse r in items)
                {
                    InternalSession.Delete(r);
                }
                InternalSession.Flush();
            }
        }

        public Radiuse GetEntity(int id)
        {
            return InternalSession.Get<Radiuse>(id);
        }

        public void Update(Radiuse radiuse)
        {
            base.Update(radiuse);
        }

        public void Insert(Radiuse radiuse)
        {
            base.Insert(radiuse);
        }
    }
}
