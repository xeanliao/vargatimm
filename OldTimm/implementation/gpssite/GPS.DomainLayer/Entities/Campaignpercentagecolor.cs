using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class CampaignPercentageColor
    {
        public CampaignPercentageColor()
        {
        }
        public virtual int CampaignId
        {
            get { return Campaign.Id; }
            set { Campaign.Id = value; }
        }
        public virtual Campaign Campaign{get;set;}
        public virtual int ColorId
        {
            get;
            set;
        }
        public virtual int Id
        {
            get;
            set;
        }
        public virtual double Max
        {
            get;
            set;
        }
        public virtual double Min
        {
            get;
            set;
        }
    }
}
