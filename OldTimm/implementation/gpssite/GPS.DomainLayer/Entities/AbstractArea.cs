using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public abstract class AbstractArea
    {
        #region Interface
        
        public virtual Int32 Id 
        { 
            get { return _id; } 
            set { _id = value; } 
        }
        
        public virtual String Code 
        { 
            get { return _code; } 
            set { _code = value; } 
        }
        
        public virtual Boolean IsEnabled 
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public virtual Int32 IsInnerShape 
        {
            get { return _isInnerShape; }
            set { _isInnerShape = value; }
        }
        
        public virtual Int32 PartCount 
        {
            get { return _partCount; }
            set { _partCount = value; }
        }
        
        public virtual String ArbitraryUniqueCode 
        {
            get { return _arbitraryUniqueCode; }
            set { _arbitraryUniqueCode = value; }
        }

        public virtual Boolean HasMultipleParts
        {
            get { return PartCount > 1; }
        }

        public virtual Int32 IsInnerRing
        {
            get { return _isInnerRing; }
            set { _isInnerRing = value; }
        }

        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual double MaxLatitude { get; set; }
        public virtual double MaxLongitude { get; set; }
        public virtual double MinLatitude { get; set; }
        public virtual double MinLongitude { get; set; }

        #endregion

        #region Implementations
        private Int32 _id = 0;
        private String _code = string.Empty;
        private Boolean _isEnabled = true;
        private Int32 _isInnerShape = 0;
        private Int32 _isInnerRing = 0;
        private Int32 _partCount = 1;
        private String _arbitraryUniqueCode = string.Empty;
        #endregion
    }
}
