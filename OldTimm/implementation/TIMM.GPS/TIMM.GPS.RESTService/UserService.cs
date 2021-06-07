using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GPS.Model;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace TIMM.GPS.RESTService
{
    [ServiceContract]
    public class UserService
    {
        internal static User CurrentUser
        {
            get
            {
#if DEBUG
                using (var context = new TIMMContext())
                {
                    return context.Users.FirstOrDefault(i => i.Id == 1);
                }
#endif

                var userId = (int)HttpContext.Current.Session["CrossDomainUserId"];
                using (var context = new TIMMContext())
                {
                    return context.Users.FirstOrDefault(i => i.Id == userId);
                }
                
            }
        }

        internal static int CurrentUserId
        {
            get
            {
#if DEBUG
                return 1;
#endif
                var userId = (int)HttpContext.Current.Session["CrossDomainUserId"];
                return userId;

            }
        }

        [WebGet(UriTemplate = "sales/")]
        public List<User> GetSales()
        {
            using(var context = new TIMMContext())
            {
                var query = from i in context.Groups
                            where i.Id == 46
                            select i.Users;
                return query.First();
            }
        }

        [WebInvoke(UriTemplate = "groups/")]
        public List<User> GetUserByGroupList(List<int> groups)
        {
            using (var context = new TIMMContext())
            {
                var query = from i in context.Groups
                            where groups.Contains(i.Id)
                            select i.Users;
                return query.First();
            }
        }
    }
}
