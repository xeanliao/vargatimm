using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TrackServices")]
    public class ToGtu
    {
        [DataMember]
        public int Id
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
        public String Model
        {
            get;
            set;
        }
        [DataMember]
        public Boolean IsEnabled
        {
            get;
            set;
        }

        [DataMember]
        public int UserId
        {
            get;
            set;
        }
        [DataMember]
        public String UserName
        {
            get;
            set;
        }
        //[DataMember]
        //public ToTaskgtuinfomapping[] Taskgtuinfomappings
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Only used by the Otis transformer to transform the userId and UserName property.
        /// </summary>
        /// <param name="target">A <see cref="ToGtu"/>, the DTO.</param>
        /// <param name="source">A <see cref="Gtu"/>, the source.</param>
        public static void Convert(ref ToGtu target, ref Gtu source)
        {
            if (null != source.User)
            {
                target.UserId = source.User.Id;
                target.UserName = source.User.UserName;
            }
            else
            {
                target.UserId = 0;
                target.UserName = String.Empty;
            }
        }

        /// <summary>
        /// Only used by the Otis transformer to transform the role property.
        /// </summary>
        /// <param name="target">A <see cref="ToGtu"/>, the DTO.</param>
        /// <param name="source">A <see cref="Gtu"/>, the source.</param>
        public static void ConvertBack(ref Gtu target, ref ToGtu source)
        {
            User user = new User();
            user.Id = source.UserId;
            target.User = user;
        }

    }
}
