using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;
using GPS.DataLayer.DataInfrastructure;
using GPS.DataLayer.RepositoryImplementations.BulkRepository;

namespace GPS.DataLayer.DataInfrastructure.Implementations
{
    internal class BulkRepositoryFactory : IBulkRepositoryFactory
    {
        #region Implementations
        private NHibernate.IStatelessSession _session;

        private RepositoryType GetRepository<RepositoryType>(NHibernate.IStatelessSession session, RepositoryType rep)
        {
            IBulkRepository r = rep as IBulkRepository;
            r.Session = session;
            return rep;
        }
        #endregion
        
        #region Constructors
        public BulkRepositoryFactory(NHibernate.IStatelessSession session)
        {
            _session = session;
        }
        #endregion

        #region IBulkRepositoryFactory Members

        IBulkCampaignBGRepository IBulkRepositoryFactory.BulkCampaignBGRepository
        {
            get { return GetRepository<BulkCampaignBGRepository>(_session, new BulkCampaignBGRepository()); }
        }
        IBulkRadiusRecordRepository IBulkRepositoryFactory.BulkRadiusRecordRepository
        {
            get { return GetRepository<BulkRadiusRecordRepository>(_session, new BulkRadiusRecordRepository()); }
        }
        public IBulkSubMapCoordinateRepository BulkSubMapCoordinateRepository
        {
            get { return GetRepository<BulkSubMapCoordinateRepository>(_session, new BulkSubMapCoordinateRepository()); }
        }

        public IBulkDistributionMapCoordinateRepository BulkDistributionMapCoordinateRepository
        {
            get { return GetRepository<BulkDistributionMapCoordinateRepository>(_session, new BulkDistributionMapCoordinateRepository()); }
        }

        public IBulkCampaignCRouteRepository BulkCampaignCRouteRepository
        {
            get { return GetRepository<BulkCampaignCRouteRepository>(_session, new BulkCampaignCRouteRepository()); }
        }

        #endregion
    }
}
