using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public class BGAreaRepository
    {
        AreaDataContext db = new AreaDataContext();

        public int AddAll(List<BlockGroup> bgList)
        {
            db.BlockGroups.InsertAllOnSubmit(bgList);
            Save();

            return bgList[0].Id;
        }

        private void Save()
        {
            db.SubmitChanges();
        }
    }
}
