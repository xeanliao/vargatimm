using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.RepositoryInterfaces.BulkRepositories
{
    public interface IBulkRadiusRecordRepository
    {
        void InsertEntityList(IEnumerable<RadiusRecord> records);
        void DeleteRadiusRecords(int radiusId);
    }
}
