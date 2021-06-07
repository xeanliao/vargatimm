using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker
{
    class UrbanAreaBoxMappingMaker:MappingMaker
    {
        List<UrbanAreaBoxMapping> mappingList;

        protected override void MakeMapping()
        {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(MappingTables.UrbanAreaBoxMapping);

            AreaDataContext dataContext = new AreaDataContext();

            UrbanAreaRepository entiryRep = new UrbanAreaRepository();
            entiryRep.DataContext = dataContext;

            List<UrbanArea> entiryList = entiryRep.GetAll()
                .Where(b => b.Id >= areaId).ToList();
            SendMessage(true, entiryList.Count(), 0, "");
            int i = 1;

            UrbanAreaBoxMappingRepository boxRep = new UrbanAreaBoxMappingRepository();
            boxRep.DataContext = dataContext;

            mappingList = new List<UrbanAreaBoxMapping>();
            foreach (UrbanArea entity in entiryList)
            {
                if (_stopEnabled)
                {
                    MappingRecorder.SetCurrentId(
                        MappingTables.UrbanAreaBoxMapping, entity.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(entity, 100, 150);
                foreach (int id in ids)
                {
                    AppendEitity(id, entity);
                    i++;
                    SendMessage(id, entity);
                    //if (i % 1000 == 0)
                    //{
                    //    boxRep.Add(mappingList);
                    //    mappingList = new List<UrbanAreaBoxMapping>();
                    //}
                }
            }

            for (Int32 index = 0; index * 1000 < mappingList.Count; index++) {
                IEnumerable<UrbanAreaBoxMapping> section = mappingList.Skip(index * 1000).Take(1000);
                List<UrbanAreaBoxMapping> mappings = new List<UrbanAreaBoxMapping>(section);
                boxRep.Add(mappings);
            }
            SendStatus(false, !_stopEnabled);
        }

        private void AppendEitity(int boxId, UrbanArea entity)
        {
            UrbanAreaBoxMapping mapping =
                new UrbanAreaBoxMapping();
            mapping.BoxId = boxId;
            mapping.UrbanAreaId = entity.Id;
            mappingList.Add(mapping);
        }

        private void SendMessage(int boxId, UrbanArea entity)
        {
            SendMessage(true, -1, entity.Id,
                string.Format("Box:{0},urban:{1}",
                boxId,
                entity.Id));
        }
    }
}
