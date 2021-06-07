using System; 
using System.Collections.Generic; 
using System.Text;
using GPS.DomainLayer.Enum; 

namespace GPS.DomainLayer.Entities 
{
    public class SubMap 
    {
        public virtual int Id
        {
            get;
            set;
        }
        public virtual int CampaignId
        {
            get { return Campaign.Id; }
            set
            {
            	Campaign.Id = value;
            }
        }

        public virtual Campaign Campaign { get; set; }

        public virtual int ColorB {
            get;
            set;
        }
        public virtual int ColorG {
            get;
            set;
        }
        public virtual int ColorR {
            get;
            set;
        }
        public virtual string ColorString {
            get;
            set;
        }
        public virtual string Name {
            get;
            set;
        }
        public virtual int OrderId {
            get;
            set;
        }
        public virtual int Penetration {
            get;
            set;
        }
        public virtual double Percentage {
            get;
            set;
        }
        public virtual int Total {
            get;
            set;
        }
        public virtual int TotalAdjustment
        {
            get;
            set;
        }
        public virtual int CountAdjustment
        {
            get;
            set;
        }
        public virtual List<double[][]> Holes { get; set; }
        #region children objects

        public virtual IList<SubMapCoordinate> SubMapCoordinates { get; set; }

        public virtual IList<SubMapRecord> SubMapRecords { get; set; }

        public virtual IList<DistributionMap> DistributionMaps { get; set; }

        #endregion
    }
}
