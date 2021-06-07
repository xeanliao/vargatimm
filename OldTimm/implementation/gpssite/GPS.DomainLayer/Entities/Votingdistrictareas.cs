using System; 
using System.Collections.Generic; 
using System.Text; 


namespace GPS.DomainLayer.Entities {
    
    public class VotingDistrictArea {
        public VotingDistrictArea() {
        }
        public virtual string Code {
            get;
            set;
        }
        public virtual string CountyCode {
            get;
            set;
        }
        public virtual int Id {
            get;
            set;
        }
        public virtual double Latitude {
            get;
            set;
        }
        public virtual double Longitude {
            get;
            set;
        }
        public virtual string LSAD {
            get;
            set;
        }
        public virtual string LSAD_TRAN {
            get;
            set;
        }
        public virtual double MaxLatitude {
            get;
            set;
        }
        public virtual double MaxLongitude {
            get;
            set;
        }
        public virtual double MinLatitude {
            get;
            set;
        }
        public virtual double MinLongitude {
            get;
            set;
        }
        public virtual string Name {
            get;
            set;
        }
        public virtual string StateCode {
            get;
            set;
        }
        public virtual string VTD {
            get;
            set;
        }
    }
}
