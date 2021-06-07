using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using System.Data.Linq;
using GPS.DomainLayer.Interfaces;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class CampaignRecordRepository : RepositoryBase, GPS.DataLayer.ICampaignRecordRepository
    {
        public CampaignRecordRepository() { }

        public CampaignRecordRepository(ISession session) : base(session) { }

        public CampaignRecord GetEntity(int id)
        {
            return InternalSession.Get<CampaignRecord>(id);
        }

        public void UpdateEntity(CampaignRecord entity)
        {
            CampaignRecord exists = GetEntity(entity.Id);
            exists.Classification = entity.Classification;
            exists.AreaId = entity.AreaId;
            exists.Value = entity.Value;

            base.Update(exists);
        }

        public void InsertEntity(CampaignRecord entity)
        {
            base.Insert(entity);
        }

        public void InsertEntityList(List<CampaignRecord> entityList)
        {
            foreach (CampaignRecord cr in entityList)
            {
                InternalSession.Save(cr);
            }

            InternalSession.Flush();
        }

        public IQueryable<CampaignRecord> GetEntitiesByCampaign(int campaignId)
        {
            return InternalSession.Linq<CampaignRecord>().Where(c => c.Campaign.Id == campaignId);
        }

        public void DeleteEntityListById(int campaignId)
        {
            IEnumerable<CampaignRecord> entityList = InternalSession.Linq<CampaignRecord>().Where(c => c.Campaign.Id == campaignId);

            if (null != entityList)
            {
                foreach (CampaignRecord cr in entityList)
                {
                    InternalSession.Delete(cr);
                }

                InternalSession.Flush();
            }
        }
    }
}
