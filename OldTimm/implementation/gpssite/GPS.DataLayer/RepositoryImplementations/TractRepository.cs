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
    public class TractRepository : RepositoryBase, IExportSourceRepository<Tract>, GPS.DataLayer.ITractRepository
    {
        public TractRepository() { }

        public TractRepository(ISession session) : base(session) { }

        public IEnumerable<Tract> GetBoxItems(int boxId)
        {
            const string queryFormat = "select distinct t from Tract t join t.TractBoxMappings tbm where tbm.BoxId = :boxId";
            return InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<Tract>();
        }

        public Tract GetItem(int id)
        {
            return InternalSession.Get<Tract>(id);
        }

        public void SetTractEnabled(string stateCode, string countyCode, string code, int total, string description, bool enabled)
        {
            Tract t = InternalSession.Linq<Tract>().Where(c => c.StateCode == stateCode && c.CountyCode == countyCode && c.Code == code).SingleOrDefault();

            if (null != t)
            {
                t.OTotal = total;
                t.Description = description;
                t.IsEnabled = enabled;

                base.Update(t);
            }
        }

        public List<Tract> GetTracts(string stateCode, string countyCode, string code)
        {
            return InternalSession.Linq<Tract>().Where(t => t.StateCode == stateCode && t.CountyCode == countyCode && t.Code == code).ToList();
        }

        #region IExportSourceProvider Members

        IEnumerable<Tract> IExportSourceRepository<Tract>.GetExportSource(IExportSourceSpecification spec)
        {
            const string queryFormat =
                  " select distinct t from Tract t join t.BlockGroupSelectMappings bgsm where"
                + " bgsm.Tract.Id in (:inTractIds)"
                + " or bgsm.FiveZipArea.Id in (:inFiveZipIds) and bgsm.Tract.Id not in (:nonTractIds)"
                + " or bgsm.ThreeZipArea.Id in (:inThreeZipIds) and bgsm.FiveZipArea.Id not in (:nonFiveZipIds)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("inTractIds", spec.SelectedTractIds.ToArray())
                .SetParameterList("nonTractIds", spec.DeselectedTractIds.ToArray())
                .SetParameterList("inFiveZipIds", spec.SelectedFiveZipIds.ToArray())
                .SetParameterList("inThreeZipIds", spec.SelectedThreeZipIds.ToArray())
                .SetParameterList("nonFiveZipIds", spec.DeselectedFiveZipIds.ToArray())
                .List<Tract>();
        }

        #endregion

        #region IImportedDataRepository<Tract> Members

        public IEnumerable<Tract> GetShapesAccordingToCodes(string[] arbitraryUniqueCodes)
        {
            const string queryFormat =
                "select distinct t from Tract t where t.ArbitraryUniqueCode in (:codes)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("codes", arbitraryUniqueCodes)
                .List<Tract>();
        }

        #endregion

        public IEnumerable<Tract> GetTractsSatisfyingSubMap(TractsSatisfyingSubMapSpecification spec)
        {
            const string queryFormatAllTractIds = " select distinct bgsm.Tract.Id from BlockGroupSelectMapping bgsm where"
                + " bgsm.BlockGroup.Id in (:selectedBlockGroupIds) or"
                + " (bgsm.Tract.Id in (:selectedTractIds) and bgsm.BlockGroup.Id not in (:deselectedBlockGroupIds))";
            IList<Int32> allTractIds = InternalSession.CreateQuery(queryFormatAllTractIds)
                            .SetParameterList("selectedTractIds", spec.SelectedTractIds.ToArray())
                            .SetParameterList("selectedBlockGroupIds", spec.SelectedBlockGroupIds.ToArray())
                            .SetParameterList("deselectedBlockGroupIds", spec.DeselectedBlockGroupIds.ToArray())
                            .List<Int32>();

            const string queryFormatNonTractIds = " select distinct bgsm.Tract.Id from BlockGroupSelectMapping bgsm where"
                + " bgsm.Tract.Id in (:allTractIds) and"
                + " (not (bgsm.BlockGroup.Id in (:selectedBlockGroupIds) or"
                + " (bgsm.Tract.Id in (:selectedTractIds) and bgsm.BlockGroup.Id not in (:deselectedBlockGroupIds))))";
            IList<Int32> nonTractIds = InternalSession.CreateQuery(queryFormatNonTractIds)
                            .SetParameterList("allTractIds", allTractIds.ToArray())
                            .SetParameterList("selectedTractIds", spec.SelectedTractIds.ToArray())
                            .SetParameterList("selectedBlockGroupIds", spec.SelectedBlockGroupIds.ToArray())
                            .SetParameterList("deselectedBlockGroupIds", spec.DeselectedBlockGroupIds.ToArray())
                            .List<Int32>();
            if (nonTractIds.Count == 0) { nonTractIds.Add(-1); }

            const string queryFormatTract = " select distinct bgsm.Tract from BlockGroupSelectMapping bgsm where"
                + " bgsm.Tract.Id in (:allTractIds) and bgsm.Tract.Id not in (:nonTractIds)";
            return InternalSession.CreateQuery(queryFormatTract)
                .SetParameterList("allTractIds", allTractIds.ToArray())
                .SetParameterList("nonTractIds", nonTractIds.ToArray())
                .List<Tract>().OrderBy(t=>t.Code);
        }


        public IEnumerable<Tract> GetTractsSatisfyingDistributionMap(TractsSatisfyingDistributionMapSpecification spec)
        {
            //const string queryFormatAllTractIds = " select distinct bgsm.Tract.Id from BlockGroupSelectMapping bgsm where"
            //    + " bgsm.BlockGroup.Id in (:selectedBlockGroupIds) or"
            //    + " (bgsm.Tract.Id in (:selectedTractIds) and bgsm.BlockGroup.Id not in (:deselectedBlockGroupIds))";
            //IList<Int32> allTractIds = InternalSession.CreateQuery(queryFormatAllTractIds)
            //                .SetParameterList("selectedTractIds", spec.SelectedTractIds.ToArray())
            //                .SetParameterList("selectedBlockGroupIds", spec.SelectedBlockGroupIds.ToArray())
            //                .SetParameterList("deselectedBlockGroupIds", spec.DeselectedBlockGroupIds.ToArray())
            //                .List<Int32>();

            //const string queryFormatNonTractIds = " select distinct bgsm.Tract.Id from BlockGroupSelectMapping bgsm where"
            //    + " bgsm.Tract.Id in (:allTractIds) and"
            //    + " (not (bgsm.BlockGroup.Id in (:selectedBlockGroupIds) or"
            //    + " (bgsm.Tract.Id in (:selectedTractIds) and bgsm.BlockGroup.Id not in (:deselectedBlockGroupIds))))";
            //IList<Int32> nonTractIds = InternalSession.CreateQuery(queryFormatNonTractIds)
            //                .SetParameterList("allTractIds", allTractIds.ToArray())
            //                .SetParameterList("selectedTractIds", spec.SelectedTractIds.ToArray())
            //                .SetParameterList("selectedBlockGroupIds", spec.SelectedBlockGroupIds.ToArray())
            //                .SetParameterList("deselectedBlockGroupIds", spec.DeselectedBlockGroupIds.ToArray())
            //                .List<Int32>();
            //if (nonTractIds.Count == 0) { nonTractIds.Add(-1); }

            //const string queryFormatTract = " select distinct bgsm.Tract from BlockGroupSelectMapping bgsm where"
            //    + " bgsm.Tract.Id in (:allTractIds) and bgsm.Tract.Id not in (:nonTractIds)";
            //return InternalSession.CreateQuery(queryFormatTract)
            //    .SetParameterList("allTractIds", allTractIds.ToArray())
            //    .SetParameterList("nonTractIds", nonTractIds.ToArray())
            //    .List<Tract>();
            return new List<Tract>();
        }
    }
}
