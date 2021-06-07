using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Distribution
{
    /// <summary>
    /// Provide methods used to create new instances of <see cref="DistributionJob"/> 
    /// related classes, which.makes these classes work as an aggregation, as well as
    /// ensure consistence and integrity of object states.
    /// </summary>
    public class DistributionJobFactory
    {
        /// <summary>
        /// Create a new <see cref="DistributionJob"/> for specified <see cref="Campaign"/>
        /// with the specified job name.
        /// </summary>
        /// <param name="name">The Name of the distribution job, required.</param>
        /// <param name="campaign">The <see cref="Campaign"/> for which the distribution 
        /// job is created.</param>
        /// <returns>A <see cref="DistributionJob"/> instance.</returns>
        public static DistributionJob CreateDistributionJob(Campaign campaign, String name)
        {
            return new DistributionJob(Guid.NewGuid().GetHashCode(), name, campaign);
        }

        /// <summary>
        /// Create a new <see cref="Auditor"/> from a <see cref="User"/> for the specified
        /// <see cref="DistributionJob"/>.
        /// </summary>
        /// <param name="dj">The distribution job.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static AuditorAssignment CreateAuditorFromUser(DistributionJob dj, User user)
        {
            return new AuditorAssignment(dj, user);
        }

        /// <summary>
        /// Create a new <see cref="Driver"/> from a <see cref="User"/> for the specified
        /// <see cref="DistributionJob"/>.
        /// </summary>
        /// <param name="dj">The distribution job.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static DriverAssignment CreateDriverFromUser(DistributionJob dj, User user)
        {
            return new DriverAssignment(dj, user);
        }

        /// <summary>
        /// Create a new <see cref="Driver"/> for the specified <see cref="DistributionJob"/>.
        /// </summary>
        /// <param name="dj">The distribution job.</param>
        /// <param name="fullName">The Full Name of the driver.</param>
        /// <returns></returns>
        public static DriverAssignment CreateDriver(DistributionJob dj, String fullName)
        {
            return new DriverAssignment(dj, fullName);
        }

        /// <summary>
        /// Create a new <see cref="Walker"/> from a <see cref="User"/> for the specified
        /// <see cref="DistributionJob"/>.
        /// </summary>
        /// <param name="dj">The distribution job.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static WalkerAssignment CreateWalkerFromUser(DistributionJob dj, User user)
        {
            return new WalkerAssignment(dj, user);
        }

        /// <summary>
        /// Create a new <see cref="Walker"/> for the specified <see cref="DistributionJob"/>.
        /// </summary>
        /// <param name="dj">The distribution job.</param>
        /// <param name="fullName">The Full Name of the walker.</param>
        /// <returns></returns>
        public static WalkerAssignment CreateWalker(DistributionJob dj, String fullName)
        {
            return new WalkerAssignment(dj, fullName);
        }
    }
}
