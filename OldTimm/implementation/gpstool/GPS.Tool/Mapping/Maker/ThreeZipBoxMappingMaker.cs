using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;

namespace GPS.Tool.Mapping.Maker
{
    class ThreeZipBoxMappingMaker:MappingMaker
    {

        protected override void MakeMapping()
        {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(
                MappingTables.ThreeZipBoxMappings);
            List<ThreeZipArea> threeZipAreas = dataContext.ThreeZipAreas
                .Where(b => b.Id >= areaId).ToList();
            SendMessage(true, threeZipAreas.Count(), 0, "");
            int i = 0;
            foreach (ThreeZipArea threeZipArea in threeZipAreas)
            {
                if (_stopEnabled)
                {
                    MappingRecorder.SetCurrentId(
                        MappingTables.ThreeZipBoxMappings, threeZipArea.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(threeZipArea, 25, 40);
                foreach (int id in ids)
                {
                    i++;
                    InsertItem(id, threeZipArea);
                    SendMessage(id, threeZipArea);
                    if (i % 1000 == 0)
                        dataContext.SubmitChanges();
                }
            }
            dataContext.SubmitChanges();
            SendStatus(false, !_stopEnabled);
        }

        private void InsertItem(int boxId, ThreeZipArea threeZipArea)
        {
            ThreeZipBoxMapping mapping = new ThreeZipBoxMapping();
            mapping.ThreeZipAreaId = threeZipArea.Id;
            mapping.BoxId = boxId;
            dataContext.ThreeZipBoxMappings.InsertOnSubmit(mapping);
        }

        private void SendMessage(int boxId, ThreeZipArea threeZipArea)
        {
            SendMessage(true, -1, threeZipArea.Id, string.Format("Box:{0},Zip:{1},Zip'Id:{2}", boxId, threeZipArea.Code, threeZipArea.Id));
        }
    }
}
