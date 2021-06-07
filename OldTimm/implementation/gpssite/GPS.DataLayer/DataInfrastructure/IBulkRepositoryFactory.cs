using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.RepositoryInterfaces.BulkRepositories;

namespace GPS.DataLayer.DataInfrastructure
{
    public interface IBulkRepositoryFactory
    {
        IBulkCampaignBGRepository BulkCampaignBGRepository { get; }
        IBulkCampaignCRouteRepository BulkCampaignCRouteRepository { get; }
        IBulkRadiusRecordRepository BulkRadiusRecordRepository { get; }
        IBulkSubMapCoordinateRepository BulkSubMapCoordinateRepository { get; }
        IBulkDistributionMapCoordinateRepository BulkDistributionMapCoordinateRepository { get; }
    }
}
