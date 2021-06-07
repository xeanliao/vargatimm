using System; 
using System.Collections.Generic; 
using System.Text;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities {

    public class ThreeZipArea : IBoxArea
    {
        public ThreeZipArea() {
        }
        public virtual string Code {
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
        public virtual string LSADTrans {
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


        public virtual int APT_COUNT { get; set; }
        public virtual int BUSINESS_COUNT { get; set; }
        public virtual int HOME_COUNT { get; set; }
        public virtual int TOTAL_COUNT { get; set; }

        #region children objects

        public virtual IList<ThreeZipAreaCoordinate> ThreeZipAreaCoordinates { get; set; }

        public virtual IList<ThreeZipBoxMapping> ThreeZipBoxMappings { get; set; }

        #endregion

        #region extended properties
        /// <summary>
        /// population total
        /// </summary>
        public virtual string Total { get; set; }

        /// <summary>
        /// percentage of penetration
        /// </summary>
        public virtual double Percentage { get; set; }
        #endregion
    }
}
