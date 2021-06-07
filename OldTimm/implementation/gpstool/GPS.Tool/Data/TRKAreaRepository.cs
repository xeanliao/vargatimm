using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class TRKAreaRepository
    {
        AreaDataContext db = new AreaDataContext();

        public int AddAll(List<Tract> trkList)
        {
            db.Tracts.InsertAllOnSubmit(trkList);
            Save();

            return trkList[0].Id;
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
