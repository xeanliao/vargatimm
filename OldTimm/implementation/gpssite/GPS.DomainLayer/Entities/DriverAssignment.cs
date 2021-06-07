using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    public class DriverAssignment : DjParticipantAssignment
    {
        protected DriverAssignment() { }

        public DriverAssignment(DistributionJob dj, User user)
            : base(Guid.NewGuid().GetHashCode(), UserRoles.Driver, dj) { this.LoginUser = user; }

        public DriverAssignment(DistributionJob dj, String fullName)
            : base(Guid.NewGuid().GetHashCode(), UserRoles.Driver, dj) { FullName = fullName; }
    }
}
