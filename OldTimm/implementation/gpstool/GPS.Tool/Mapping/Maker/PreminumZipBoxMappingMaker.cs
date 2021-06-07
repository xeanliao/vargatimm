using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;

namespace GPS.Tool.Mapping.Maker
{
    class PreminumZipBoxMappingMaker : MappingMaker
    {
        public PreminumZipBoxMappingMaker() : base(true) { }
        protected override void MakeMapping()
        {
            SendStatus(true, false);
            List<PremiumZip> zips = GetItems();
            int count = zips.Count;
            SendMessage(true, count, 0, "");
            List<PremiumZipBoxMapping> mappings = new List<PremiumZipBoxMapping>();
            int num = 0;
            for (int i = 31483; i < count; i++)
            {

                List<int> ids = ShapeMethods.GetBoxIds(zips[i], 10, 15);
                foreach (int id in ids)
                {
                    num++;
                    PremiumZipBoxMapping mapping = new PremiumZipBoxMapping();
                    mapping.BoxId = id;
                    mapping.PreminumZipId = zips[i].ID;
                    mappings.Add(mapping);
                    if (num % 1000 == 0)
                    {
                        InsertMappings(mappings);
                        mappings.Clear();
                    }
                    SendMessage(true, -1, zips[i].ID, string.Format("Box:{0},zip:{1}", id, zips[i].ZIP));
                }
            }
            InsertMappings(mappings);
            SendStatus(false, !_stopEnabled);
        }

        public List<PremiumZip> GetItems()
        {
            PremiumAreasDataContext preminumDataContext = new PremiumAreasDataContext();
            return preminumDataContext.PremiumZips.ToList();
        }
        public void InsertMappings(List<PremiumZipBoxMapping> mappings)
        {
            PremiumAreasDataContext preminumDataContext = new PremiumAreasDataContext();
            preminumDataContext.PremiumZipBoxMappings.InsertAllOnSubmit(mappings);
            preminumDataContext.SubmitChanges();
        }
    }
}
