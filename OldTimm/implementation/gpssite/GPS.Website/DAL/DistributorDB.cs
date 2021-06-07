using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.DAL
{
    public class DistributorDB
    {
        public static DAL.company[] GetDistributorByName(string sName)
        {
            timmEntities context = new timmEntities();
            return context.companies.Where(c => c.Name.IndexOf(sName) >= 0).ToArray();
        }

        public static DAL.company GetDistributorByID(int iID)
        {
            timmEntities context = new timmEntities();
            return context.companies.Where(c => c.Id == iID).FirstOrDefault();
        }
    }
}