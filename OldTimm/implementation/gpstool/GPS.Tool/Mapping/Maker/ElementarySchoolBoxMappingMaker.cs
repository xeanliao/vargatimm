using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker {
    class ElementarySchoolBoxMappingMaker : MappingMaker {
        List<ElementarySchoolAreaBoxMapping> mappingList = new List<ElementarySchoolAreaBoxMapping>();

        protected override void MakeMapping() {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(MappingTables.ElementarySchoolBoxMapping);

            AreaDataContext dataContext = new AreaDataContext();

            ElementarySchoolAreaRepository entiryRep = new ElementarySchoolAreaRepository();
            entiryRep.DataContext = dataContext;
            List<ElementarySchoolArea> entiryList = entiryRep.GetAll().Where(b => b.Id >= areaId).ToList();

            ElementarySchoolAreaBoxMappingRepository boxRep = new ElementarySchoolAreaBoxMappingRepository();
            boxRep.DataContext = dataContext;

            SendMessage(true, entiryList.Count(), 0, "");
            
            int i = 1;
            
            foreach (ElementarySchoolArea entity in entiryList) {
                if (_stopEnabled) {
                    MappingRecorder.SetCurrentId(
                        MappingTables.ElementarySchoolBoxMapping, entity.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(entity, 10, 15);
                foreach (int id in ids) {
                    AppendEitity(id, entity);
                    i++;
                    SendMessage(id, entity);
                    //if (i % 1000 == 0) {
                    //    boxRep.Add(mappingList);
                    //    mappingList = new List<ElementarySchoolAreaBoxMapping>();
                    //}
                }
            }

            for (Int32 index = 0; index * 1000 < mappingList.Count; index++) {
                IEnumerable<ElementarySchoolAreaBoxMapping> section = mappingList.Skip(index * 1000).Take(1000);
                List<ElementarySchoolAreaBoxMapping> mappings = new List<ElementarySchoolAreaBoxMapping>(section);
                boxRep.Add(mappings);
            }
            
            SendStatus(false, !_stopEnabled);
        }

        private void AppendEitity(int boxId, ElementarySchoolArea entity) {
            ElementarySchoolAreaBoxMapping mapping =
                new ElementarySchoolAreaBoxMapping();
            mapping.BoxId = boxId;
            mapping.ElementarySchoolAreaId = entity.Id;
            mappingList.Add(mapping);
        }

        private void SendMessage(int boxId, ElementarySchoolArea entity) {
            SendMessage(true, -1, entity.Id,
                string.Format("Box:{0},School-Elementary:{1},State:{2}",
                boxId,
                entity.Id,
                entity.StateCode));
        }
    }
}
