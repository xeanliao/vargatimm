using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DataLayer.ValueObjects;

namespace GPS.DomainLayer.Entities
{
    public class AuditorAssignment : DjParticipantAssignment
    {
        protected AuditorAssignment() { }

        public AuditorAssignment(DistributionJob dj, User user)
            : base(Guid.NewGuid().GetHashCode(), UserRoles.Auditor, dj)
        {
            this.LoginUser = user;
        }
    }
}
