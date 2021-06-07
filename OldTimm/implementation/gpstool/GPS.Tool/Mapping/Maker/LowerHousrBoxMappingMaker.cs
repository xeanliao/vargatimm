using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker {
    class LowerHousrBoxMappingMaker : MappingMaker {
        List<LowerHouseAreaBoxMapping> mappingList = new List<LowerHouseAreaBoxMapping>();

        protected override void MakeMapping() {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(MappingTables.LowerHouseBoxMapping);
            
            AreaDataContext dataContext = new AreaDataContext();
            
            LowerHouseAreaRepository entiryRep = new LowerHouseAreaRepository();
            entiryRep.DataContext = dataContext;
            List<LowerHouseArea> entiryList = entiryRep.GetAll().Where(b => b.Id >= areaId).ToList();

            LowerHouseAreaBoxMappingRepository boxRep = new LowerHouseAreaBoxMappingRepository();
            boxRep.DataContext = dataContext;

            SendMessage(true, entiryList.Count(), 0, "");
            int i = 1;

            foreach (LowerHouseArea entity in entiryList) {
                if (_stopEnabled) {
                    MappingRecorder.SetCurrentId(MappingTables.LowerHouseBoxMapping, entity.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(entity, 25, 40);
                foreach (int id in ids) {
                    AppendEitity(id, entity);
                    i++;
                    SendMessage(id, entity);
                    //if (i % 1000 == 0) {
                    //    boxRep.Add(mappingList);
                    //    mappingList = new List<LowerHouseAreaBoxMapping>();
                    //}
                }
            }

            for (Int32 index = 0; index * 1000 < mappingList.Count; index++) {
                IEnumerable<LowerHouseAreaBoxMapping> section = mappingList.Skip(index * 1000).Take(1000);
                List<LowerHouseAreaBoxMapping> mappings = new List<LowerHouseAreaBoxMapping>(section);
                boxRep.Add(mappings);
            }
            
            SendStatus(false, !_stopEnabled);
        }

        private void AppendEitity(int boxId, LowerHouseArea entity) {
            LowerHouseAreaBoxMapping mapping = new LowerHouseAreaBoxMapping();
            mapping.BoxId = boxId;
            mapping.LowerHouseAreaId = entity.Id;
            mappingList.Add(mapping);
        }

        private void SendMessage(int boxId, LowerHouseArea entity) {
            SendMessage(true, -1, entity.Id,
                string.Format("Box:{0},Lower-House:{1},State:{2}",
                boxId,
                entity.Id,
                entity.StateCode));
        }
    }
}
