using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.QuerySpecifications;

namespace GPS.DataLayer
{
    public class PremiumCRouteRepository : RepositoryBase, IExportSourceRepository<PremiumCRoute>, GPS.DataLayer.IPremiumCRouteRepository
    {
        public PremiumCRouteRepository() { }

        public PremiumCRouteRepository(ISession session) : base(session) { }

        public List<PremiumCRoute> GetPremiumCRoutes(int boxId)
        {
            return InternalSession.Linq<PremiumCRouteBoxMapping>().Where(t => t.BoxId == boxId).Select(t => t.PremiumCRoute).Distinct().ToList();
        }

        public IEnumerable<PremiumCRoute> GetBoxItems(int boxId)
        {
            const string queryFormat = "select distinct t from PremiumCRoute t join t.PremiumCRouteBoxMappings tbm where tbm.BoxId = :boxId and t.IsInnerRing=0";
            return InternalSession.CreateQuery(queryFormat).SetInt32("boxId", boxId).List<PremiumCRoute>();
        }

        public IEnumerable<PremiumCRoute> GetItemsByCode(string code)
        {
            const string queryFormat = "select distinct pcr from PremiumCRoute pcr where pcr.Code = :code and pcr.IsInnerRing=0";

            return InternalSession.CreateQuery(queryFormat).SetString("code", code).List<PremiumCRoute>();
        }

        public PremiumCRoute GetItem(int id)
        {
            return InternalSession.Get<PremiumCRoute>(id);
        }

        public IEnumerable<PremiumCRoute> GetBoxItems(IEnumerable<int> boxIds)
        {
            IList<PremiumCRoute> items = new List<PremiumCRoute>();

            if (null != boxIds && boxIds.Count() > 0)
            {
                const string queryFormat = "select distinct pcrbm.PremiumCRoute from PremiumCRouteBoxMapping pcrbm where pcrbm.BoxId in (:boxIds)";
                items = InternalSession.CreateQuery(queryFormat).SetParameterList("boxIds", boxIds.ToArray()).List<PremiumCRoute>();
            }

            return items;
        }

        #region IExportSourceProvider Members

        IEnumerable<PremiumCRoute> IExportSourceRepository<PremiumCRoute>.GetExportSource(IExportSourceSpecification spec)
        {
            const string queryFormat =
                  " select distinct cr from PremiumCRoute cr join cr.PremiumCRouteSelectMappings pcrsm where "
                + " pcrsm.PremiumCRouteId in (:inCRIds)"
                + " or pcrsm.FiveZipAreaId in (:inFiveZipIds) and pcrsm.PremiumCRouteId not in (:nonCRIds)"
                + " or pcrsm.ThreeZipAreaId in (:inThreeZipIds) and pcrsm.FiveZipAreaId not in (:nonFiveZipIds)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("inCRIds", spec.SelectedCRouteIds.ToArray())
                .SetParameterList("nonCRIds", spec.DeselectedCRouteIds.ToArray())
                .SetParameterList("inFiveZipIds", spec.SelectedFiveZipIds.ToArray())
                .SetParameterList("inThreeZipIds", spec.SelectedThreeZipIds.ToArray())
                .SetParameterList("nonFiveZipIds", spec.DeselectedFiveZipIds.ToArray())
                .List<PremiumCRoute>();
        }

        #endregion

        #region IImportedDataRepository<PremiumCRoute> Members

        public IEnumerable<PremiumCRoute> GetShapesAccordingToCodes(string[] geocodes)
        {
            const string queryFormat =
                "select distinct cr from PremiumCRoute cr where cr.Code in (:codes)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("codes", geocodes)
                .List<PremiumCRoute>();
        }

        #endregion

        public IEnumerable<PremiumCRoute> GetPremiumCRoutesSatisfyingSubMap(PremiumCRoutesSatisfyingSubMapSpecification spec)
        {
            #region modified by steve.j.yin on 2013/7/16 for remove PremiumCRouteSelectMappings check
            //const string queryFormat =
            //      " select distinct cr from PremiumCRoute cr join cr.PremiumCRouteSelectMappings pcrsm where"
            //    + " (cr.Id in (:selectedCRouteIds) or"
            //    + " (pcrsm.FiveZipAreaId in (:selectedFiveZipIds) and cr.Id not in (:deselectedCRouteIds))) and"
            //    + " pcrsm.FiveZipAreaId not in (:deselectedFiveZipIds)";

            //return InternalSession.CreateQuery(queryFormat)
            //    .SetParameterList("selectedFiveZipIds", spec.SelectedFiveZipIds.ToArray())
            //    .SetParameterList("deselectedFiveZipIds", spec.DeselectedFiveZipIds.ToArray())
            //    .SetParameterList("selectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .SetParameterList("deselectedCRouteIds", spec.DeselectedCRouteIds.ToArray())
            //    .List<PremiumCRoute>().OrderBy(p=>p.Code);

            const string queryFormat =
                  " select distinct cr from PremiumCRoute cr where"
                + " cr.Id in (:selectedCRouteIds) and"
                + " cr.Id not in (:deselectedCRouteIds)";

            return InternalSession.CreateQuery(queryFormat)
                .SetParameterList("selectedCRouteIds", spec.SelectedCRouteIds.ToArray())
                .SetParameterList("deselectedCRouteIds", spec.DeselectedCRouteIds.ToArray())
                .List<PremiumCRoute>().OrderBy(p => p.Code);

            #endregion
        }


        public IEnumerable<PremiumCRoute> GetPremiumCRoutesSatisfyingDistributionMap(PremiumCRoutesSatisfyingDistributionMapSpecification spec)
        {
            //const string queryFormat =
            //      " select distinct cr from PremiumCRoute cr join cr.PremiumCRouteSelectMappings pcrsm where"
            //    + " (cr.Id in (:selectedCRouteIds) or"
            //    + " (pcrsm.FiveZipAreaId in (:selectedFiveZipIds) and cr.Id not in (:deselectedCRouteIds))) and"
            //    + " pcrsm.FiveZipAreaId not in (:deselectedFiveZipIds)";

            //return InternalSession.CreateQuery(queryFormat)
            //    .SetParameterList("selectedFiveZipIds", spec.SelectedFiveZipIds.ToArray())
            //    .SetParameterList("deselectedFiveZipIds", spec.DeselectedFiveZipIds.ToArray())
            //    .SetParameterList("selectedCRouteIds", spec.SelectedCRouteIds.ToArray())
            //    .SetParameterList("deselectedCRouteIds", spec.DeselectedCRouteIds.ToArray())
            //    .List<PremiumCRoute>();
            return new List<PremiumCRoute>();
        }
    }
}
