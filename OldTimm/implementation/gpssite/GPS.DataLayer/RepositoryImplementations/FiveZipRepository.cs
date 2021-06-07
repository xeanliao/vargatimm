using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Interfaces;
using GPS.DataLayer;
using GPS.DomainLayer.QuerySpecifications;
using System.Collections;
using NHibernate.Criterion;

namespace GPS.DataLayer
{
    public class FiveZipRepository : RepositoryBase, GPS.DataLayer.IFiveZipRepository
    {
        public FiveZipRepository() { }

        public FiveZipRepository(ISession session) : base(session) { }


        public IEnumerable<FiveZipArea> GetBoxItems(int boxId)
        {
            const string queryFormat = "select distinct fza from FiveZipArea fza join fza.FiveZipBoxMappings fzbm where fzbm.BoxId = :boxId and fza.IsInnerRing=0";
            return InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<FiveZipArea>();
        }

        public FiveZipArea GetItem(int id)
        {
            return InternalSession.Get<FiveZipArea>(id);
        }

        public void SetFiveZipEnabled(string code, int total, string desription, bool enabled)
        {
            IEnumerable<FiveZipArea> items = InternalSession.Linq<FiveZipArea>().Where(c => c.Code == code);

            if (null != items)
            {
                foreach (FiveZipArea fza in items)
                {
                    fza.OTotal = total;
                    fza.Description = desription;
                    fza.IsEnabled = enabled;

                    InternalSession.Update(fza);
                }

                InternalSession.Flush();
            }
        }

        public List<FiveZipArea> GetAreaByZipCode(string zipCode)
        {
            List<FiveZipArea> results = new List<FiveZipArea>();

            const string queryFormat = "select distinct fza from FiveZipArea fza where fza.Code = :code and fza.IsInnerRing=0";

            IList<FiveZipArea> items = InternalSession.CreateQuery(queryFormat).SetString("code", zipCode).List<FiveZipArea>();

            results.AddRange(items);

            return results;
        }



        #region IExportSourceProvider Members
        public IEnumerable<FiveZipArea> GetExportSource(IExportSourceSpecification spec)
        {
            const string queryFormat =
                              " select distinct fza from FiveZipArea fza join fza.PremiumCRouteSelectMappings bgsm where "
                            + " bgsm.FiveZipAreaId in (:inFiveZipIds)"
                            + " or bgsm.ThreeZipAreaId in (:inThreeZipIds) and bgsm.FiveZipAreaId not in (:nonFiveZipIds)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("inFiveZipIds", spec.SelectedFiveZipIds.ToArray())
                .SetParameterList("inThreeZipIds", spec.SelectedThreeZipIds.ToArray())
                .SetParameterList("nonFiveZipIds", spec.DeselectedFiveZipIds.ToArray())
                .List<FiveZipArea>();
        }

        #endregion

        #region IImportedDataRepository<FiveZipArea> Members

        public IEnumerable<FiveZipArea> GetShapesAccordingToCodes(string[] codes)
        {
            const string queryFormat =
                "select distinct fza from FiveZipArea fza where fza.Code in (:codes)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("codes", codes)
                .List<FiveZipArea>();
        }

        #endregion


        public IEnumerable<BlockGroup> GetBGByFiveZip(int fivezipId)
        {
            ICriteria criteria = InternalSession.CreateCriteria(typeof(BlockGroup), "bg").CreateCriteria("BlockGroupSelectMappings", "bgsm")
                .CreateCriteria("FiveZipArea", "fza").Add(Restrictions.Eq("fza.Id", fivezipId));

            return criteria.List<BlockGroup>();
        }

        public IEnumerable<BlockGroup> GetBGByTract(int tractId)
        {
            ICriteria criteria = InternalSession.CreateCriteria(typeof(BlockGroup), "bg").CreateCriteria("BlockGroupSelectMappings", "bgsm")
               .CreateCriteria("Tract", "tra").Add(Restrictions.Eq("tra.Id", tractId));

            return criteria.List<BlockGroup>();
        }

        public IEnumerable<BlockGroup> GetBGByPremiumCRoute(int croutId) {
            ICriteria criteria = InternalSession.CreateCriteria(typeof(FiveZipArea), "fza").CreateCriteria("PremiumCRouteSelectMappings", "pcrm").Add(Restrictions.Eq("pcrm.Id", croutId));
            IEnumerable<FiveZipArea> fiveList = criteria.List<FiveZipArea>();

            ICriteria criteriaFive = null;
            IEnumerable<BlockGroup> list = null;
            List<int> fiveListIds = new List<int>();
            foreach (var fza in fiveList)
            {
                fiveListIds.Add(fza.Id);
              }
            criteriaFive = InternalSession.CreateCriteria(typeof(BlockGroup), "bg").CreateCriteria("BlockGroupSelectMappings", "bgsm").CreateCriteria("FiveZipArea", "fza").Add(Restrictions.In("fza.Id", fiveListIds.ToArray()));
            list = criteriaFive.List<BlockGroup>();

            
            
            return criteriaFive.List<BlockGroup>();
        }


        public IEnumerable<FiveZipArea> GetFiveZipsSatisfyingSubMap(FiveZipsSatisfyingSubMapSpecification spec)
        {
            //const string queryFormatAllFIds = " select distinct pcrsm.FiveZipAreaId from PremiumCRouteSelectMapping pcrsm where"
            //    + " pcrsm.PremiumCRouteId in (:selectedCRouteIds) or"
            //    + " (pcrsm.FiveZipAreaId in (:selectedFiveZipIds) and pcrsm.PremiumCRouteId not in (:deselectedCRouteIds)))";
            //IList<Int32> allFIds = InternalSession.CreateQuery(queryFormatAllFIds)
            //    .SetParameterList("selectedFiveZipIds", spec.SelectedFiveZipIds.ToArray())
            //    .SetParameterList("selectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .SetParameterList("deselectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .List<Int32>();

            //const string queryFormatNonFIds = " select distinct pcrsm.FiveZipAreaId from PremiumCRouteSelectMapping pcrsm where"
            //    + " pcrsm.FiveZipAreaId in (:allFiveZipIds) and"
            //    + " (not (pcrsm.PremiumCRouteId in (:selectedCRouteIds) or"
            //    + " (pcrsm.FiveZipAreaId in (:selectedFiveZipIds) and pcrsm.PremiumCRouteId not in (:deselectedCRouteIds))))";
            //IList<Int32> nonFIds = InternalSession.CreateQuery(queryFormatNonFIds)
            //    .SetParameterList("allFiveZipIds", allFIds.ToArray())
            //    .SetParameterList("selectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .SetParameterList("selectedFiveZipIds", spec.SelectedFiveZipIds.ToArray())
            //    .SetParameterList("deselectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .List<Int32>();
            //if (nonFIds.Count == 0) nonFIds.Add(-1);

            //const string queryFormat =
            //      " select distinct fza from FiveZipArea fza join fza.PremiumCRouteSelectMappings pcrsm where"
            //    + " (fza.Id in (:allFiveZipIds) and fza.Id not in (:nonFiveZipIds))";
            //return InternalSession.CreateQuery(queryFormat)
            //    .SetParameterList("allFiveZipIds", allFIds.ToArray())
            //    .SetParameterList("nonFiveZipIds", nonFIds.ToArray())
            //    .List<FiveZipArea>();

            const string queryFormat =
                  " select distinct fza from FiveZipArea fza  where"
                + " fza.Id in (:allFiveZipIds)";
            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("allFiveZipIds", spec.SelectedFiveZipIds.ToArray())
                .List<FiveZipArea>().OrderBy(f => f.Code);
        }


        public IEnumerable<FiveZipArea> GetFiveZipsSatisfyingDistributionMap(FiveZipsSatisfyingDistributionMapSpecification spec)
        {
            //const string queryFormatAllFIds = " select distinct pcrsm.FiveZipAreaId from PremiumCRouteSelectMapping pcrsm where"
            //    + " pcrsm.PremiumCRouteId in (:selectedCRouteIds) or"
            //    + " (pcrsm.FiveZipAreaId in (:selectedFiveZipIds) and pcrsm.PremiumCRouteId not in (:deselectedCRouteIds)))";
            //IList<Int32> allFIds = InternalSession.CreateQuery(queryFormatAllFIds)
            //    .SetParameterList("selectedFiveZipIds", spec.SelectedFiveZipIds.ToArray())
            //    .SetParameterList("selectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .SetParameterList("deselectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .List<Int32>();

            //const string queryFormatNonFIds = " select distinct pcrsm.FiveZipAreaId from PremiumCRouteSelectMapping pcrsm where"
            //    + " pcrsm.FiveZipAreaId in (:allFiveZipIds) and"
            //    + " (not (pcrsm.PremiumCRouteId in (:selectedCRouteIds) or"
            //    + " (pcrsm.FiveZipAreaId in (:selectedFiveZipIds) and pcrsm.PremiumCRouteId not in (:deselectedCRouteIds))))";
            //IList<Int32> nonFIds = InternalSession.CreateQuery(queryFormatNonFIds)
            //    .SetParameterList("allFiveZipIds", allFIds.ToArray())
            //    .SetParameterList("selectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .SetParameterList("selectedFiveZipIds", spec.SelectedFiveZipIds.ToArray())
            //    .SetParameterList("deselectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .List<Int32>();
            //if (nonFIds.Count == 0) nonFIds.Add(-1);

            //const string queryFormat =
            //      " select distinct fza from FiveZipArea fza join fza.PremiumCRouteSelectMappings pcrsm where"
            //    + " (fza.Id in (:allFiveZipIds) and fza.Id not in (:nonFiveZipIds))";
            //return InternalSession.CreateQuery(queryFormat)
            //    .SetParameterList("allFiveZipIds", allFIds.ToArray())
            //    .SetParameterList("nonFiveZipIds", nonFIds.ToArray())
            //    .List<FiveZipArea>();


            return new List<FiveZipArea>();
        }

    }
}
