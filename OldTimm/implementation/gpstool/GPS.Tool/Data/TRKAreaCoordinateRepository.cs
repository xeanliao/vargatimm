using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class TRKAreaCoordinateRepository
    {
        AreaDataContext db = new AreaDataContext();

        public void AddAll(List<TractCoordinate> trkCoordList)
        {
            db.TractCoordinates.InsertAllOnSubmit(trkCoordList);
            Save();
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
