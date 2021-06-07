﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTUService.TIMM;

namespace GTUService.TIMM
{
    public class GTULisTMonitor
    {

        private System.Threading.Timer _oTimer;
        long _nInterval;
        static private void GTULisTMonitorProc(Object stateInfo)
        {
            long nInterval = (long)stateInfo;
            DateTime dt = DateTime.Now - new TimeSpan(10 * nInterval);
            GTUStore.Instance.CleanupGTU(dt); 

        }

        public GTULisTMonitor(long nInterval)
        {
            _nInterval = nInterval;
            _oTimer = new System.Threading.Timer(GTULisTMonitorProc, nInterval, 0, nInterval);

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
    
    }
}
