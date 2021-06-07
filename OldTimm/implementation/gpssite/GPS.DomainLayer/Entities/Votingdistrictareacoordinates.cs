using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class VotingDistrictAreaCoordinate : AbstractAreaCoordinate
    {
        public virtual int VotingDistrictAreaId { get; set; }
    }
}
