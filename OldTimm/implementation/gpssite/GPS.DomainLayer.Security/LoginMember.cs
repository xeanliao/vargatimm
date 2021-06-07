using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using GPS.DomainLayer.Enum;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Security
{
        /// <summary>
    /// Current member
    /// </summary>
    public class LoginMember
    {
        private const string MEMBER_SESSION_KEY = "USER";

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        /// <value>The current user.</value>
        public static User CurrentMember
        {
            get
            {
                var authCookie = HttpContext.Current.Request.Cookies["ssid"];
                if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
                {
                    var user = new UserRepository().GetUserByToken(authCookie.Value);
                    if (user != null && user.Enabled == true)
                    {
                        return user;
                    }
                }
                return null;
            }
            private set { }
        }
    }
}
