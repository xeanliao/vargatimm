using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.QuerySpecifications;
using GPS.DomainLayer.Interfaces;

namespace GPS.DataLayer
{
    public class BlockGroupRepository : RepositoryBase, IExportSourceRepository<BlockGroup>, GPS.DataLayer.IBlockGroupRepository
    {
        public BlockGroupRepository() { }

        public BlockGroupRepository(ISession session) : base(session) { }

        public IEnumerable<BlockGroup> GetBoxItems(int boxId)
        {
            const string queryFormat = "select distinct t from BlockGroup t join t.BlockGroupBoxMappings tbm where tbm.BoxId = :boxId";
            return InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<BlockGroup>();
        }

        public BlockGroup GetItem(int id)
        {
            return InternalSession.Get<BlockGroup>(id);
        }

        public List<BlockGroup> GetAreaByBGArbitraryUniqueCode(string arbitraryUniqueCode)
        {
            List<BlockGroup> results = new List<BlockGroup>();

            const string queryFormat = "select distinct bg from BlockGroup bg where bg.ArbitraryUniqueCode = :code";

            IList<BlockGroup> items = InternalSession.CreateQuery(queryFormat).SetString("code", arbitraryUniqueCode).List<BlockGroup>();

            results.AddRange(items);

            return results;
        }

        public IEnumerable<BlockGroup> GetBoxItems(IEnumerable<int> boxIds)
        {
            IList<BlockGroup> items = new List<BlockGroup>();

            if (null != boxIds && boxIds.Count() > 0)
            {
                const string queryFormat = "select distinct bgbm.BlockGroup from BlockGroupBoxMapping bgbm where bgbm.BoxId in (:boxIds)";
                items = InternalSession.CreateQuery(queryFormat).SetParameterList("boxIds", boxIds.ToArray()).List<BlockGroup>();
            }

            return items;
        }

        public void SetBlockGroupEnabled(string stateCode, string countyCode, string tractCode, string code, int total, string description, bool enabled)
        {
            BlockGroup bg = InternalSession.Linq<BlockGroup>().Where(
                    c => c.StateCode == stateCode &&
                    c.CountyCode == countyCode &&
                    c.TractCode == tractCode &&
                    c.Code == code).FirstOrDefault();

            if (null != bg)
            {
                bg.OTotal = total;
                bg.Description = description;
                bg.IsEnabled = enabled;

                InternalSession.SaveOrUpdate(bg);
                InternalSession.Flush();
            }
        }

        public List<BlockGroup> GetBlockGroups(string stateCode, string countyCode, string tractCode, string code)
        {
            return InternalSession.Linq<BlockGroup>().Where(t => t.StateCode == stateCode && t.CountyCode == countyCode && t.TractCode == tractCode && t.Code == code).ToList();
        }

        #region IExportSourceProvider Members

        IEnumerable<BlockGroup> IExportSourceRepository<BlockGroup>.GetExportSource(IExportSourceSpecification spec)
        {
            const string queryFormat = 
                  " select distinct b from BlockGroup b join b.BlockGroupSelectMappings bgsm where"
                + " bgsm.BlockGroup.Id in (:inBGIds)"
                + " or bgsm.Tract.Id in (:inTractIds) and bgsm.BlockGroup.Id not in (:nonBGIds)"
                + " or bgsm.FiveZipArea.Id in (:inFiveZipIds) and bgsm.Tract.Id not in (:nonTractIds)"
                + " or bgsm.ThreeZipArea.Id in (:inThreeZipIds) and bgsm.FiveZipArea.Id not in (:nonFiveZipIds)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("inBGIds", spec.SelectedBlockGroupIds.ToArray())
                .SetParameterList("nonBGIds", spec.DeselectedBlockGroupIds.ToArray())
                .SetParameterList("inTractIds", spec.SelectedTractIds.ToArray())
                .SetParameterList("nonTractIds", spec.DeselectedTractIds.ToArray())
                .SetParameterList("inFiveZipIds", spec.SelectedFiveZipIds.ToArray())
                .SetParameterList("inThreeZipIds", spec.SelectedThreeZipIds.ToArray())
                .SetParameterList("nonFiveZipIds", spec.DeselectedFiveZipIds.ToArray())
                .List<BlockGroup>();
        }

        #endregion

        #region IImportedDataRepository<BlockGroup> Members

        public IEnumerable<BlockGroup> GetShapesAccordingToCodes(string[] arbitraryUniqueCodes)
        {
            const string queryFormat =
                "select distinct bg from BlockGroup bg where bg.ArbitraryUniqueCode in (:codes)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("codes", arbitraryUniqueCodes)
                .List<BlockGroup>();
        }

        #endregion

        public IEnumerable<BlockGroup> GetBlockGroupsSatisfyingSubMap(BlockGroupsSatisfyingSubMapSpecification spec)
        {
            const string queryFormat =
                  " select distinct bg from BlockGroup bg join bg.BlockGroupSelectMappings bgsm where"
                + " (bg.Id in (:selectedBGIds) or"
                + " (bgsm.Tract.Id in (:selectedTractIds) and bg.Id not in (:deselectedBGIds))) and"
                + " bgsm.Tract.Id not in (:deselectedTractIds)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("selectedTractIds", spec.SelectedTractIds.ToArray())
                .SetParameterList("deselectedTractIds", spec.DeselectedTractIds.ToArray())
                .SetParameterList("selectedBGIds", spec.SelectedBGIds.ToArray())
                .SetParameterList("deselectedBGIds", spec.DeselectedBGIds.ToArray())
                .List<BlockGroup>().OrderBy(b=>b.Code);

        }


        public IEnumerable<BlockGroup> GetBlockGroupsSatisfyingDistributionMap(BlockGroupsSatisfyingDistributionMapSpecification spec)
        {
            //const string queryFormat =
            //      " select distinct bg from BlockGroup bg join bg.BlockGroupSelectMappings bgsm where"
            //    + " (bg.Id in (:selectedBGIds) or"
            //    + " (bgsm.Tract.Id in (:selectedTractIds) and bg.Id not in (:deselectedBGIds))) and"
            //    + " bgsm.Tract.Id not in (:deselectedTractIds)";

            //return InternalSession.CreateQuery(queryFormat)
            //    .SetParameterList("selectedTractIds", spec.SelectedTractIds.ToArray())
            //    .SetParameterList("deselectedTractIds", spec.DeselectedTractIds.ToArray())
            //    .SetParameterList("selectedBGIds", spec.SelectedBGIds.ToArray())
            //    .SetParameterList("deselectedBGIds", spec.DeselectedBGIds.ToArray())
            //    .List<BlockGroup>();
            return new List<BlockGroup>();

        }
    }
}
