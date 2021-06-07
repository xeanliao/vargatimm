using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker {
    /// <summary>
    /// Secondary School Area Box Mapping Maker
    /// </summary>
    class SecondarySchoolBoxMappingMaker : MappingMaker {
        List<SecondarySchoolAreaBoxMapping> mappingList = new List<SecondarySchoolAreaBoxMapping>();

        protected override void MakeMapping() {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(MappingTables.SecondarySchoolBoxMapping);
            
            AreaDataContext dataContext = new AreaDataContext();

            SecondarySchoolAreaRepository entiryRep = new SecondarySchoolAreaRepository();
            entiryRep.DataContext = dataContext;

            SecondarySchoolAreaBoxMappingRepository boxRep = new SecondarySchoolAreaBoxMappingRepository();
            boxRep.DataContext = dataContext;

            List<SecondarySchoolArea> entiryList = entiryRep.GetAll().Where(b => b.Id >= areaId).ToList();

            SendMessage(true, entiryList.Count(), 0, "");
            
            int i = 1;
            foreach (SecondarySchoolArea entity in entiryList) {
                if (_stopEnabled) {
                    MappingRecorder.SetCurrentId(MappingTables.SecondarySchoolBoxMapping, entity.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(entity, 10, 15);
                foreach (int id in ids) {
                    AppendEitity(id, entity);
                    i++;
                    SendMessage(id, entity);
                    //if (i % 1000 == 0) {
                    //    boxRep.Add(mappingList);
                    //    mappingList = new List<SecondarySchoolAreaBoxMapping>();
                    //}
                }
            }

            for (Int32 index = 0; index * 1000 < mappingList.Count; index++) {
                IEnumerable<SecondarySchoolAreaBoxMapping> section = mappingList.Skip(index * 1000).Take(1000);
                List<SecondarySchoolAreaBoxMapping> mappings = new List<SecondarySchoolAreaBoxMapping>(section);
                boxRep.Add(mappings);
            }

            SendStatus(false, !_stopEnabled);
        }

        private void AppendEitity(int boxId, SecondarySchoolArea entity) {
            SecondarySchoolAreaBoxMapping mapping = new SecondarySchoolAreaBoxMapping();
            mapping.BoxId = boxId;
            mapping.SecondarySchoolAreaId = entity.Id;
            mappingList.Add(mapping);
        }

        private void SendMessage(int boxId, SecondarySchoolArea entity) {
            SendMessage(true, -1, entity.Id,
                string.Format("Box:{0},School-Secondary:{1},State:{2}",
                boxId,
                entity.Id,
                entity.StateCode));
        }
    }
}
