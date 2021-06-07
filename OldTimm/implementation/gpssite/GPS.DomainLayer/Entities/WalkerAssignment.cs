using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    public class WalkerAssignment : DjParticipantAssignment
    {
        protected WalkerAssignment() { }

        public WalkerAssignment(DistributionJob dj, User user)
            : base(Guid.NewGuid().GetHashCode(), UserRoles.Walker, dj)
        {
            this.LoginUser = user;
        }

        public WalkerAssignment(DistributionJob dj, String fullName)
            : base(Guid.NewGuid().GetHashCode(), UserRoles.Walker, dj) { FullName = fullName; }
    }
}
