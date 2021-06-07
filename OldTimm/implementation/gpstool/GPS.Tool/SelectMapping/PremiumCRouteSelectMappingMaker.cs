using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using GPS.Tool.Mapping;

namespace GPS.Tool.SelectMapping
{
    class PremiumCRouteSelectMappingMaker : Maker<PremiumCRoute, PremiumCRouteSelectMapping, PremiumZipCoordinate>
    {

        protected override int GetItemsCount()
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
            return dataContext.PremiumCRoutes.Count();
        }

        protected override List<PremiumCRoute> GetItems(int skip, int count)
        {
            PremiumAreasDataContext dataContext = new PremiumAreasDataContext();
            return dataContext.PremiumCRoutes.OrderBy(t => t.ID).Skip(skip).Take(count).ToList();
        }

        protected override void MappingItems(List<PremiumCRoute> items)
        {
            SelectMappingDataContext dataContext = new SelectMappingDataContext();
            foreach (PremiumCRoute item in items)
            {
                try
                {
                    dataContext.PremiumCRouteSelectMappings.InsertAllOnSubmit(GetMappings(item));
                }
                catch
                {
                }
            }
            dataContext.SubmitChanges();
        }

        protected override List<PremiumCRouteSelectMapping> GetMappings(PremiumCRoute cRoute)
        {
            List<PremiumCRouteSelectMapping> mappings = new List<PremiumCRouteSelectMapping>();
            List<ICoordinate> cRouteShape = GetShape(cRoute);
            List<FiveZipArea> fiveZips = GetFiveZipAreas(cRoute.ZIP);
            bool hasInner = false;
            foreach (FiveZipArea fiveZip in fiveZips)
            {
                
                List<ICoordinate> fiveZipShape = GetShape(fiveZip.FiveZipAreaCoordinates.OrderBy(t => t.Id).ToList());
                if (ShapeMethods.PolygonInPolygon(fiveZipShape, cRouteShape))
                {
                    ZipRelation zipRelation = GetZipRelation(fiveZip, fiveZipShape);
                    if (zipRelation.ThreeZipIds.Count > 0)
                    {
                        foreach (int threeId in zipRelation.ThreeZipIds)
                        {
                            mappings.Add(new PremiumCRouteSelectMapping()
                            {
                                ThreeZipAreaId = threeId,
                                FiveZipAreaId = fiveZip.Id,
                                PremiumCRouteId = cRoute.ID
                            });
                        }
                    }
                    else
                    {
                        mappings.Add(new PremiumCRouteSelectMapping()
                        {
                            ThreeZipAreaId = 0,
                            FiveZipAreaId = fiveZip.Id,
                            PremiumCRouteId = cRoute.ID
                        });
                    }
                    hasInner = true;
                }
                
            }
            SendMessage(cRoute.CROUTE, hasInner);
            if (hasInner)
            {
                
                _innerCount++;
            }
            _current++;
            return mappings;
        }

        protected override List<ICoordinate> GetShape(PremiumCRoute t)
        {
            List<ICoordinate> shape = new List<ICoordinate>();
            var coordinates = t.PremiumCRouteCoordinates.OrderBy(pc => pc.ID).ToList();
            foreach (var c in coordinates)
            {
                shape.Add(c);
            }
            return shape;
        }
    }
}
