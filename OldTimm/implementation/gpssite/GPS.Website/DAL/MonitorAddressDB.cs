using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.DAL
{
    public class MonitorAddressDB
    {
        public static void SaveMonitorAddress(int id, string street, string zip, string picture, int distributionMapID, string sNotes, int taskId)
        {
            DAL.timmEntities context = new timmEntities();
            DAL.monitoraddress addr = null;
            if (id == 0)
            {
                if (context.monitoraddresses.Where(it => it.TaskID == taskId && it.Address == street && it.ZipCode == zip).Count() > 0)
                    throw new Exception("Address is duplicated.");
                addr = new monitoraddress();
            }
            else
                addr = context.monitoraddresses.Where(a => a.Id == id).FirstOrDefault();

            if (addr.Address != street | addr.ZipCode != zip)
            {
                // call bing map service to get 
                DAL.Coordinate point = DAL.BingMapUtils.GetCoordinate(street + ", " + zip);

                addr.Address = street;
                addr.ZipCode = zip;
                addr.Latitude = (float)Convert.ToDecimal(point.Latitude);
                addr.Longitude = (float)Convert.ToDecimal(point.Longitude);
                addr.OriginalLatitude = addr.Latitude;
                addr.OriginalLongitude = addr.Longitude;
            }

            // only change photo when there is a new photot, to remove phone call RemoveMonitorAddressPhoto
            addr.DmId = distributionMapID;
            addr.Notes = sNotes;
            addr.TaskID = taskId;
            
            if (picture.Trim() != "")
            {
                addr.Picture = picture;
                addr.PictureUploadTime = DateTime.Now;
            }

            if(addr.Id == 0)
                context.AddTomonitoraddresses(addr);
            context.SaveChanges();
        }

        public static List<DAL.monitoraddress> GetMonitorAddressListByDMapID(int iDistributionMapID)
        {
            DAL.timmEntities context = new timmEntities();
            return (from m in context.monitoraddresses
                             where m.DmId == iDistributionMapID
                             select m).ToList();
        }

        public static DAL.monitoraddress GetMonitorAddressByID(int id)
        {
            DAL.timmEntities context = new timmEntities();
            return context.monitoraddresses.Where(a => a.Id == id).FirstOrDefault();
        }

        public static void DeleteMonitorAddress(int iAddressID)
        {
            DAL.timmEntities context = new timmEntities();
            monitoraddress addr = (from a in context.monitoraddresses
                                       where a.Id == iAddressID
                                       select a).FirstOrDefault();
            context.DeleteObject(addr);
            context.SaveChanges();
        }

        public static void RemoveMonitorAddressPhoto(int id)
        {
            DAL.timmEntities context = new timmEntities();
            DAL.monitoraddress addr = context.monitoraddresses.Where(a => a.Id == id).FirstOrDefault();
            addr.Picture = "";
            context.SaveChanges();
        }

    }   // end of class
}