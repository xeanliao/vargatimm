using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Track;
using GTU.Utilities.Base;
using GTU.DataLayer.Track;

namespace GTU.BusinessLayer.Track
{
    public class TrackItemBo
    {
        public TrackItem TrackItemModel { get; set; }

        public TrackItemBo()
        {
            TrackItemModel = new TrackItem();
        }

        public TrackItemBo(TrackItem trackItemModel)
        {
            TrackItemModel = trackItemModel;
        }

        public void Save()
        {
            TrackItemDal trackItemDal = new TrackItemDal();

            trackItemDal.Save(TrackItemModel);

        }
    }
}
