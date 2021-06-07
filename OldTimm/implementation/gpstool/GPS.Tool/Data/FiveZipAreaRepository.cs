using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class FiveZipAreaRepository
    {
        AreaDataContext db = new AreaDataContext();

        public int AddAll(List<FiveZipArea> fiveZipList)
        {
            db.FiveZipAreas.InsertAllOnSubmit(fiveZipList);
            Save();
            return fiveZipList[0].Id;
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
