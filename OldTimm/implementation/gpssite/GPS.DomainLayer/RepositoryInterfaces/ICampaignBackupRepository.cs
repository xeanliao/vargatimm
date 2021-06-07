using System;
namespace GPS.DataLayer
{
    public interface ICampaignBackupRepository
    {
        void Create(GPS.DomainLayer.Entities.CampaignBackup campaign);      
    }
}