using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.Website.TransferObjects;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToDriver
    {
        #region properties
        [DataMember]
        public int DjRole
        {
            get;
            set;
        }

        [DataMember]
        public String DjRoleName
        {
            get;
            set;
        }
       
        [DataMember]
        public int DistributionJobId
        {
            get;
            set;
        }

        [DataMember]
        public String FullName
        {
            get;
            set;
        }

        [DataMember]
        public String UniqueID
        {
            get;
            set;
        }

        [DataMember]
        public int LoginUserId
        {
            get;
            set;
        }
        #endregion


        /// <summary>
        /// Used by the Otis transformer to transform DjRole, DistributionJobId,GtuId and LoginUserId properties.
        /// </summary>
        /// <param name="target">A <see cref="ToDriver"/>, the DTO.</param>
        /// <param name="source">A <see cref="Driver"/>, the source.</param>
        public static void Convert(ref ToDriver target, ref DriverAssignment source)
        {
            target.DjRole = (int)source.DjRole;
            target.DjRoleName = source.DjRole.ToString();

            if (null != source.DistributionJob)
            {
                target.DistributionJobId = source.DistributionJob.Id;
            }
            else
            {
                target.DistributionJobId = 0;
            }

            if (null != source.Gtu)
            {
                target.UniqueID = source.Gtu.UniqueID;
            }
            else
            {
                target.UniqueID = String.Empty;
            }

            if (null != source.LoginUser)
            {
                target.LoginUserId = source.LoginUser.Id;
            }
            else
            {
                target.LoginUserId = 0;
            }
        }
    }
}
