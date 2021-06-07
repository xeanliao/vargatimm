using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker
{
    class MetropolitanCoreAreaBlockGroupMappingMaker : MappingMaker
    {
        protected override void MakeMapping()
        {
            Table<MetropolitanCoreArea> metropolitanCoreAreas = dataContext.MetropolitanCoreAreas;
            Table<BlockGroup> blockGroups = dataContext.BlockGroups;
            SendMessage(true, blockGroups.Count(), 0, "");
            int i = 0;
            foreach (BlockGroup blockGroup in blockGroups)
            {
                if (_stopEnabled) break;
                bool exist = false;
                foreach (MetropolitanCoreArea metropolitanCoreArea in metropolitanCoreAreas)
                {
                    if (_stopEnabled) break;
                    if (AreaInPolygon(metropolitanCoreArea, blockGroup))
                    {
                        InsertItem(metropolitanCoreArea, blockGroup);
                        i++;
                        if (i % 1000 == 0)
                        {
                            dataContext.SubmitChanges();
                        }
                        exist = true;
                        SendMessage(metropolitanCoreArea, blockGroup);
                    }
                }
                if (!exist)
                {
                    SendMessage(blockGroup);
                }
            }
            dataContext.SubmitChanges();
        }

        private void InsertItem(MetropolitanCoreArea metropolitanCoreArea, BlockGroup blockGroup)
        {
            CountyArea county = dataContext.GetCountyArea(blockGroup);
            MetropolitanCoreAreaBlockGroupMapping mapping = new MetropolitanCoreAreaBlockGroupMapping();
            mapping.MetropolitanCoreAreaId = metropolitanCoreArea.Id;
            mapping.CountyId = metropolitanCoreArea.Id;
            mapping.BlockGroupId = blockGroup.Id;
            dataContext.MetropolitanCoreAreaBlockGroupMappings.InsertOnSubmit(mapping);
        }

        private bool AreaInPolygon(MetropolitanCoreArea metropolitanCoreArea, BlockGroup blockGroup)
        {
            if (ShapeMethods.BoxInBox(metropolitanCoreArea, blockGroup))
            {
                List<ICoordinate> masterCoordinates = new List<ICoordinate>();
                foreach (MetropolitanCoreAreaCoordinate fzc in metropolitanCoreArea.MetropolitanCoreAreaCoordinates)
                {
                    masterCoordinates.Add(fzc);
                }
                List<ICoordinate> innerCoordinates = new List<ICoordinate>();
                foreach (BlockGroupCoordinate bgc in blockGroup.BlockGroupCoordinates)
                {
                    innerCoordinates.Add(bgc);
                }
                return ShapeMethods.PolygonInPolygon(masterCoordinates, innerCoordinates);
            }
            else
            {
                return false;
            }
        }
        private void SendMessage(BlockGroup blockGroup)
        {
            SendMessage(false, -1, blockGroup.Id, string.Format("CBSA:{0},BG:{1},BG'Id:{2}", "Null", blockGroup.Code, blockGroup.Id));
        }
        private void SendMessage(MetropolitanCoreArea metropolitanCoreArea, BlockGroup blockGroup)
        {
            SendMessage(true, -1, blockGroup.Id, string.Format("CBSA:{0},BG:{1},BG'Id:{2}", metropolitanCoreArea.Code, blockGroup.Code, blockGroup.Id));
        }
    }
}
