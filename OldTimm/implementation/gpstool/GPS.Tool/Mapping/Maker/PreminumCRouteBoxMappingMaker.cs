using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker
{
    class PreminumCRouteBoxMappingMaker : MappingMaker
    {
        public PreminumCRouteBoxMappingMaker() : base(true) { }

        protected override void MakeMapping()
        {
            SendStatus(true, false);
            List<PremiumCRoute> cRoutes = GetItems();
            int count = cRoutes.Count;
            SendMessage(true, count, 0, "");
            List<PremiumCRouteBoxMapping> mappings = new List<PremiumCRouteBoxMapping>();
            int num = 0;
            for (int i = 0; i < count; i++)
            {

                List<int> ids = ShapeMethods.GetBoxIds(cRoutes[i], 3, 4);
                foreach (int id in ids)
                {
                    num++;
                    PremiumCRouteBoxMapping mapping = new PremiumCRouteBoxMapping();
                    mapping.BoxId = id;
                    mapping.PreminumCRouteId = cRoutes[i].ID;
                    mappings.Add(mapping);
                    if (num % 1000 == 0)
                    {
                        InsertMappings(mappings);
                        mappings.Clear();
                    }
                    SendMessage(true, -1, cRoutes[i].ID, string.Format("Box:{0}, zip:{1}, croute:{2}", id, cRoutes[i].ZIP, cRoutes[i].CROUTE));
                }
            }
            InsertMappings(mappings);
            SendStatus(false, !_stopEnabled);
        }

        public List<PremiumCRoute> GetItems()
        {
            PremiumAreasDataContext preminumDataContext = new PremiumAreasDataContext();
            return preminumDataContext.PremiumCRoutes.ToList();
        }
        public void InsertMappings(List<PremiumCRouteBoxMapping> mappings)
        {
            PremiumAreasDataContext preminumDataContext = new PremiumAreasDataContext();
            preminumDataContext.PremiumCRouteBoxMappings.InsertAllOnSubmit(mappings);
            preminumDataContext.SubmitChanges();
        }
    }
}
