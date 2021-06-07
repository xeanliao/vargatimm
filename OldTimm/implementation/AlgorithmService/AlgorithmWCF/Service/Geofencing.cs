using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IdentityModel.Selectors;


namespace WHYTAlgorithmService.Geo
{
    using Area = List<Coordinate>;

    public class Geofencing : IGeofencing
    {
        public bool IsInTheArea( Coordinate oLocation, string sShapeGUID )
        {
            bool bRet = false;
            List<Coordinate> oArea = null;
            bRet = AreaStore.Instance.GetArea(sShapeGUID, out oArea);
            if( bRet)
            {
                Types.Poly oPoly = AreaStore.ConverttoLoc(oArea);
                Types.Loc loc = new Types.Loc( oLocation.Latitude, oLocation.Longitude );
                bRet = GeoUtils.inPoly(loc, oPoly);
            }
            return bRet;
        }

        public string IsInTheDNDArea(Coordinate oLocation, List<int> ndAreaIds)
        {
            string retStr = "";
            bool bRet = false;
            List<Coordinate> oArea = null;
            foreach(int aid in ndAreaIds){
                bRet = DNDAreaStore.Instance.GetArea(aid,out oArea);
                if (bRet)
                {
                    Types.Poly oPoly = AreaStore.ConverttoLoc(oArea);
                    Types.Loc loc = new Types.Loc(oLocation.Latitude, oLocation.Longitude);
                    bRet = GeoUtils.inPoly(loc, oPoly);
                }
                if (bRet) retStr = aid + "&";
            }
            return retStr;
        }

        public string RegisterArea( List<Coordinate> oArea )
        {
            return AreaStore.Instance.AddArea( oArea );
        }

        public bool UpdateArea( string sGUID, Area oArea )
        {
            return AreaStore.Instance.UpdateArea( sGUID, oArea ) ;
        }
        
        public bool RemoveArea( string sGUID )
        {
            return AreaStore.Instance.DeleteArea(sGUID);
        }
    }
}
