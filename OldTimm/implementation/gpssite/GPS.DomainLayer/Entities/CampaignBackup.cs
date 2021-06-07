using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class CampaignBackup
    {
        public CampaignBackup()
        { }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string UserName { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual string CustemerName { get; set; }

        public virtual double Latitude { get; set; }

        public virtual double Longitude { get; set; }

        public virtual int ZoomLevel { get; set; }

        public virtual int Sequence { get; set; }

        public virtual string ContactName { get; set; }

        public virtual string ClientCode { get; set; }

        public virtual string Logo { get; set; }

        public virtual string AreaDescription { get; set; }

        public virtual string ClientName { get; set; }


        public virtual string IPAddress { get; set; }

        public virtual DateTime OperationTime { get; set; }

        public virtual string OperationUser { get; set; }
    }
}
