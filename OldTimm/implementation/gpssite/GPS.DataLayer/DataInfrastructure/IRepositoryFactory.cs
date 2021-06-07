using System;
using GPS.DataLayer;
namespace GPS.DataLayer.DataInfrastructure
{
    public interface IRepositoryFactory
    {
        IAddressRepository AddressRepository { get; }
        IBlockGroupRepository BlockGroupRepository { get; }
        ICampaignClassificationRepository CampaignClassificationRepository { get; }
        ICampaignPercentageColorRepository CampaignPercentageColorRepository { get; }
        ICampaignRecordRepository CampaignRecordRepository { get; }
        ISubMapRepository SubMapRepository { get; }
        ICampaignRepository CampaignRepository { get; }
        ICbsaBoxMappingRepository CbsaBoxMappingRepository { get; }
        ICountyAreaBoxMappingRepository CountyAreaBoxMappingRepository { get; }
        ICustomAreaRepository CustomAreaRepository { get; }
        IDistributionJobRepository DistributionJobRepository { get; }
        IDistributionMapRepository DistributionMapRepository { get; }
        IElementarySchoolBoxMappingRepository ElementarySchoolBoxMappingRepository { get; }
        IFiveZipRepository FiveZipRepository { get; }
        IGtuRepository GtuRepository { get; }
        ILowerHouseBoxMappingRepository LowerHouseBoxMappingRepository { get; }
        INdAddressRepository NdAddressRepository { get; }
        IPremiumCRouteRepository PremiumCRouteRepository { get; }
        IRadiusRepository RadiusRepository { get; }
        ISecondarySchoolBoxMappingRepository SecondarySchoolBoxMappingRepository { get; }
        IThreeZipRepository ThreeZipRepository { get; }
        ITractRepository TractRepository { get; }
        IUnifiedSchoolBoxMappingRepository UnifiedSchoolBoxMappingRepository { get; }
        IUpperSenateBoxMappingRepository UpperSenateBoxMappingRepository { get; }
        IUrbanBoxMappingRepository UrbanBoxMappingRepository { get; }
        IUserRepository UserRepository { get; }
        IGroupRepository GroupRepository { get; }
        IPrivilegeRepository PrivilegeRepository { get; }
        ITaskRepository TaskRepository { get; }
        ICampaignBackupRepository CampaignBackupRepository { get; }
    }
}
