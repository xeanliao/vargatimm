using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class FiveZipAreaCoordRepository
    {
        AreaDataContext db = new AreaDataContext();

        public void AddAll(List<FiveZipAreaCoordinate> fiveZipCoordList)
        {
            db.FiveZipAreaCoordinates.InsertAllOnSubmit(fiveZipCoordList);
            Save();
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
