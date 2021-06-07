using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class CountyRepository
    {
        AreaDataContext db = new AreaDataContext();

        public IQueryable<CountyArea> GetAll()
        {
            return db.CountyAreas;
        }

        public void AddAll(List<County> countyList)
        {
            db.Counties.InsertAllOnSubmit(countyList);
            Save();
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
