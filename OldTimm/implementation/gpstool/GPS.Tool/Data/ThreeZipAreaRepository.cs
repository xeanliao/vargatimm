using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class ThreeZipAreaRepository
    {
        AreaDataContext db = new AreaDataContext();

        public int AddAll(List<ThreeZipArea> threeZipList)
        {
            db.ThreeZipAreas.InsertAllOnSubmit(threeZipList);
            Save();

            return threeZipList[0].Id;
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
