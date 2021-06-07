using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;

namespace GPS.Tool.Mapping.Maker
{
    class MetropolitanCoreAreaZipCodeMappingMaker : MappingMaker
    {
        protected override void MakeMapping()
        {
            Table<MetropolitanCoreArea> metropolitanCoreAreas = dataContext.MetropolitanCoreAreas;
            Table<FiveZipArea> fiveZipAreas = dataContext.FiveZipAreas;
            SendMessage(true, fiveZipAreas.Count(), 0, "");
            int i = 0;
            foreach (FiveZipArea fiveZipArea in fiveZipAreas)
            {
                if (_stopEnabled) break;
                bool exist = false;
                foreach (MetropolitanCoreArea metropolitanCoreArea in metropolitanCoreAreas)
                {
                    if (_stopEnabled) break;
                    if (AreaInPolygon(metropolitanCoreArea, fiveZipArea))
                    {
                        InsertItem(metropolitanCoreArea, fiveZipArea);
                        i++;
                        if (i % 1000 == 0)
                        {
                            dataContext.SubmitChanges();
                        }
                        exist = true;
                        SendMessage(metropolitanCoreArea, fiveZipArea);
                    }
                }
                if (!exist)
                {
                    SendMessage(fiveZipArea);
                }
            }
            dataContext.SubmitChanges();
        }

        private void InsertItem(MetropolitanCoreArea metropolitanCoreArea, FiveZipArea fiveZipArea)
        {
            ThreeZipArea threeZipArea = dataContext.GetThreeZipArea(fiveZipArea);
            MetropolitanCoreAreaZipCodeMapping mapping = new MetropolitanCoreAreaZipCodeMapping();
            mapping.MetropolitanCoreAreaId = metropolitanCoreArea.Id;
            mapping.FiveZipAreaCode = fiveZipArea.Code;
            mapping.FiveZipAreaId = fiveZipArea.Id;
            mapping.ThreeZipAreaId = threeZipArea.Id;
            mapping.ThreeZipCode = threeZipArea.Code;
            dataContext.MetropolitanCoreAreaZipCodeMappings.InsertOnSubmit(mapping);
        }

        private bool AreaInPolygon(MetropolitanCoreArea metropolitanCoreArea, FiveZipArea fiveZipArea)
        {
            if (ShapeMethods.BoxInBox(metropolitanCoreArea, fiveZipArea))
            {
                List<ICoordinate> masterCoordinates = new List<ICoordinate>();
                foreach (MetropolitanCoreAreaCoordinate fzc in metropolitanCoreArea.MetropolitanCoreAreaCoordinates)
                {
                    masterCoordinates.Add(fzc);
                }
                List<ICoordinate> innerCoordinates = new List<ICoordinate>();
                foreach (FiveZipAreaCoordinate fzc in fiveZipArea.FiveZipAreaCoordinates)
                {
                    innerCoordinates.Add(fzc);
                }
                return ShapeMethods.PolygonInPolygon(masterCoordinates, innerCoordinates);
            }
            else
            {
                return false;
            }
        }
        private void SendMessage(FiveZipArea fiveZipArea)
        {
            SendMessage(false, -1, fiveZipArea.Id, string.Format("CBSA:{0},Zip:{1},Zip'Id:{2}", "Null", fiveZipArea.Code, fiveZipArea.Id));
        }
        private void SendMessage(MetropolitanCoreArea metropolitanCoreArea, FiveZipArea fiveZipArea)
        {
            SendMessage(true, -1, fiveZipArea.Id, string.Format("CBSA:{0},Zip:{1},Zip'Id:{2}", metropolitanCoreArea.Code, fiveZipArea.Code, fiveZipArea.Id));
        }

    }
}
