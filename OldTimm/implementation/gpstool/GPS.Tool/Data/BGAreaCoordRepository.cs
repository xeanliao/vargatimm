using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class BGAreaCoordRepository
    {
        AreaDataContext db = new AreaDataContext();

        public void AddAll(List<BlockGroupCoordinate> bgCoordList)
        {
            db.BlockGroupCoordinates.InsertAllOnSubmit(bgCoordList);
            Save();
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
