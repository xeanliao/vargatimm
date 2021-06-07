using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker {
    class CountyAreaBoxMappingMaker : MappingMaker {
        List<CountyAreaBoxMapping> mappingList = new List<CountyAreaBoxMapping>();

        protected override void MakeMapping() {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(MappingTables.CountyAreaBoxMapping);

            AreaDataContext dataContext = new AreaDataContext();

            CountyAreaRepository entiryRep = new CountyAreaRepository();
            entiryRep.DataContext = dataContext;
            List<CountyArea> entiryList = entiryRep.GetAll().Where(b => b.Id >= areaId).ToList();

            CountyAreaBoxMappingRepository boxRep = new CountyAreaBoxMappingRepository();
            boxRep.DataContext = dataContext;

            SendMessage(true, entiryList.Count(), 0, "");
            int i = 1;                        
            
            foreach (CountyArea entity in entiryList) {
                if (_stopEnabled) {
                    MappingRecorder.SetCurrentId(MappingTables.CountyAreaBoxMapping, entity.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(entity, 100, 150);
                foreach (int id in ids) {
                    AppendEitity(id, entity);
                    i++;
                    SendMessage(id, entity);
                    //if (i % 1000 == 0) {
                    //    boxRep.Add(mappingList);
                    //    mappingList = new List<CountyAreaBoxMapping>();
                    //}
                }
            }

            for (Int32 index = 0; index * 1000 < mappingList.Count; index++) {
                IEnumerable<CountyAreaBoxMapping> section = mappingList.Skip(index * 1000).Take(1000);
                List<CountyAreaBoxMapping> mappings = new List<CountyAreaBoxMapping>(section);
                boxRep.Add(mappings);
            }

            SendStatus(false, !_stopEnabled);
        }

        private void AppendEitity(int boxId, CountyArea entity) {
            CountyAreaBoxMapping mapping =
                new CountyAreaBoxMapping();
            mapping.BoxId = boxId;
            mapping.CountyAreaId = entity.Id;
            mappingList.Add(mapping);
        }

        private void SendMessage(int boxId, CountyArea entity) {
            SendMessage(true, -1, entity.Id,
                string.Format("Box:{0},County:{1},State:{2}",
                boxId,
                entity.Id,
                entity.StateCode));
        }
    }
}
