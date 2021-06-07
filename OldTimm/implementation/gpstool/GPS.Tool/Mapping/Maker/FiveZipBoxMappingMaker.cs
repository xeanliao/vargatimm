using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;

namespace GPS.Tool.Mapping.Maker
{
    class FiveZipBoxMappingMaker : MappingMaker
    {
        protected override void MakeMapping()
        {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(
                MappingTables.FiveZipBoxMappings);
            List<FiveZipArea> fiveZipAreas = dataContext.FiveZipAreas
                .Where(b => b.Id >= areaId).ToList();
            SendMessage(true, fiveZipAreas.Count(), 0, "");
            int i = 0;
            foreach (FiveZipArea fiveZipArea in fiveZipAreas)
            {
                if (_stopEnabled)
                {
                    MappingRecorder.SetCurrentId(
                        MappingTables.FiveZipBoxMappings, fiveZipArea.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(fiveZipArea, 10, 15);
                foreach (int id in ids)
                {
                    i++;
                    InsertItem(id, fiveZipArea);
                    SendMessage(id, fiveZipArea);
                    if (i % 1000 == 0)
                        dataContext.SubmitChanges();
                }
            }
            dataContext.SubmitChanges();
            SendStatus(false, !_stopEnabled);
        }

        private void InsertItem(int boxId, FiveZipArea fiveZipArea)
        {
            FiveZipBoxMapping mapping = new FiveZipBoxMapping();
            mapping.FiveZipAreaId = fiveZipArea.Id;
            mapping.BoxId = boxId;
            dataContext.FiveZipBoxMappings.InsertOnSubmit(mapping);
        }

        private void SendMessage(int boxId, FiveZipArea fiveZipArea)
        {
            SendMessage(true, -1, fiveZipArea.Id, string.Format("Box:{0},Zip:{1},Zip'Id:{2}", boxId, fiveZipArea.Code, fiveZipArea.Id));
        }
    }
}
