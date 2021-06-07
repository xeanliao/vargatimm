using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.Data
{
    public partial class AreaDataContext
    {
        public Tract GetTract(BlockGroup blockGroup)
        {
            Tract tract = this.Tracts.Where(t => t.StateCode == blockGroup.StateCode && t.CountyCode == blockGroup.CountyCode && t.Code == blockGroup.TractCode).First();
            return tract;
        }

        public ThreeZipArea GetThreeZipArea(FiveZipArea fiveZipArea)
        {
            string threeZip = fiveZipArea.Code.Substring(0, 3);
            ThreeZipArea threeZipArea = this.ThreeZipAreas.Where(t => t.Code == threeZip).First();
            return threeZipArea;
        }

        public CountyArea GetCountyArea(BlockGroup blockGroup)
        {
            CountyArea county = this.CountyAreas.Where(t => t.Code == blockGroup.CountyCode && t.StateCode == blockGroup.StateCode).First();
            return county;
        }

        public List<BlockGroup> GetBlockGroups(Tract tract)
        {
            var items = this.BlockGroups.Where(t => t.StateCode == tract.StateCode && t.CountyCode == tract.CountyCode && t.TractCode == tract.Code);
            return items.ToList();
        }

        public List<FiveZipArea> GetFiveZipsByBoxIds(List<int> boxIds)
        {
            List<FiveZipArea> fiveZips = new List<FiveZipArea>();
            foreach (int box in boxIds) {
                var items = this.FiveZipBoxMappings.Where(t => t.BoxId == box).Select(t => t.FiveZipArea);
                fiveZips.AddRange(items);
            }
            return new List<FiveZipArea>(fiveZips.Distinct(new FiveZipAreaEqualityComparer()));
        }

        class FiveZipAreaEqualityComparer : IEqualityComparer<FiveZipArea> {
            #region IEqualityComparer<FiveZipArea> Members

            public bool Equals(FiveZipArea x, FiveZipArea y) {
                return x.Id == y.Id;
            }

            public int GetHashCode(FiveZipArea obj) {
                return obj.Id;
            }

            #endregion
        }
    }
        
    public partial class BlockGroup : ICoordinateArea { }
    public partial class Tract : ICoordinateArea { }
    public partial class FiveZipArea : ICoordinateArea{ }
    public partial class ThreeZipArea : ICoordinateArea { }
    public partial class MetropolitanCoreArea : ICoordinateArea { }
    public partial class UrbanArea : ICoordinateArea { }
    public partial class CountyArea : ICoordinateArea { }
    public partial class ElementarySchoolArea : ICoordinateArea { }
    public partial class SecondarySchoolArea : ICoordinateArea { }
    public partial class UnifiedSchoolArea : ICoordinateArea { }
    public partial class LowerHouseArea : ICoordinateArea { }
    public partial class UpperSenateArea : ICoordinateArea { }

    public partial class BlockGroupCoordinate : ICoordinate { }
    public partial class FiveZipAreaCoordinate : ICoordinate, IShapeCoordinate  { }
    public partial class MetropolitanCoreAreaCoordinate : ICoordinate { }
    public partial class UrbanAreaCoordinate : ICoordinate { }
    public partial class TractCoordinate : ICoordinate { }
    public partial class ThreeZipAreaCoordinate : ICoordinate { }
}
