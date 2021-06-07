using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker
{
    /// <summary>
    /// 
    /// </summary>
    class UpperSenateBoxMappingMaker:MappingMaker
    {
        /// <summary>
        /// 
        /// </summary>
        List<UpperSenateAreaBoxMapping> mappingList = new List<UpperSenateAreaBoxMapping>();

        protected override void MakeMapping()
        {
            SendStatus(true, false);
            int areaId = MappingRecorder.GetCurrentId(
              MappingTables.UpperSenateBoxMapping);

            AreaDataContext dataContext = new AreaDataContext();

            SenateAreaRepository entiryRep = new SenateAreaRepository();
            entiryRep.DataContext = dataContext;

            List<UpperSenateArea> entiryList = entiryRep.GetAll()
                .Where(b => b.Id >= areaId).ToList();
            SendMessage(true, entiryList.Count(), 0, "");
            int i = 1;

            SenateAreaBoxMappingRepository boxRep = new SenateAreaBoxMappingRepository();
            boxRep.DataContext = dataContext;

            foreach (UpperSenateArea entity in entiryList)
            {
                if (_stopEnabled)
                {
                    MappingRecorder.SetCurrentId(
                        MappingTables.UpperSenateBoxMapping, entity.Id);
                    break;
                }
                List<int> ids = ShapeMethods.GetBoxIds(entity, 50, 70);
                foreach (int id in ids)
                {
                    AppendEitity(id, entity);
                    i++;
                    SendMessage(id, entity);
                    //if (i % 1000 == 0)
                    //{
                    //    boxRep.Add(mappingList);
                    //    mappingList = new List<UpperSenateAreaBoxMapping>();
                    //}
                }
            }
            for (Int32 index = 0; index * 1000 < mappingList.Count; index++) {
                IEnumerable<UpperSenateAreaBoxMapping> section = mappingList.Skip(index * 1000).Take(1000);
                List<UpperSenateAreaBoxMapping> mappings = new List<UpperSenateAreaBoxMapping>(section);
                boxRep.Add(mappings);
            }

            SendStatus(false, !_stopEnabled);
        }

        private void AppendEitity(int boxId, UpperSenateArea entity)
        {
            UpperSenateAreaBoxMapping mapping = new UpperSenateAreaBoxMapping();
            mapping.BoxId = boxId;
            mapping.UpperSenateAreaId = entity.Id;
            mappingList.Add(mapping);
        }

        private void SendMessage(int boxId, UpperSenateArea entity)
        {
            SendMessage(true, -1, entity.Id,
                string.Format("Box:{0},Upper-Senate:{1},State:{2}",
                boxId,
                entity.Id,
                entity.StateCode));
        }
    }
}
