using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer.RepositoryInterfaces;
using GPS.DataLayer.RepositoryImplementations;

namespace GPS.DataLayer.DataInfrastructure.Implementations
{
    internal class RepositoryFactory : IRepositoryFactory
    {
        #region Implementations
        private NHibernate.ISession _session;

        private RepositoryType GetRepository<RepositoryType>(NHibernate.ISession session, RepositoryType rep)
        {
            IRepository r = rep as IRepository;
            r.Session = session;
            return rep;
        }
        #endregion

        #region Constructors
        public RepositoryFactory(NHibernate.ISession session)
        {
            _session = session;
        }
        #endregion

        #region Repository instances
        public IAddressRepository AddressRepository 
        {
            get { return GetRepository<AddressRepository>(_session, new AddressRepository()); }
        }
        public IBlockGroupRepository BlockGroupRepository
        {
            get { return GetRepository<BlockGroupRepository>(_session, new BlockGroupRepository()); }
        }
        public ICampaignClassificationRepository CampaignClassificationRepository
        {
            get { return GetRepository<CampaignClassificationRepository>(_session, new CampaignClassificationRepository()); }
        }
        public ICampaignPercentageColorRepository CampaignPercentageColorRepository
        {
            get { return GetRepository<CampaignPercentageColorRepository>(_session, new CampaignPercentageColorRepository()); }
        }
        public ICampaignRecordRepository CampaignRecordRepository
        {
            get { return GetRepository<CampaignRecordRepository>(_session, new CampaignRecordRepository()); }
        }
        public ICampaignRepository CampaignRepository
        {
            get { return GetRepository<CampaignRepository>(_session, new CampaignRepository()); }
        }

        public ISubMapRepository SubMapRepository
        {
            get { return GetRepository<SubMapRepository>(_session, new SubMapRepository()); }
        }
        public ICbsaBoxMappingRepository CbsaBoxMappingRepository
        {
            get { return GetRepository<CbsaBoxMappingRepository>(_session, new CbsaBoxMappingRepository()); }
        }
        public ICountyAreaBoxMappingRepository CountyAreaBoxMappingRepository
        {
            get { return GetRepository<CountyAreaBoxMappingRepository>(_session, new CountyAreaBoxMappingRepository()); }
        }
        public ICustomAreaRepository CustomAreaRepository
        {
            get { return GetRepository<CustomAreaRepository>(_session, new CustomAreaRepository()); }
        }
        public IDistributionJobRepository DistributionJobRepository
        {
            get { return GetRepository<DistributionJobRepository>(_session, new DistributionJobRepository()); }
        }
        public IDistributionMapRepository DistributionMapRepository
        {
            get { return GetRepository<DistributionMapRepository>(_session, new DistributionMapRepository()); }
        }
        public IElementarySchoolBoxMappingRepository ElementarySchoolBoxMappingRepository
        {
            get { return GetRepository<ElementarySchoolBoxMappingRepository>(_session, new ElementarySchoolBoxMappingRepository()); }
        }
        public IFiveZipRepository FiveZipRepository
        {
            get { return GetRepository<FiveZipRepository>(_session, new FiveZipRepository()); }
        }
        public IGtuRepository GtuRepository
        {
            get { return GetRepository<GtuRepository>(_session, new GtuRepository()); }
        }
        public ILowerHouseBoxMappingRepository LowerHouseBoxMappingRepository
        {
            get { return GetRepository<LowerHouseBoxMappingRepository>(_session, new LowerHouseBoxMappingRepository()); }
        }
        public INdAddressRepository NdAddressRepository
        {
            get { return GetRepository<NdAddressRepository>(_session, new NdAddressRepository()); }
        }
        public IPremiumCRouteRepository PremiumCRouteRepository
        {
            get { return GetRepository<PremiumCRouteRepository>(_session, new PremiumCRouteRepository()); }
        }
        public IRadiusRepository RadiusRepository
        {
            get { return GetRepository<RadiusRepository>(_session, new RadiusRepository()); }
        }
        public ISecondarySchoolBoxMappingRepository SecondarySchoolBoxMappingRepository
        {
            get { return GetRepository<SecondarySchoolBoxMappingRepository>(_session, new SecondarySchoolBoxMappingRepository()); }
        }
        public IThreeZipRepository ThreeZipRepository
        {
            get { return GetRepository<ThreeZipRepository>(_session, new ThreeZipRepository()); }
        }
        public ITractRepository TractRepository
        {
            get { return GetRepository<TractRepository>(_session, new TractRepository()); }
        }
        public IUnifiedSchoolBoxMappingRepository UnifiedSchoolBoxMappingRepository
        {
            get { return GetRepository<UnifiedSchoolBoxMappingRepository>(_session, new UnifiedSchoolBoxMappingRepository()); }
        }
        public IUpperSenateBoxMappingRepository UpperSenateBoxMappingRepository
        {
            get { return GetRepository<UpperSenateBoxMappingRepository>(_session, new UpperSenateBoxMappingRepository()); }
        }
        public IUrbanBoxMappingRepository UrbanBoxMappingRepository
        {
            get { return GetRepository<UrbanBoxMappingRepository>(_session, new UrbanBoxMappingRepository()); }
        }
        public IUserRepository UserRepository
        {
            get { return GetRepository<UserRepository>(_session, new UserRepository()); }
        }
        public IGroupRepository GroupRepository
        {
            get { return GetRepository<GroupRepository>(_session, new GroupRepository()); }
        }

        public IPrivilegeRepository PrivilegeRepository
        {
            get { return GetRepository<PrivilegeRepository>(_session, new PrivilegeRepository()); }
        }

        public ITaskRepository TaskRepository
        {
            get { return GetRepository<TaskRepository>(_session, new TaskRepository()); }
        }

        public ICampaignBackupRepository CampaignBackupRepository
        {
            get { return GetRepository<CampaignBackupRepository>(_session, new CampaignBackupRepository()); }
        }
        #endregion
    }
}
