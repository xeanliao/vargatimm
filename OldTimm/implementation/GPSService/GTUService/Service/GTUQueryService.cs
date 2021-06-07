using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IdentityModel.Selectors;
using TIMM.GTUService;
using TIMM.GTUService.Geofencing;

using Geo = TIMM.GTUService.Geofencing;

namespace GTUService.TIMM
{
    public class GTUQueryService : IGTUQueryService
    {
        GTUStore _oGTUStore;
        TaskStore _oTaskStore;
        GeofencingClient geoClient = new GeofencingClient();

        public GTUQueryService()
        {
            _oGTUStore = GTUStore.Instance;
            _oTaskStore = TaskStore.Instance;
        }

        public string RegisterDM(List<Coordinate> oArea)
        {
            string geoString="";
            List<Geo.Coordinate> area = new List<Geo.Coordinate>();
            foreach (Coordinate cn in oArea)
            {
                Geo.Coordinate gcn = new Geo.Coordinate();
                gcn.Latitude = cn.Latitude;
                gcn.Longitude = cn.Longitude;
                gcn.Altitude = cn.Altitude;
                area.Add(gcn);
            }
            try
            {
                geoString = geoClient.RegisterArea(area.ToArray());
                System.Diagnostics.Trace.TraceInformation("get geoString  successfully" + "\n");
            }
            catch
            {
                System.Diagnostics.Trace.TraceInformation("get geoString  unsuccessfully" + "\n");
            }
            return geoString;
        }


        public GTU GetGTU(String sGTUCode)
        {
            GTU oGTU = _oGTUStore.GetGTU(sGTUCode);
            if (oGTU == null)
            {
                //oGTU = new GTU(); // return  a empty GTU
                System.Diagnostics.Trace.TraceWarning("Cannot find the GTU based on the ID {0} \n", sGTUCode);
            }
            return oGTU;
        }

        public bool IsInArea(Coordinate cn,String geoStr)
        {
            return geoClient.IsInTheArea(convertToGeoCoordinate(cn), geoStr);
        }

        public bool IsFrozen(Coordinate cn, String sGTUCode)
        {
            return false;
        }
        //public List<GTU> GetGTUs(List<String> sGTUCodes, int tid)
        //{
        //    List<GTU> list = new List<GTU>();
        //    string gString = _oTaskStore.getRegisterString(tid);
            
        //    foreach (string code in sGTUCodes)
        //    {
        //        GTU g = GetGTU(code);
        //        if (g != null && gString != String.Empty)
        //            if (!IsInArea(g.CurrentCoordinate, gString))
        //            {
        //                g.Status = Status.OutBoundary;
        //                sendEmail(tid, code);
        //            }
        //        if (IsFrozen(g.CurrentCoordinate, g.Code))
        //            if (g.Status == Status.OutBoundary)
        //                g.Status = Status.OutAndFrozen;
        //            else
        //                g.Status = Status.Frozen;
        //       list.Add(g);
        //    }
        //    return list;
        //}

        public List<GTU> GetGTUs(List<String> sGTUCodes)
        {
            List<GTU> list = new List<GTU>();
            foreach (string code in sGTUCodes)
            {
                GTU g = GetGTU(code);
                if(g!=null)
                    list.Add(g);
            }
            return list;
        }

        public void sendEmail(int tid, String gtuCode)
        { 
            
        }

        public Coordinate GetCurrentCoordinate(string sGTUCode)
        {
            Coordinate oCoordinate = null;
            GTU oGTU = _oGTUStore.GetGTU(sGTUCode);
            if (oGTU != null)
            {
                oCoordinate = oGTU.CurrentCoordinate;
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Cannot find the GTU based on the ID {0} \n", sGTUCode);
            }
            return oCoordinate;
        }

        public List<Coordinate> GetCurrentCoordinates(List<string> sGTUCodes)
        {
            List<Coordinate> list = new List<Coordinate>();
            foreach (string code in sGTUCodes)
            {
                Coordinate c = GetCurrentCoordinate(code);
                
                if (c != null)
                    list.Add(c);
            }
            return list;
        }
               
        public bool GetGTUInfo(String sGTUCode, ref int nHeading, ref double dwSpeed, ref PowerInfo ePowerInfo)
        {
            bool bRet = false;
            GTU oGTU = _oGTUStore.GetGTU(sGTUCode);
            if (oGTU != null)
            {
                nHeading = oGTU.Heading;
                dwSpeed = oGTU.Speed;
                ePowerInfo = oGTU.PowerInfo;
                bRet = true;
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Cannot find the GTU based on the ID {0} \n", sGTUCode);
            }
            return bRet;
        }

        public List<String> GetGTUCodeList()
        {
            return _oGTUStore.GetGTUNameList();
        }

        public Geo.Coordinate convertToGeoCoordinate(Coordinate source) 
        {
            Geo.Coordinate target = new Geo.Coordinate();
            target.Latitude = source.Latitude;
            target.Longitude = source.Longitude;
            target.Altitude = source.Altitude;
            return target;
        }

        public bool AddTaskToStore(int tid) 
        {
            return _oTaskStore.addTask(tid);
        }

        public bool RemoveTaskFromStore(int tid) 
        {
            return _oTaskStore.removeTask(tid); 
        }
    }
}
