using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Data;
using GPS.Tool.Mapping;

namespace GPS.Tool.SelectMapping
{
    class BlockGroupSelectMappingMaker : Maker<Tract, Data.BlockGroupSelectMapping, Data.TractCoordinate>
    {

        protected override int GetItemsCount()
        {
            AreaDataContext dataContext = new AreaDataContext();
            return dataContext.Tracts.Count();
        }

        protected override List<Tract> GetItems(int skip, int count)
        {
            AreaDataContext dataContext = new AreaDataContext();
            return dataContext.Tracts.OrderBy(t => t.Id).Skip(skip).Take(count).ToList();
        }

        protected override void MappingItems(List<Tract> items)
        {
            SelectMappingDataContext dataContext = new SelectMappingDataContext();
            foreach (Tract item in items)
            {
                try
                {
                    dataContext.BlockGroupSelectMappings.InsertAllOnSubmit(GetMappings(item));
                }
                catch
                {
                }
            }
            dataContext.SubmitChanges();
        }

        protected override List<BlockGroupSelectMapping> GetMappings(Tract t)
        {
            List<BlockGroupSelectMapping> mappings = new List<BlockGroupSelectMapping>();
            List<ICoordinate> cRouteShape = GetShape(t);
            List<FiveZipArea> fiveZips = GetBoxFiveZipAreas(t);
            List<BlockGroup> blockGroups = GetBlockGroups(t);
            bool hasInner = false;
            foreach (FiveZipArea fiveZip in fiveZips)
            {
                List<ICoordinate> fiveZipShape = GetShape(fiveZip.FiveZipAreaCoordinates.OrderBy(c => c.Id).ToList());

                if (ShapeMethods.PolygonInPolygon(fiveZipShape, cRouteShape))
                {
                    ZipRelation zipRelation = GetZipRelation(fiveZip, fiveZipShape);
                    foreach (BlockGroup bg in blockGroups)
                    {
                        if (zipRelation.ThreeZipIds.Count > 0)
                        {
                            foreach (int threeZipId in zipRelation.ThreeZipIds)
                            {
                                mappings.Add(new BlockGroupSelectMapping()
                                {
                                    ThreeZipAreaId = threeZipId,
                                    FiveZipAreaId = fiveZip.Id,
                                    TractId = t.Id,
                                    BlockGroupId = bg.Id
                                });
                            }
                        }
                        else
                        {
                            mappings.Add(new BlockGroupSelectMapping()
                            {
                                ThreeZipAreaId = 0,
                                FiveZipAreaId = fiveZip.Id,
                                TractId = t.Id,
                                BlockGroupId = bg.Id
                            });
                        }
                    }
                    hasInner = true;
                }
            }

            SendMessage(t.Code, hasInner);
            if (hasInner)
            {
                _innerCount++;
            }

            _current++;
            return mappings;
        }

        private List<BlockGroup> GetBlockGroups(Tract tract)
        {
            AreaDataContext dataContext = new AreaDataContext();
            return dataContext.BlockGroups.Where(t => t.StateCode == tract.StateCode && t.CountyCode == tract.CountyCode && t.TractCode == tract.Code).OrderBy(t => t.Id).ToList();
        }

        private List<FiveZipArea> GetBoxFiveZipAreas(Tract tract)
        {
            List<FiveZipArea> fiveZips = new List<FiveZipArea>();
            AreaDataContext dataContext = new AreaDataContext();
            List<int> ids = ShapeMethods.GetBoxIds(tract, 10, 15);
            foreach (int id in ids)
            {
                fiveZips.AddRange(dataContext.FiveZipBoxMappings.Where(t => id == t.BoxId).Select(t => t.FiveZipArea).Distinct().ToList());
            }
            return fiveZips.Distinct().ToList();
        }

        protected override List<ICoordinate> GetShape(Tract t)
        {
            List<ICoordinate> shape = new List<ICoordinate>();
            var coordinates = t.TractCoordinates.OrderBy(tc => tc.Id).ToList();
            foreach (var c in coordinates)
            {
                shape.Add(c);
            }
            return shape;
        }
    }
}
