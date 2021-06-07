using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WHYTAlgorithmService.Geo
{
    using Area = List<Coordinate>;

    public class AreaStore
    {
        static readonly AreaStore s_Instance = new AreaStore();
        static Dictionary<string, Types.Poly> _oAreaStoreDct = new Dictionary<string, Types.Poly>();

        /// <summary>
        /// Singleton class
        /// </summary>
        public static AreaStore Instance
        {
            get
            {
                return s_Instance;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sGUID"></param>
        /// <param name="oArea"></param>
        /// <returns></returns>
        public bool GetArea(string sGUID, out List<Coordinate> oArea)
        {
            bool bRet = false;
            oArea = null;
            System.Diagnostics.Trace.TraceInformation("Begin to GetArea \n");
            if (sGUID != null)
            {
                lock (_oAreaStoreDct)
                {
                      Types.Poly oPoly = _oAreaStoreDct[sGUID];
                      if (oPoly != null)
                      {
                          oArea = ConverttoArea(oPoly);
                      }
                      bRet = true;
                }
            }
            System.Diagnostics.Trace.TraceInformation("End GetArea \n");
            return bRet;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UpdateArea(string sGUID, List<Coordinate> oArea)
        {
            bool bRet = false;
            System.Diagnostics.Trace.TraceInformation("Begin to UpdateArea \n");
            if (sGUID != null)
            {
                lock (_oAreaStoreDct)
                {
                    if (_oAreaStoreDct.ContainsKey(sGUID))
                    {
                        _oAreaStoreDct[sGUID] = ConverttoLoc( oArea );
                        bRet = true;
                    }
                }
            }
            System.Diagnostics.Trace.TraceInformation("End UpdateArea \n");
            return bRet;
        }

        public string AddArea(List<Coordinate> oArea)
        {
           // 32 digits separated by hyphens xxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx 
            string sGUID = System.Guid.NewGuid().ToString("D") ;
            System.Diagnostics.Trace.TraceInformation("Begin to AddArea \n");
            lock (_oAreaStoreDct)
            {
                _oAreaStoreDct.Add( sGUID, ConverttoLoc (oArea ) );
            }
            System.Diagnostics.Trace.TraceInformation("End AddArea \n");
            return sGUID;
        }

        public bool DeleteArea(string sGUID)
        {
            bool bRet = false;
            System.Diagnostics.Trace.TraceInformation("Begin to UpdateArea \n");
            if (sGUID != null)
            {
                lock (_oAreaStoreDct)
                {
                    if (_oAreaStoreDct.ContainsKey(sGUID))
                    {
                        bRet = _oAreaStoreDct.Remove(sGUID);
                    }
                }
            }
            System.Diagnostics.Trace.TraceInformation("End UpdateArea \n");
            return bRet;
        }
        static public Types.Poly ConverttoLoc(Area oArea)
        {
            List<Types.Loc> oLocList = new List<Types.Loc>();
            
            foreach( Coordinate o in oArea)
            {
                oLocList.Add(new Types.Loc(o.Latitude, o.Longitude));
            }
            
            Types.Poly oPoly = new Types.Poly(oLocList);
            return oPoly;
        }

        static public Area ConverttoArea( Types.Poly  oloc )
        {
            Area oArea = new Area();

            foreach (Types.Loc o in oloc.getLocs() )
            {
                oArea.Add( new Coordinate( o.getX(), o.getY()) );
            }

            return oArea;
        }

    }
}
