using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using System.Data.Linq;

namespace GPS.Tool.Mapping.Maker
{
    class UrbanAreaZipCodeMappingMaker : MappingMaker
    {
        protected override void MakeMapping()
        {
            Table<UrbanArea> urbanAreas = dataContext.UrbanAreas;
            Table<FiveZipArea> fiveZipAreas = dataContext.FiveZipAreas;
            SendMessage(true, fiveZipAreas.Count(), 0, "");
            int i = 0;
            foreach (FiveZipArea fiveZipArea in fiveZipAreas)
            {
                if (_stopEnabled) break;
                bool exist = false;
                foreach (UrbanArea urbanArea in urbanAreas)
                {
                    if (_stopEnabled) break;
                    if (AreaInPolygon(urbanArea, fiveZipArea))
                    {
                        InsertItem(urbanArea, fiveZipArea);
                        i++;
                        if (i % 1000 == 0)
                        {
                            dataContext.SubmitChanges();
                        }
                        exist = true;
                        SendMessage(urbanArea, fiveZipArea);
                    }
                }
                if (!exist)
                {
                    SendMessage(fiveZipArea);
                }
            }
            dataContext.SubmitChanges();
        }

        private void InsertItem(UrbanArea urbanArea, FiveZipArea fiveZipArea)
        {
            ThreeZipArea threeZipArea = dataContext.GetThreeZipArea(fiveZipArea);
            UrbanAreaZipCodeMapping mapping = new UrbanAreaZipCodeMapping();
            mapping.UrbanAreaId = urbanArea.Id;
            mapping.UrbanAreaCode = urbanArea.Code;
            mapping.FiveZipCode = fiveZipArea.Code;
            mapping.FiveZipAreaId = fiveZipArea.Id;
            mapping.ThreeZipAreaId = threeZipArea.Id;
            mapping.ThreeZipCode = threeZipArea.Code;
            dataContext.UrbanAreaZipCodeMappings.InsertOnSubmit(mapping);
        }

        private bool AreaInPolygon(UrbanArea urbanArea, FiveZipArea fiveZipArea)
        {
            if (ShapeMethods.BoxInBox(urbanArea, fiveZipArea))
            {
                List<ICoordinate> masterCoordinates = new List<ICoordinate>();
                foreach (UrbanAreaCoordinate uac in urbanArea.UrbanAreaCoordinates)
                {
                    masterCoordinates.Add(uac);
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
            SendMessage(false, -1, fiveZipArea.Id, string.Format("Urban:{0},Zip:{1},Zip'Id:{2}", "Null", fiveZipArea.Code, fiveZipArea.Id));
        }
        private void SendMessage(UrbanArea metropolitanCoreArea, FiveZipArea fiveZipArea)
        {
            SendMessage(true, -1, fiveZipArea.Id, string.Format("Urban:{0},Zip:{1},Zip'Id:{2}", metropolitanCoreArea.Code, fiveZipArea.Code, fiveZipArea.Id));
        }

    }
}
