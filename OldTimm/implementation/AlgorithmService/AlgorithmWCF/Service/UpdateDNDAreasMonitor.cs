using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WHYTAlgorithmService.Geo
{
    public class UpdateDNDAreasMonitor
    {
        private System.Threading.Timer _oTimer;
        long _nInterval;
        public void GTULisTMonitorProc(Object stateInfo)
       {
            long nInterval = (long)stateInfo;
            //DateTime dt = DateTime.Now - new TimeSpan(10 * nInterval);
            //GTUStore.Instance.CleanupGTU(dt); 
            if (DateTime.Now.Hour > 22 || DateTime.Now.Hour < 7 || nInterval == 28800000)
           {
               DNDAreaStore.Instance.UpdateDNDAreaStoreDct();
           }
        }

        public UpdateDNDAreasMonitor(long nInterval)
        {
            _nInterval = nInterval;
            _oTimer = new System.Threading.Timer(GTULisTMonitorProc, nInterval, Timeout.Infinite, nInterval);

        }
        /// <summary>
        /// Using double lock check to make sure only one instance of timer will be started
        /// </summary>
        public void Start()
        {
            if(_oTimer != null)
            {
                lock (_oTimer)
                {
                    if (_oTimer != null)
                        _oTimer = new System.Threading.Timer(GTULisTMonitorProc, _nInterval, 0, _nInterval);
                }
            }

        }

        public void GTULisTMonitorProc2(Object stateInfo)
        {}
    }
}
