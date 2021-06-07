using System; 
using System.Collections.Generic; 
using System.Text; 


namespace GPS.DomainLayer.Entities {
    
    public class CustomArea {
        public CustomArea() {
        }
        public virtual string Description {
            get;
            set;
        }
        public virtual int Id {
            get;
            set;
        }
        public virtual bool IsEnabled {
            get;
            set;
        }
        public virtual string Name {
            get;
            set;
        }
        public virtual int total {
            get;
            set;
        }

        #region children objects

        public virtual IList<CustomAreaCoordinate> CustomAreaCoordinates
        {
            get;
            set;
        }

        public virtual IList<CustomAreaBoxMapping> CustomAreaBoxMappings
        {
            get;
            set;
        }

        #endregion
    }
}
