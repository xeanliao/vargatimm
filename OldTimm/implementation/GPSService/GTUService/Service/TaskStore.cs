using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTUService.TIMM;
using TIMM.GTUService.Geofencing;

using Geo = TIMM.GTUService.Geofencing;
using TIMM.GTUService;

namespace GTUService.TIMM
{
    public class TaskStore
    {
        static readonly TaskStore t_Instance = new TaskStore();
        static Dictionary<int, string> _oTaskDct = new Dictionary<int, string>();
        static Dictionary<int, Dictionary<int, string>> _oTaskNdIdsDct = new Dictionary<int, Dictionary<int, string>>();
        static Dictionary<string, int> _oGTUTaskDct = new Dictionary<string, int>();
        static Dictionary<int, TaskInfoDctValue> _oTaskInfoDct = new Dictionary<int, TaskInfoDctValue>();
        GeofencingClient geoClient = new GeofencingClient();

        /// <summary>
        /// Singleton class
        /// </summary>
        public static TaskStore Instance
        {
            get
            {
                return t_Instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="regStr"></param>
        public void getAviableTasks()
        {
            System.Diagnostics.Trace.TraceInformation("Begin to getAviableTasks \n");
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            _oTaskInfoDct = dao.AvailableTaskId();
            if (_oTaskInfoDct == null) return;
            Dictionary<int, List<Coordinate>> taskcollectionDic = dao.getDmCollectionByTaskIds(_oTaskInfoDct.Keys.ToList());
            foreach (int tid in taskcollectionDic.Keys) 
            {
                try
                {
                    List<string> gtuLst = new List<string>();
                    gtuLst.AddRange(dao.getGTULstByTaskId(tid));
                    foreach(string gCode in gtuLst)
                        if(!_oGTUTaskDct.ContainsKey(gCode))
                            _oGTUTaskDct.Add(gCode, tid);
                    //string geoString = geoClient.RegisterArea(convertToGeoCoordinate(taskcollectionDic[tid]).ToArray());
                    _oTaskInfoDct[tid].registerStr = String.Empty; //TODO geoClient.RegisterArea(convertToGeoCoordinate(taskcollectionDic[tid]).ToArray());
                    System.Diagnostics.Trace.TraceInformation("Add task id="+ tid +"successfully" + "\n");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.TraceInformation("Fail to get task id=" + tid + "\n" + e.Message);
                }
            }

            System.Diagnostics.Trace.TraceInformation("End getAviableTasks \n");
        }

        public bool addTask(int tid)
        {
            System.Diagnostics.Trace.TraceInformation("Begin  addTask \n");
            if (_oTaskDct.ContainsKey(tid)) return true;
            try
            {
                List<string> gtuLst = new List<string>();
                IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
                gtuLst.AddRange(dao.getGTULstByTaskId(tid));
                foreach (string gCode in gtuLst)
                    if (!_oGTUTaskDct.ContainsKey(gCode))
                        _oGTUTaskDct.Add(gCode, tid);
                List<Coordinate> taskcollection = dao.getDmCollectionByTaskId(tid);
                string geoString = String.Empty; // TODO geoClient.RegisterArea(convertToGeoCoordinate(taskcollection).ToArray());
                //_oTaskDct.Add(tid, geoString);
                _oTaskInfoDct.Add(tid, new TaskInfoDctValue(dao.getDMName(tid), geoString, dao.getMailAddress(tid)));
                System.Diagnostics.Trace.TraceInformation("Add task id=" + tid + "successfully" + "\n");
                System.Diagnostics.Trace.TraceInformation("End addTask \n");
                System.Diagnostics.Trace.TraceInformation("Begin  getNdIds \n");
                getNdIds(tid);
                System.Diagnostics.Trace.TraceInformation("End  getNdIds \n");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceInformation("Fail to get task id=" + tid + "\n" + e.Message);
                System.Diagnostics.Trace.TraceInformation("End addTask \n");
                return false;
            }
        }

        public bool removeTask(int tid)
        {
            try
            {
                if (_oTaskInfoDct.ContainsKey(tid))
                    _oTaskInfoDct.Remove(tid);
                List<string> keys = new List<string>();
                foreach( KeyValuePair<string,int> kv in _oGTUTaskDct )
                {
                    if (kv.Value == tid) keys.Add(kv.Key);
                } 
                if (keys!=null && keys.Count>0)
                    keys.ForEach(k => _oGTUTaskDct.Remove(k));

                System.Diagnostics.Trace.TraceInformation("removeTask  successfully" + "\n");
                if (_oTaskNdIdsDct.ContainsKey(tid))
                    _oTaskNdIdsDct.Remove(tid);
                System.Diagnostics.Trace.TraceInformation("removeNdIds  successfully" + "\n");
                return true;
            }
            catch
            {
                System.Diagnostics.Trace.TraceInformation("Fail to removeTask" + "\n");
                return true;
            }
        }

        public string getRegisterString(int tid)
        {
            string gString = string.Empty;
            _oTaskDct.TryGetValue(tid, out gString);
            return gString;
        }

        public string getRegisterStr(string gid)
        {
            string gString = string.Empty;
            TaskInfoDctValue tv = new TaskInfoDctValue();
            if (_oGTUTaskDct.ContainsKey(gid))
                if (_oTaskInfoDct.TryGetValue(_oGTUTaskDct[gid], out tv))
                    gString = tv.registerStr;
                //_oGTUTaskDct[gid]
            
            return gString;
        }

        public int[] getAroundDNDAreaIds(string gid)
        {

            Dictionary<int, string> ret = new Dictionary<int, string>();
            if (_oGTUTaskDct.ContainsKey(gid) && _oTaskNdIdsDct.ContainsKey(_oGTUTaskDct[gid]))
            {
                ret = _oTaskNdIdsDct[_oGTUTaskDct[gid]];
            }
            //_oTaskNdIdsDct.TryGetValue(_oGTUTaskDct[gid], out ret);
            return ret.Keys.ToArray();
           
        }

        public bool IsInDNDArea(GTU oGtu, int[] aroundDNDAreaIds)
        {
            string retStr="";
            oGtu.dndInfo = string.Empty;
            Dictionary<int, string> ret = new Dictionary<int, string>();
            string dndIds = geoClient.IsInTheDNDArea(convertToGeoCoordinate(oGtu.CurrentCoordinate), aroundDNDAreaIds);
            if (dndIds == string.Empty) return false;
            var ids= dndIds.Split('&');
            if (ids.Length == 0) return false;
            try{
                if (_oTaskNdIdsDct.TryGetValue(_oGTUTaskDct[oGtu.Code], out ret))
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        if (ids[i] == string.Empty) continue;
                        string temp;
                        if (ret.TryGetValue(Convert.ToInt32(ids[i]), out temp))
                        {
                            temp.Trim();
                            retStr = retStr + temp + " ;";
                        }
                        else
                        {
                            retStr = retStr + "Can't get DND Area information!";

                        }
                    }
                }
            }
            catch
            {
                System.Diagnostics.Trace.TraceInformation("Fail to get DND zone informations" + "\n");
                return false;
            }
            oGtu.dndInfo = retStr.Trim(';').Trim();
            return true;
        }

        public string getEmailAddresses(string gid)
        {
            string emailAddresses = string.Empty;
            TaskInfoDctValue tv = new TaskInfoDctValue();
            if (_oGTUTaskDct.ContainsKey(gid))
                if (_oTaskInfoDct.TryGetValue(_oGTUTaskDct[gid], out tv))
                    emailAddresses = tv.mailAddresses;
            //_oGTUTaskDct[gid]

            return emailAddresses;
        }

        public string getDMName(string gid)
        {
            string dmName = string.Empty;
            TaskInfoDctValue tv = new TaskInfoDctValue();
            if (_oGTUTaskDct.ContainsKey(gid))
                if (_oTaskInfoDct.TryGetValue(_oGTUTaskDct[gid], out tv))
                    dmName = tv.dmName;
            //_oGTUTaskDct[gid]

            return dmName;
        }

        public TaskInfoDctValue getTaskInfo(string gid)
        {
            TaskInfoDctValue tv = new TaskInfoDctValue();
            if (_oGTUTaskDct.ContainsKey(gid))
                _oTaskInfoDct.TryGetValue(_oGTUTaskDct[gid], out tv);
            return tv;

        }

        public List<Geo.Coordinate> convertToGeoCoordinate(List<Coordinate> source)
        {
            List<Geo.Coordinate> target = new List<Geo.Coordinate>();
            foreach (Coordinate sitem in source)
            {
                Geo.Coordinate titem = new Geo.Coordinate();
                titem.Latitude = sitem.Latitude;
                titem.Longitude = sitem.Longitude;
                titem.Altitude = sitem.Altitude;
                target.Add(titem);
            }
            return target;
        }

        public bool IsInArea(Coordinate cn, String geoStr)
        {
            return geoClient.IsInTheArea(convertToGeoCoordinate(cn), geoStr);
        }

        public Geo.Coordinate convertToGeoCoordinate(Coordinate source)
        {
            Geo.Coordinate target = new Geo.Coordinate();
            target.Latitude = source.Latitude;
            target.Longitude = source.Longitude;
            target.Altitude = source.Altitude;
            return target;
        }

        public void getNdIds(int taskid)
        {
            List<int> tids = new List<int>();
            if (taskid != -1) tids.Add(taskid);
            if (taskid == -1 && _oTaskInfoDct.Count > 0)
            {
                tids.AddRange(_oTaskInfoDct.Keys);
                IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
                foreach (int tid in tids)
                {
                    if (!_oTaskNdIdsDct.ContainsKey(tid))
                    {
                        _oTaskNdIdsDct.Add(tid, dao.getNDAreaIdsByTaskId(tid));
                    }
                }
            }
        }
    }

    public class TaskInfoDctValue 
    {
        string sRegisterStr;
        string sMailAddresses;
        string sDMName;

        public TaskInfoDctValue()
        {
        }

        public TaskInfoDctValue(string dStr,string rStr,string mStr)
        {
            sDMName = dStr;
            sRegisterStr = rStr;
            sMailAddresses = mStr;
        }

        public string registerStr
        {
            get { return sRegisterStr; }
            set { sRegisterStr = value; }
        }
        public string mailAddresses
        {
            get { return sMailAddresses; }
            set { sMailAddresses = value; }
        }
        public string dmName
        {
            get { return sDMName; }
            set { sDMName = value; }
        } 
    }
}
