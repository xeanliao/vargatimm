using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;

namespace GPS.Tool.Mapping.Maker
{
    class BlockGroupBoxMappingMaker : MappingMaker
    {
        protected override void MakeMapping()
        {
            SendStatus(true,false);
            int areaId = MappingRecorder.GetCurrentId(
                MappingTables.BlockGroupBoxMappings);
            //Table<BlockGroup> blockGroups = dataContext.BlockGroups;
            List<BlockGroup> blockGroups = dataContext.BlockGroups
                .Where(b=>b.Id>=areaId).ToList();
            SendMessage(true, blockGroups.Count(), 0, "");
            int i = 0;
            foreach (BlockGroup blockGroup in blockGroups)
            {
                if (_stopEnabled)
                {
                    MappingRecorder.SetCurrentId(
                        MappingTables.BlockGroupBoxMappings, blockGroup.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(blockGroup, 3, 4);
                foreach (int id in ids)
                {
                    i++;
                    InsertItem(id, blockGroup);
                    SendMessage(id, blockGroup);
                    if (i % 1000 == 0)
                        dataContext.SubmitChanges();                   
                }
            }
            dataContext.SubmitChanges();
            SendStatus(false, !_stopEnabled);
        }

        private void InsertItem(int boxId, BlockGroup blockGroup)
        {
            BlockGroupBoxMapping mapping = new BlockGroupBoxMapping();
            mapping.BlockGroupId = blockGroup.Id;
            mapping.BoxId = boxId;
            dataContext.BlockGroupBoxMappings.InsertOnSubmit(mapping);
        }

        private void SendMessage(int boxId, BlockGroup blockGroup)
        {
            SendMessage(true, -1, blockGroup.Id, string.Format("Box:{0},trk:{1},BG:{2},BG'Id:{3}", boxId, blockGroup.TractCode, blockGroup.Code, blockGroup.Id));
        }
    }
}
