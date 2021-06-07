using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class ThreeZipCoordinateRepository
    {
        AreaDataContext db = new AreaDataContext();

        public void AddAll(List<ThreeZipAreaCoordinate> threeZipCoordList)
        {
            db.ThreeZipAreaCoordinates.InsertAllOnSubmit(threeZipCoordList);
            Save();
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
