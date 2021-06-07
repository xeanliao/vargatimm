using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using System.Data.Linq;
using GPS.DomainLayer.Interfaces;
using GPS.DataLayer.ValueObjects;
using NHibernate;
using NHibernate.Linq;
using GPS.DomainLayer.Entities;
using NHibernate.Criterion;

namespace GPS.DataLayer
{
    /// <summary>
    /// Class <see cref="CampaignBackupRepository"/> is responsible for reading and 
    /// writing CampaignBackup entities into the database.
    /// </summary>
    public class CampaignBackupRepository : RepositoryBase, GPS.DataLayer.ICampaignBackupRepository
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CampaignBackupRepository() { }

        public CampaignBackupRepository(ISession session) : base(session) { }

        /// <summary>
        /// Add specified CampaignBackup to the database.
        /// </summary>
        /// <param name="CampaignBackup">The CampaignBackup to add.</param>
        public void Create(CampaignBackup CampaignBackup)
        {
            base.Insert(CampaignBackup);
        }

     
    }
}
