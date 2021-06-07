using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker {
    class CBSABoxMappingMaker : MappingMaker {
        List<MetropolitanCoreAreaBoxMapping> mappingList = new List<MetropolitanCoreAreaBoxMapping>();

        protected override void MakeMapping() {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(MappingTables.CBSABoxMapping);

            AreaDataContext dataContext = new AreaDataContext();

            CBSARepository cbsaRep = new CBSARepository();
            cbsaRep.DataContext = dataContext;

            CBSABoxMappingRepository boxRep = new CBSABoxMappingRepository();
            boxRep.DataContext = dataContext;

            List<MetropolitanCoreArea> cbsaList = cbsaRep.GetAll().Where(b => b.Id >= areaId).ToList();

            SendMessage(true, cbsaList.Count(), 0, "");

            int i = 1;
            foreach (MetropolitanCoreArea cbsa in cbsaList) {
                if (_stopEnabled) {
                    MappingRecorder.SetCurrentId(
                        MappingTables.CBSABoxMapping, cbsa.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(cbsa, 100, 150);
                foreach (int id in ids) {
                    AppendCBSA(id, cbsa);
                    i++;
                    SendMessage(id, cbsa);
                    //if (i % 1000 == 0) {
                    //    boxRep.Add(mappingList);
                    //    mappingList = new List<MetropolitanCoreAreaBoxMapping>();
                    //}
                }
            }

            for (Int32 index = 0; index * 1000 < mappingList.Count; index++) {
                IEnumerable<MetropolitanCoreAreaBoxMapping> section = mappingList.Skip(index * 1000).Take(1000);
                List<MetropolitanCoreAreaBoxMapping> mappings = new List<MetropolitanCoreAreaBoxMapping>(section);
                boxRep.Add(mappings);
            }
            
            SendStatus(false, !_stopEnabled);
        }

        private void AppendCBSA(int boxId, MetropolitanCoreArea cbsa) {
            MetropolitanCoreAreaBoxMapping mapping =
                new MetropolitanCoreAreaBoxMapping();
            mapping.BoxId = boxId;
            mapping.MetropolitanCoreAreaId = cbsa.Id;
            mappingList.Add(mapping);
        }

        private void SendMessage(int boxId, MetropolitanCoreArea cbsa) {
            SendMessage(true, -1, cbsa.Id,
                string.Format("Box:{0},CBSA:{1}",
                boxId,
                cbsa.Id));
        }
    }
}
