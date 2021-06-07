using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace WHYTAlgorithmService.Geo
{
    using Area = List<Coordinate>;

    public class DNDAreaStore
    {
        static readonly DNDAreaStore s_Instance = new DNDAreaStore();
        static Dictionary<int, Types.Poly> _oDNDAreaStoreDct = new Dictionary<int, Types.Poly>();
        static Dictionary<int, string> _oDNDAreaInfoDct = new Dictionary<int, string>();

        /// <summary>
        /// Singleton class
        /// </summary>
        public static DNDAreaStore Instance
        {
            get
            {
                return s_Instance;
            }
        }

        public void GetAllDNDAreas()
        {
            //Dictionary<int, List<Coordinate>> _oDNDAreaDct = DNDAreaDao.AvailableDNDArea();
            Dictionary<int, List<Coordinate>> _oDNDAreaDct = DaoFactory.CreateInstance<IDNDAreaDao>().AvailableDNDArea();
            _oDNDAreaStoreDct.Clear();
            foreach (int id in _oDNDAreaDct.Keys)
            {
                _oDNDAreaStoreDct.Add(id, ConverttoLoc(_oDNDAreaDct[id]));
            }
        }

        public void UpdateDNDAreaStoreDct()
        {
            //_oDNDAreaStoreDct.Clear();
            GetAllDNDAreas();
        }

        public bool GetArea(int areaId, out List<Coordinate> oArea)
        {
            bool bRet = false;
            oArea = null;
            System.Diagnostics.Trace.TraceInformation("Begin to GetArea \n");
            if (areaId != null)
            {
                lock (_oDNDAreaStoreDct)
                {
                    Types.Poly oPoly = _oDNDAreaStoreDct[areaId];
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

        static public Types.Poly ConverttoLoc(Area oArea)
        {
            List<Types.Loc> oLocList = new List<Types.Loc>();

            foreach (Coordinate o in oArea)
            {
                oLocList.Add(new Types.Loc(o.Latitude, o.Longitude));
            }

            Types.Poly oPoly = new Types.Poly(oLocList);
            return oPoly;
        }

        static public Area ConverttoArea(Types.Poly oloc)
        {
            Area oArea = new Area();

            foreach (Types.Loc o in oloc.getLocs())
            {
                oArea.Add(new Coordinate(o.getX(), o.getY()));
            }

            return oArea;
        }

    }

    public class DNDAreaInfoDctValue
    {
        Types.Poly areaPoly;
        string areaInfo;

    }
}
