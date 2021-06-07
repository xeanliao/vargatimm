/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.DAL
{
    public class GtuStatusDB
    {
        public static DAL.gtustatushistory GetLastGtuStatusRecord(int gtuID)
        {
            timmEntities context = new timmEntities();
            DAL.gtustatushistory gtuStatusRecord = context.gtustatushistories.Where(s => s.GTUId == gtuID).OrderByDescending(it => it.Id).FirstOrDefault();

            return gtuStatusRecord;
        }

        public static int GetLastGtuStatusID(int gtuID)
        {
            DAL.gtustatushistory gtuStatusRecord = GetLastGtuStatusRecord(gtuID);
            if (gtuStatusRecord == null)
                return (int)DAL.GtuStatusEum.No_GTU_Signal;

            return gtuStatusRecord.StatusId;
        }
    }
}
*/