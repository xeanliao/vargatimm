using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;

namespace GPS.DataLayer
{
    public class CustomAreaRepository : RepositoryBase, GPS.DataLayer.ICustomAreaRepository
    {
        public CustomAreaRepository() { }

        public CustomAreaRepository(ISession session) : base(session) { }

        public void InsertCustomArea(CustomArea area)
        {
            base.Insert(area);
        }

        public void DeleteCustomArea(CustomArea area)
        {
            base.Delete(area);
        }

        public CustomArea GetCustomArea(string name)
        {
            const string queryFormat = "select ca from CustomArea ca where ca.Name = :name";

            return InternalSession.CreateQuery(queryFormat).SetString("name", name).UniqueResult<CustomArea>();
        }
        public List<CustomArea> GetCustomAreas(int boxId)
        {
            List<CustomArea> results = new List<CustomArea>();

            const string queryFormat = "select distinct cabm.CustomArea from CustomAreaBoxMapping cabm where cabm.BoxId = :boxId";

            IList<CustomArea> items = InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<CustomArea>();

            results.AddRange(items);

            return results;
        }

        public List<CustomArea> GetCustomAreas()
        {
            List<CustomArea> results = new List<CustomArea>();

            const string queryFormat = "select distinct cabm.CustomArea from CustomAreaBoxMapping cabm ";

            IList<CustomArea> items = InternalSession.CreateQuery(queryFormat).List<CustomArea>();

            results.AddRange(items);

            return results;
        }
    }
}
