using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker {
    class UnifiedSchoolBoxMappingMaker : MappingMaker {
        List<UnifiedSchoolAreaBoxMapping> mappingList = new List<UnifiedSchoolAreaBoxMapping>();

        protected override void MakeMapping() {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(MappingTables.UnifiedSchoolBoxMapping);

            AreaDataContext dataContext = new AreaDataContext();

            UnifiedSchoolAreaRepository entityRep = new UnifiedSchoolAreaRepository();
            entityRep.DataContext = dataContext;

            UnifiedSchoolAreaBoxMappingRepository boxRep = new UnifiedSchoolAreaBoxMappingRepository();
            boxRep.DataContext = dataContext;

            List<UnifiedSchoolArea> entiryList = entityRep.GetAll()
                .Where(b => b.Id >= areaId).ToList();

            SendMessage(true, entiryList.Count(), 0, "");
            
            int i = 1;

            foreach (UnifiedSchoolArea entity in entiryList) {
                if (_stopEnabled) {
                    MappingRecorder.SetCurrentId(MappingTables.UnifiedSchoolBoxMapping, entity.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(entity, 25, 40);
                foreach (int id in ids) {
                    AppendEitity(id, entity);
                    i++;
                    SendMessage(id, entity);
                    //if (i % 1000 == 0) {
                    //    boxRep.Add(mappingList);
                    //    mappingList = new List<UnifiedSchoolAreaBoxMapping>();
                    //}
                }
            }

            for (Int32 index = 0; index * 1000 < mappingList.Count; index++) {
                IEnumerable<UnifiedSchoolAreaBoxMapping> section = mappingList.Skip(index * 1000).Take(1000);
                List<UnifiedSchoolAreaBoxMapping> mappings = new List<UnifiedSchoolAreaBoxMapping>(section);
                boxRep.Add(mappings);
            }

            SendStatus(false, !_stopEnabled);
        }

        private void AppendEitity(int boxId, UnifiedSchoolArea entity) {
            UnifiedSchoolAreaBoxMapping mapping =
                new UnifiedSchoolAreaBoxMapping();
            mapping.BoxId = boxId;
            mapping.UnifiedSchoolAreaId = entity.Id;
            mappingList.Add(mapping);
        }

        private void SendMessage(int boxId, UnifiedSchoolArea entity) {
            SendMessage(true, -1, entity.Id,
                string.Format("Box:{0},School-Unified:{1},State:{2}",
                boxId,
                entity.Id,
                entity.StateCode));
        }
    }
}
