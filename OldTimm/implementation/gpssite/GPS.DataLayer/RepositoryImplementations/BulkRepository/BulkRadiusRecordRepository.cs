using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;

namespace GPS.DataLayer.RepositoryImplementations.BulkRepository
{
    public class BulkRadiusRecordRepository : BulkRepositoryBase, IBulkRadiusRecordRepository
    {
        #region IRadiusRecordRepository Members

        public void InsertEntityList(IEnumerable<RadiusRecord> records)
        {

            foreach (RadiusRecord record in records)
            {
                InternalSession.Insert(record);
            }
        }

        public void DeleteRadiusRecords(int radiusId)
        {
            //var target = InternalSession.Get<Radiuse>(radiusId);
            //var recordIds = target.RadiusRecords.Select(i => i.Id).ToArray();
            //InternalSession.CreateQuery("delete from RadiusRecord rr where rr.Id in :radiusId")
            //    .NamedParameters(radi
            //.ExecuteUpdate();

            var targets = InternalSession.CreateQuery("delete from RadiusRecord a where a.Radiuse.Id = :radiusId")
                .SetInt32("radiusId", radiusId)
                .ExecuteUpdate();
        }

        #endregion
    }
}
