using System; 
using System.Collections.Generic; 
using System.Text;
using System.Runtime.Serialization;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities {
    [Serializable]
    public class Radiuse : IRadius
    {
        #region Basic properties
        public virtual int Id { get; set; }
        public virtual int AddressId
        {
            get { return Address.Id; }
            set
            {
                Address.Id = value;
            }
        }
        public virtual Address Address { get; set; }
        public virtual bool IsDisplay { get; set; }
        public virtual double Length { get; set; }
        public virtual int LengthMeasuresId { get; set; }
        #endregion

        #region Children
        public virtual IList<RadiusRecord> RadiusRecords { get; set; }
        #endregion
    }
}
