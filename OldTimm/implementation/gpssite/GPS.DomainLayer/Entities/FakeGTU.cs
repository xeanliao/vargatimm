using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class FakeGTU
    { //code for demonstrate
        double rate = 1000000.0;
        double step = 80000.0;
        double lonArl = 0;
        double latArl = 0;
        int direction = 1;
        //IList<ccGTURecord> GTURecordlst =new List<ccGTURecord>();

        //coordinate
        //double maxlon = -118.07283782959;
        //double minlon = -118.175720214844;
        //double maxlat = 34.0539016723633;
        //double minlat = 33.9722518920898;
        //double cenlon = -118.124279022217;
        //double cenlat = 34.013076782226548;

        double maxlon = 0;
        double minlon = 0;
        double maxlat = 0;
        double minlat = 0;
        double cenlon = 0;
        double cenlat = 0;

        ccLocation ccCurrentLocation = new ccLocation();
        String[] ccColorName = { "Red", "Purple", "Pink", "Yellow", "Green", "Brown", "CadetBlue", "Bisque", "Chocolate", "BurlyWood" };

        public List<ccGTURecord> makeGTUList(int len, double imaxlat, double imaxlon, double iminlat, double iminlon)
        {
            maxlon = imaxlon;
            minlon = iminlon;
            maxlat = imaxlat;
            minlat = iminlat;
            cenlon = (imaxlon + iminlon) / 2;
            cenlat = (iminlat + imaxlat) / 2;

            //gtuRecordlst.Clear();
            //GTURecordlst.Clear();
            List<string> GTURecordStrlst = new List<string>();
            List<ccGTURecord> GTURecordlst = new List<ccGTURecord>();
            List<string> gtuIdlst = new List<string>();
            for (int i = 0; i < len; i++)
            {
                string gtuId = "";
                gtuId = (i + 1).ToString("D3");
                gtuIdlst.Add(gtuId);
                GTURecordlst.Add(new ccGTURecord(gtuId, getRamdom(minlat, maxlat), getRamdom(minlon,maxlon),maxlat,maxlon, minlat,minlon));
            }
            return GTURecordlst;
        }

        public void makeGTUList2(List<ccGTURecord> GTURecordlst)
        {
            maxlon = GTURecordlst[0].ccmaxlon;
            minlon = GTURecordlst[0].ccminlon;
            maxlat = GTURecordlst[0].ccmaxlat;
            minlat = GTURecordlst[0].ccminlat;
            cenlon = (maxlon + minlon) / 2;
            cenlat = (maxlat + minlat) / 2;

            List<string> GTURecordStrlst = new List<string>();
            List<string> gtuIdlst = new List<string>();
            foreach (ccGTURecord gr in GTURecordlst)
            {
                gr.cclat = getRamdom(minlat, maxlat);
                gr.cclon = getRamdom(minlon, maxlon);
            }
        }


        public List<ccGTURecord> MakeGTUs(List<ccGTURecord> GTURecordlst)
        {
            if (GTURecordlst == null)
                return null;
            //List<string> gids = new List<string>();
            //foreach (ccGTURecord cr in GTURecordlst)
            //{
            //    if (cr.isSelected.Equals('Y')) ;
            //}
            maxlon = GTURecordlst[0].ccmaxlon;
            minlon = GTURecordlst[0].ccminlon;
            maxlat = GTURecordlst[0].ccmaxlat;
            minlat = GTURecordlst[0].ccminlat;
            cenlon = (GTURecordlst[0].ccmaxlon + GTURecordlst[0].ccminlon) / 2;
            cenlat = (GTURecordlst[0].ccminlat + GTURecordlst[0].ccmaxlat) / 2;
            lonArl = (maxlon - minlon) / step;
            latArl = (maxlat - minlat) / step;
            //List<ccGTURecord> resultlst = new List<ccGTURecord>();
            foreach (ccGTURecord cr in GTURecordlst)
            {
                if (cr.isSelected)
                {
                    //ccGTURecord ccGTU = new ccGTURecord();
                    //ccGTURecord lastGTU = new ccGTURecord();
                    //lastGTU = GTURecordlst.Where(cc => cc.ccGID == cr.ccGID).First();
                    System.Random dr = new Random(GetRandomSeed());
                    //System.Random dr = new Random();
                    direction = dr.Next(1, 200);
                    ccLocation lastLoc = new ccLocation() { lat = cr.cclat, lon = cr.cclon };
                    //ccLocation lastLoc = new ccLocation(lastGTU.cclat, lastGTU.cclon);
                    ccLocation currLoc = getNextLoc(direction, lastLoc);
                    //cr.ccGID = cr.ccGID;
                    cr.cclat = currLoc.lat;
                    cr.cclon = currLoc.lon;
                    cr.ccmaxlat = maxlat;
                    cr.ccmaxlon = maxlon;
                    cr.ccminlat = minlat;
                    cr.ccminlon = minlon;
                }
            }

            return GTURecordlst;
        
        
        }
        //{
        //    if (GTURecordStrlst == null)
        //        return null;
        //    List<ccGTURecord> GTURecordlst = new List<ccGTURecord>();
        //    List<string> gids = new List<string>();
        //    foreach (string str in GTURecordStrlst)
        //    {
        //        GTURecordlst.Add(StringToCcGTURecord(str));
        //        //isSlected
        //        if (StringToCcGTURecord(str).isSelected.Equals('Y'))
        //            gids.Add(StringToCcGTURecord(str).ccGID);
        //    }
        //    maxlon = GTURecordlst[0].ccmaxlon;
        //    minlon = GTURecordlst[0].ccminlon;
        //    maxlat = GTURecordlst[0].ccmaxlat;
        //    minlat = GTURecordlst[0].ccminlat;
        //    cenlon = (GTURecordlst[0].ccmaxlon + GTURecordlst[0].ccminlon) / 2;
        //    cenlat = (GTURecordlst[0].ccminlat + GTURecordlst[0].ccmaxlat) / 2;
        //    //List<ccGTURecord> resultlst = new List<ccGTURecord>();
        //    if (gids.Count == 0)
        //        return null;
        //    foreach (string gid in gids)
        //    {
        //        ccGTURecord ccGTU = new ccGTURecord();
        //        ccGTURecord lastGTU = new ccGTURecord();
        //        lastGTU = GTURecordlst.Where(cc => cc.ccGID == gid).First();
        //        System.Random dr = new Random(GetRandomSeed());
        //        //System.Random dr = new Random();
        //        direction = dr.Next(1, 200);
        //        ccLocation lastLoc = new ccLocation() { lat = lastGTU.cclat, lon = lastGTU.cclon };
        //        //ccLocation lastLoc = new ccLocation(lastGTU.cclat, lastGTU.cclon);
        //        ccLocation currLoc = getNextLoc(direction, lastLoc);
        //        ccGTU.ccGID = gid;
        //        ccGTU.cclat = currLoc.lat;
        //        ccGTU.cclon = currLoc.lon;
        //        ccGTU.ccmaxlat = maxlat;
        //        ccGTU.ccmaxlon = maxlon;
        //        ccGTU.ccminlat = minlat;
        //        ccGTU.ccminlon = minlon;
        //        GTURecordStrlst.Add(ccGTURecordToString(ccGTU));
        //    }

        //    return GTURecordStrlst;
        //}

        private ccLocation getNextLoc(int direction, ccLocation lastLoc)
        {
            ccLocation curr = new ccLocation();
            //if (((1 == (direction & 1)) == (lastLoc.lat < cenlat)) && ((2 == (direction & 2)) == (lastLoc.lon < cenlon)))
            //{
            //    curr.lat = lastLoc.lat + ((1 == (direction & 1)) ? 1 : -1) * getRamdomArl(false);
            //    curr.lon = lastLoc.lon + ((2 == (direction & 2)) ? 1 : -1) * getRamdomArl(true);
            //}
            //else
            //{
            //    curr.lat = lastLoc.lat + ((1 == (direction & 1)) ? -1 : 1) * getRamdomArl(false);
            //    curr.lon = lastLoc.lon + ((2 == (direction & 2)) ? -1 : 1) * getRamdomArl(true);
            //}
            //curr.lat = lastLoc.lat + ((lastLoc.lat > cenlat) ? -1 : 1) * getRamdomArl(false);
            //curr.lon = lastLoc.lon + ((lastLoc.lon > cenlon) ? -1 : 1) *getRamdomArl(true);
            curr.lat = lastLoc.lat + ((1 == (direction & 1)) ? -1 : 1) * getRamdomArl(false);
            curr.lon = lastLoc.lon + ((2 == (direction & 2)) ? -1 : 1) * getRamdomArl(true);
            if (curr.lat < minlat || curr.lat > maxlat || curr.lon > maxlon || curr.lon < minlon)
                curr = new ccLocation() { lat = getRamdom(minlat, maxlat), lon = getRamdom(minlon, maxlon) };
                //curr = new ccLocation(cenlat, cenlon);
            return curr;
        }


        //random
        public double getRamdom(double rmin, double rmax)
        {
            System.Random r = new Random(GetRandomSeed());
            return (r.Next(Convert.ToInt32(rmin * rate), Convert.ToInt32(rmax * rate))) * (1 / rate);
        }

        public double getRamdomArl(bool islon)
        {
            System.Random r = new Random(GetRandomSeed());
            if(islon)
                return (r.Next(500, 1000)) * (lonArl);
            else
                return (r.Next(500, 1000)) * (latArl);
        }

        static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        //random

        //public IList<ccGTURecord> gtuRecordlst = new List<ccGTURecord>();
        //public virtual IList<ccGTURecord> GTURecordlst
        //{
        //    get { return gtuRecordlst; }
        //    set { gtuRecordlst = value; }
        //}

        //public string ccGTURecordToString(ccGTURecord cr)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(cr.ccGID + '@');
        //    sb.Append(cr.cclat + '@');
        //    sb.Append(cr.cclon + '@');
        //    sb.Append(cr.ccmaxlat + '@');
        //    sb.Append(cr.ccmaxlon + '@');
        //    sb.Append(cr.ccminlat + '@');
        //    sb.Append(cr.ccminlon + '@');
        //    return sb.ToString(); 
        //}

        //public ccGTURecord StringToCcGTURecord(string cstring)
        //{
        //    ccGTURecord cr = new ccGTURecord();
        //    string[] crArray = cstring.Split('@');
        //    cr.ccGID = crArray[0];
        //    cr.cclat = Double.Parse(crArray[0]);
        //    cr.cclat = Double.Parse(crArray[1]);
        //    cr.cclon = Double.Parse(crArray[2]);
        //    cr.ccmaxlat = Double.Parse(crArray[3]);
        //    cr.ccmaxlon = Double.Parse(crArray[4]);
        //    cr.ccminlat = Double.Parse(crArray[5]);
        //    cr.ccminlon = Double.Parse(crArray[6]);
        //    return cr;
        //}

        //public double mmaxlon = 0;
        //public virtual double maxlon
        //{
        //    get { return mmaxlon; }
        //    set { mmaxlon = value; }
        //}
        //public double mminlon = 0;
        //public virtual double minlon
        //{
        //    get { return mminlon; }
        //    set { mminlon = value; }
        //}
        //public  double mmaxlat = 0;
        //public virtual double maxlat
        //{
        //    get { return mmaxlat; }
        //    set { mmaxlat = value; }
        //}
        //public double mminlat = 0;
        //public virtual double minlat
        //{
        //    get { return mminlat; }
        //    set { mminlat = value; }
        //}
        //public  double ccenlon = 0;
        //public virtual double cenlon
        //{
        //    get { return ccenlon; }
        //    set { ccenlon = value; }
        //}
        //public double ccenlat = 0;
        //public virtual double cenlat
        //{
        //    get { return ccenlat; }
        //    set { ccenlat = value; }
        //}  
    }

    public class ccGTURecord
    {
        public ccGTURecord(string GID, double lat, double lon,double maxlat,double maxlon,double minlat,double minlon)
        { 
            ccGID = GID;
            cclat = lat;
            cclon = lon;
            ccmaxlat = maxlat;
            ccmaxlon = maxlon;
            ccminlat = minlat;
            ccminlon = minlon;
        }
        public ccGTURecord(){}
        public string ccGID { get; set; }
        public bool isSelected { get; set; }
        public double cclat { get; set; }
        public double cclon { get; set; }
        public double ccmaxlat { get; set; }
        public double ccmaxlon { get; set; }
        public double ccminlat { get; set; }
        public double ccminlon { get; set; }
    }


    public class ccLocation
    {
        //public ccLocation(double lat, double lon);
        //public ccLocation();
        public double lat { get; set; }
        public double lon { get; set; }

    }
}
