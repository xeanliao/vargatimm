using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;

namespace GPS.DomainLayer.Entities
{
    public class RadiusRecord
    {
        public virtual Int32 Id { get; set; }
        public virtual Int32 RadiusId
        {
            get { return Radiuse.Id; }
            set
            {
                Radiuse.Id = value;
            }
        }
        public virtual Radiuse Radiuse { get; set; }
        public virtual Int32 AreaId { get; set; }
        public virtual Classifications Classification { get; set; }
        
    }
}
