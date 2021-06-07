using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTUService.TIMM
{
    public class GTUStore
    {
        static readonly GTUStore s_Instance = new GTUStore();
        static Dictionary<string, GTU> _oGTUDct = new Dictionary<string, GTU>();

        /// <summary>
        /// Singleton class
        /// </summary>
        public static GTUStore Instance
        {
            get
            {
                return s_Instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sID"></param>
        /// <param name="oGTU"></param>
        public void UpdateGTU(string sCode, GTU oGTU)
        {
            System.Diagnostics.Trace.TraceInformation("Begin to UpdateGTUDct \n");
            if (sCode != null)
            {
                lock (_oGTUDct)
                {
                    if (_oGTUDct.ContainsKey(sCode))
                    {
                        _oGTUDct[sCode] = oGTU;
                    }
                    else
                    {
                        _oGTUDct.Add(sCode, oGTU);
                    }
                }
            }
            System.Diagnostics.Trace.TraceInformation("End UpdateGTUDct \n");
        }

        /// <summary>
        /// Get GTU based on GTUID
        /// </summary>
        /// <param name="sID"></param>
        /// <returns></returns>
        public GTU GetGTU(string sCode)
        {
            System.Diagnostics.Trace.TraceInformation("Begin to GetGTU\n");
            GTU oGTU = null;
            if (sCode != null)
            {
                lock (_oGTUDct)
                {
                    if (!_oGTUDct.TryGetValue(sCode, out oGTU))
                    {
                        oGTU = null;
                    }
                }
            }
            System.Diagnostics.Trace.TraceInformation("End GetGTU \n");
            return oGTU; 
        }


        /// <summary>
        /// Get the GTU ID list
        /// </summary>
        /// <returns></returns>
        public List<String> GetGTUNameList()
        {
            System.Diagnostics.Trace.TraceInformation("Begin to GetGTUNameList \n");
            List<String> oGTUNameList = null;
            lock (_oGTUDct)
            {
                oGTUNameList = new List<string>(_oGTUDct.Keys);
            }
            System.Diagnostics.Trace.TraceInformation("End GetGTUNameList \n");
            return oGTUNameList;
        }

        /// <summary>
        /// Delete the GTU from the list if the time is older than the passed in time
        /// </summary>
        /// <param name="dt"></param>
        public void CleanupGTU(DateTime dt)
        {
            System.Diagnostics.Trace.TraceInformation("Begin to CleanupGTU \n");
            lock (_oGTUDct)
            {
                List<GTU> oGTUList = new List<GTU>(_oGTUDct.Values);
                foreach(GTU oGTU in oGTUList)
                {
                    if (oGTU.ReceivedTime < dt)
                    {
                        _oGTUDct.Remove(oGTU.Code); 
                    }

                }
            }
            System.Diagnostics.Trace.TraceInformation("End CleanupGTU \n");
        }
    }
}
