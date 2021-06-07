using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.RepositoryInterfaces.BulkRepositories
{
    public interface IBulkCampaignBGRepository
    {
        void Add(CampaignBlockGroupImported campaignBlockGroupImported);
        void Update(CampaignBlockGroupImported campaignBlockGroupImported);
        void DeleteAllCampaignBlockGroupImportedsOfCampaign(Int32 campaignId);
    }
}
