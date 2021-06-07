using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer;

namespace GPS.DomainLayer.Area {
    class BlockGroupService {
        private AreaDataContext _areaDataContext;
        public BlockGroupService() {
            _areaDataContext = DataContext.CreateDataContext();
        }

        public List<BlockGroup> GetBlockGroupsByBoxIds(List<int> boxIds) {
            var items = (from bgbm in _areaDataContext.BlockGroupBoxMappings
                         where boxIds.Contains(bgbm.BoxId)
                         select (new {
                             bgbm.BlockGroup,
                             tractId = (from bgm1 in _areaDataContext.BlockGroupMappings
                                        where bgm1.BlockGroupId == bgbm.BlockGroupId
                                        select bgm1.TractId
                                        ).SingleOrDefault(),
                             fiveZipIds = (from bgm2 in _areaDataContext.BlockGroupMappings
                                           where bgm2.BlockGroupId == bgbm.BlockGroupId
                                           select bgm2.FiveZipAreaId
                                               ).Distinct().ToList(),

                             threeZipIds = (from bgm3 in _areaDataContext.BlockGroupMappings
                                            where bgm3.BlockGroupId == bgbm.BlockGroupId
                                            select bgm3.FiveZipAreaId
                                               ).Distinct().ToList(),

                         })).Distinct();

            List<BlockGroup> bgs = new List<BlockGroup>();
            foreach (var item in items) {
                item.BlockGroup.tractId = item.tractId;
                item.BlockGroup.FiveZipIds = item.fiveZipIds;
                item.BlockGroup.ThreeZipIds = item.threeZipIds;
                bgs.Add(item.BlockGroup);
            }

            return bgs;
        }
    }
}
