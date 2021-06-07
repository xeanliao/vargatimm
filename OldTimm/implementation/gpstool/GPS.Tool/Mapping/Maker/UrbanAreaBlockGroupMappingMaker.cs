using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;

namespace GPS.Tool.Mapping.Maker
{
    class UrbanAreaBlockGroupMappingMaker : MappingMaker
    {
        protected override void MakeMapping()
        {
            Table<UrbanArea> urbanAreas = dataContext.UrbanAreas;
            Table<BlockGroup> blockGroups = dataContext.BlockGroups;
            SendMessage(true, blockGroups.Count(), 0, "");
            int i = 0;
            foreach (BlockGroup blockGroup in blockGroups)
            {
                if (_stopEnabled) break;
                bool exist = false;
                foreach (UrbanArea urbanArea in urbanAreas)
                {
                    if (_stopEnabled) break;
                    if (AreaInPolygon(urbanArea, blockGroup))
                    {
                        InsertItem(urbanArea, blockGroup);
                        i++;
                        if (i % 1000 == 0)
                        {
                            dataContext.SubmitChanges();
                        }
                        exist = true;
                        SendMessage(urbanArea, blockGroup);
                    }
                }
                if (!exist)
                {
                    SendMessage(blockGroup);
                }
            }
            dataContext.SubmitChanges();
        }

        private void InsertItem(UrbanArea urbanArea, BlockGroup blockGroup)
        {
            CountyArea county = dataContext.GetCountyArea(blockGroup);
            Tract tract = dataContext.GetTract(blockGroup);
            UrbanAreaBlockGroupMapping mapping = new UrbanAreaBlockGroupMapping();
            mapping.UrbanAreaId = urbanArea.Id;
            mapping.UrbanCode = urbanArea.Code;
            mapping.CountyId = county.Id;
            mapping.CountyCode = county.Code;
            mapping.BlockGroupId = blockGroup.Id;
            mapping.TractId = tract.Id;
            mapping.TractCode = tract.Code;
            mapping.BlockGroupCode = blockGroup.Code;
            dataContext.UrbanAreaBlockGroupMappings.InsertOnSubmit(mapping);
        }

        private bool AreaInPolygon(UrbanArea urbanArea, BlockGroup blockGroup)
        {
            if (ShapeMethods.BoxInBox(urbanArea, blockGroup))
            {
                List<ICoordinate> masterCoordinates = new List<ICoordinate>();
                foreach (UrbanAreaCoordinate uac in urbanArea.UrbanAreaCoordinates)
                {
                    masterCoordinates.Add(uac);
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
            SendMessage(false, -1, blockGroup.Id, string.Format("Urban:{0},BG:{1},BG'Id:{2}", "Null", blockGroup.Code, blockGroup.Id));
        }
        private void SendMessage(UrbanArea urbanArea, BlockGroup blockGroup)
        {
            SendMessage(true, -1, blockGroup.Id, string.Format("Urban:{0},BG:{1},BG'Id:{2}", urbanArea.Code, blockGroup.Code, blockGroup.Id));
        }
    }
}
