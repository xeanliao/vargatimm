using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;

namespace GPS.Tool.Mapping.Maker
{
    class TractBoxMappingMaker:MappingMaker
    {
        protected override void MakeMapping()
        {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(
                MappingTables.TractBoxMappings);
            List<Tract> tracts = dataContext.Tracts
                .Where(b => b.Id >= areaId).ToList();
            SendMessage(true, tracts.Count(), 0, "");
            int i = 0;
            foreach (Tract tract in tracts)
            {
                if (_stopEnabled)
                {
                    MappingRecorder.SetCurrentId(
                        MappingTables.TractBoxMappings, tract.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(tract, 5, 7);
                foreach (int id in ids)
                {
                    i++;
                    InsertItem(id, tract);
                    SendMessage(id, tract);
                    if (i % 1000 == 0)
                        dataContext.SubmitChanges();
                }
            }
            dataContext.SubmitChanges();
            SendStatus(false, !_stopEnabled);
        }

        private void InsertItem(int boxId, Tract tract)
        {
            TractBoxMapping mapping = new TractBoxMapping();
            mapping.TractId = tract.Id;
            mapping.BoxId = boxId;
            dataContext.TractBoxMappings.InsertOnSubmit(mapping);
        }

        private void SendMessage(int boxId, Tract tract)
        {
            SendMessage(true, -1, tract.Id, string.Format("Box:{0},trk:{1},trk'Id:{2}", boxId, tract.Code, tract.Id));
        }
    }
}
