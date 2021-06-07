using System; 
using System.Collections.Generic; 
using System.Text; 


namespace GPS.DomainLayer.Entities {
    
    public class NdAddress {
        public NdAddress() {
        }
        public virtual string Description {
            get;
            set;
        }
        public virtual int Geofence {
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
        public virtual string Street {
            get;
            set;
        }
        public virtual string ZipCode {
            get;
            set;
        }

        #region children objects

        public virtual IList<NdAddressBoxMapping> NdAddressBoxMappings { get; set; }

        public virtual IList<NdAddressCoordinate> NdAddressCoordinates { get; set; }

        #endregion
    }
}
