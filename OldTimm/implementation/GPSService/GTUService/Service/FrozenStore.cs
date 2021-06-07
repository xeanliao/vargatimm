using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTUService.TIMM;

namespace TIMM.GTUService.Service
{
    public class FrozenStore
    {
        static readonly FrozenStore f_Instance = new FrozenStore();
        static Dictionary<string, Queue<GTU>> _ofrozenDct = new Dictionary<string, Queue<GTU>>();

        const int MAX_ALLOWED_RANGE = 10;   // 10 meter
        const int MAX_ALLOWED_COUNT = 5;     // 5 times

        /// <summary>
        /// Singleton class
        /// </summary>
        public static FrozenStore Instance
        {
            get
            {
                return f_Instance;
            }
        }


        public bool UpdateFrozen(GTU currentGTU)
        {
            System.Diagnostics.Trace.TraceInformation("Begin to UpdatefrozenStore \n");
            string sCode = currentGTU.Code;
            bool needSendEmail = false;
            if (sCode != null)
            {
                //Send Email for OUTBOUNDARY
                if (currentGTU.Status == Status.OutBoundary)
                    needSendEmail = true;

                lock (_ofrozenDct)
                {
                    if (_ofrozenDct.ContainsKey(sCode))
                    {
                        Queue<GTU> queue = _ofrozenDct[sCode];
                        GTU lastGTU = queue.Last<GTU>();

                        Coordinate last = lastGTU.CurrentCoordinate;
                        Coordinate current = currentGTU.CurrentCoordinate;
                        if (lastGTU.Status == Status.OutBoundary)
                            needSendEmail = false;
                        double distance = 1000 * Distance.CalcDist(last.Latitude, last.Longitude, last.Altitude, current.Latitude, current.Longitude, current.Altitude);
                        currentGTU.Distance = distance;                         
                        
                        // active
                        if (distance > MAX_ALLOWED_RANGE)   
                        {
                            queue.Clear();
                            queue.Enqueue(currentGTU);
                        }
                        // possible frozen
                        else
                        {
                            queue.Enqueue(currentGTU);
                            // centain  frozen
                            if (queue.Count >= MAX_ALLOWED_COUNT)
                            {
                                queue.Clear();
                                queue.Enqueue(currentGTU);
                                if (currentGTU.Status == Status.OutBoundary)
                                    currentGTU.Status = Status.OutAndFrozen;
                                else
                                    currentGTU.Status = Status.Frozen;
                                    
                            }
                        }
                       
                    }
                    else
                    {
                        Queue<GTU> stack = new Queue<GTU>();                        
                        stack.Enqueue(currentGTU);
                        _ofrozenDct.Add(sCode, stack);
                    }
                }                                             
            }
            System.Diagnostics.Trace.TraceInformation("End UpdatefrozenStore \n");
            return needSendEmail;
            //return true;

        }

        public void RemoveFrozen(string sCode)
        {
            if (_ofrozenDct.ContainsKey(sCode))
                _ofrozenDct.Remove(sCode);
        }

    }
}
