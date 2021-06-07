using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Enum;
using GPS.DataLayer;
using System.Runtime.Serialization;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Area
{
    public class MapArea : IArea, IBoxArea
    {
        #region Property

        private Classifications _classification;
        private int _id;
        private string _name;
        private string _state;
        Dictionary<string, string> _attributes = new Dictionary<string, string>();
        private List<ICoordinate> _locations;
        private IGPSColor _fillColor;
        private IGPSColor _lineColor;
        private bool _isExportData;
        private double _minLongitude;
        private double _maxLongitude;
        private double _minLatitude;
        private double _maxLatitude;
        private double _latitude;
        private double _longitude;
        private bool _isEnabled = true;
        private string _description;
        private List<List<string>> _relation;
        private Dictionary<int, Dictionary<int, bool>> _relations;
        #endregion

        #region Constructors
        public MapArea()
        {
            _locations = new List<ICoordinate>();
            _attributes = new Dictionary<string, string>();
        }

        public MapArea(ThreeZipArea threeZipArea)
        {
            InitializeThreeZipExpand(threeZipArea);
            InitializeDefaultProperty(Classifications.Z3,
                threeZipArea.Id.ToString(),
                threeZipArea.Name,
                "06",
                threeZipArea.Latitude,
                threeZipArea.Longitude,
                threeZipArea.MinLatitude,
                threeZipArea.MinLongitude,
                threeZipArea.MaxLatitude,
                threeZipArea.MaxLongitude,
                false,
                null);
        }
        /// <summary>
        /// Construct the Map Area by County Area
        /// </summary>
        /// <param name="countyArea">the county area</param>
        public MapArea(CountyArea countyArea)
        {
            InitializeCountyExpand(countyArea);
            InitializeDefaultProperty(Classifications.County,
                countyArea.Id.ToString(),
                countyArea.Name,
                countyArea.StateCode,
                countyArea.Latitude,
                countyArea.Longitude,
                countyArea.MinLatitude,
                countyArea.MinLongitude,
                countyArea.MaxLatitude,
                countyArea.MaxLongitude,
                false,
                null);
        }

        /// <summary>
        /// Construct the Map Area by CBSA Area
        /// </summary>
        /// <param name="cbsaArea">the cbsa area</param>
        public MapArea(MetropolitanCoreArea cbsaArea)
        {
            InitializeCbsaExpand(cbsaArea);
            InitializeDefaultProperty(Classifications.CBSA,
                cbsaArea.Id.ToString(),
                cbsaArea.Name,
                String.Empty,
                cbsaArea.Latitude,
                cbsaArea.Longitude,
                cbsaArea.MinLatitude,
                cbsaArea.MinLongitude,
                cbsaArea.MaxLatitude,
                cbsaArea.MaxLongitude,
                false,
                null);
        }

        /// <summary>
        /// Construct the Map Area by Urban Area
        /// </summary>
        /// <param name="urbanArea">the urban area</param>
        public MapArea(UrbanArea urbanArea)
        {
            InitializeUrbanExpand(urbanArea);
            InitializeDefaultProperty(Classifications.Urban,
                urbanArea.Id.ToString(),
                urbanArea.Name,
                String.Empty,
                urbanArea.Latitude,
                urbanArea.Longitude,
                urbanArea.MinLatitude,
                urbanArea.MinLongitude,
                urbanArea.MaxLatitude,
                urbanArea.MaxLongitude,
                false,
                null);
        }

        /// <summary>
        /// Construct the Map Area by House Area
        /// </summary>
        /// <param name="houseArea">the house area</param>
        public MapArea(LowerHouseArea houseArea)
        {
            InitializeHouseExpand(houseArea);
            InitializeDefaultProperty(Classifications.SLD_House,
                houseArea.Id.ToString(),
                houseArea.Name,
                String.Empty,
                houseArea.Latitude,
                houseArea.Longitude,
                houseArea.MinLatitude,
                houseArea.MinLongitude,
                houseArea.MaxLatitude,
                houseArea.MaxLongitude,
                false, null);
        }

        /// <summary>
        /// Construct the Map Area by Upper Senate Area
        /// </summary>
        /// <param name="upperSenateArea">the Senate Area</param>
        public MapArea(UpperSenateArea upperSenateArea)
        {
            InitializeSenateExpand(upperSenateArea);
            InitializeDefaultProperty(Classifications.SLD_Senate,
                upperSenateArea.Id.ToString(),
                upperSenateArea.Name,
                String.Empty,
                upperSenateArea.Latitude,
                upperSenateArea.Longitude,
                upperSenateArea.MinLatitude,
                upperSenateArea.MinLongitude,
                upperSenateArea.MaxLatitude,
                upperSenateArea.MaxLongitude,
                false, null);
        }

        /// <summary>
        /// Construct the Map Area by Elementary School Area
        /// </summary>
        /// <param name="eleSchoolArea">the ElementarySchool Area</param>
        public MapArea(ElementarySchoolArea eleSchoolArea)
        {
            InitializeEleSchoolExpand(eleSchoolArea);
            InitializeDefaultProperty(Classifications.SD_Elem,
                eleSchoolArea.Id.ToString(),
                eleSchoolArea.Name,
                String.Empty,
                eleSchoolArea.Latitude,
                eleSchoolArea.Longitude,
                eleSchoolArea.MinLatitude,
                eleSchoolArea.MinLongitude,
                eleSchoolArea.MaxLatitude,
                eleSchoolArea.MaxLongitude,
                false, null);
        }

        /// <summary>
        /// Construct the Map Area by Secondary School Area
        /// </summary>
        /// <param name="secSchoolArea">the Secondary School Area</param>
        public MapArea(SecondarySchoolArea secSchoolArea)
        {
            InitializeSecSchoolExpand(secSchoolArea);
            InitializeDefaultProperty(Classifications.SD_Secondary,
                secSchoolArea.Id.ToString(),
                secSchoolArea.Name,
                String.Empty,
                secSchoolArea.Latitude,
                secSchoolArea.Longitude,
                secSchoolArea.MinLatitude,
                secSchoolArea.MinLongitude,
                secSchoolArea.MaxLatitude,
                secSchoolArea.MaxLongitude,
                false, null);
        }

        /// <summary>
        /// Construct the Map Area by Unified School Area
        /// </summary>
        /// <param name="unifiedSchoolArea">the unified school area</param>
        public MapArea(UnifiedSchoolArea uniSchoolArea)
        {
            InitializeUniSchoolExpand(uniSchoolArea);
            InitializeDefaultProperty(Classifications.SD_Unified,
                uniSchoolArea.Id.ToString(),
                uniSchoolArea.Name,
                String.Empty,
                uniSchoolArea.Latitude,
                uniSchoolArea.Longitude,
                uniSchoolArea.MinLatitude,
                uniSchoolArea.MinLongitude,
                uniSchoolArea.MaxLatitude,
                uniSchoolArea.MaxLongitude,
                false, null);
        }

        public MapArea(CustomArea customArea)
        {
            this._id = customArea.Id;
            this._classification = Classifications.Custom;
            this._name = customArea.Name;
            this._description = customArea.Description;
            this._isEnabled = customArea.IsEnabled;
            this._attributes.Add("OTotal", customArea.total.ToString());
            _locations = new List<ICoordinate>();
            foreach (CustomAreaCoordinate coordinate in customArea.CustomAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        public MapArea(NdAddress address)
        {
            this._id = address.Id;
            this._classification = Classifications.Address;
            this._name = address.Street + "(" + address.ZipCode + ")";
            this._latitude = address.Latitude;
            this._longitude = address.Longitude;
            this._description = address.Description;
            this._attributes.Add("Geofence", address.Geofence.ToString());
            this._isEnabled = false;
            _locations = new List<ICoordinate>();
            foreach (NdAddressCoordinate coordinate in address.NdAddressCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        public MapArea(PremiumCRoute cRoute)
        {
            this._id = cRoute.Id;
            this._classification = Classifications.PremiumCRoute;
            this._name = cRoute.CROUTE;
            this._description = cRoute.Description;
            this._attributes.Add("Code", cRoute.CROUTE);
            this._attributes.Add("Zip", cRoute.ZIP);
            this._isEnabled = cRoute.IsEnabled;
            var items = cRoute.PremiumCRouteCoordinates.OrderBy(t => t.Id);
            _locations = new List<ICoordinate>();
            foreach (PremiumCRouteCoordinate coordinate in items)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }
        }

        /// <summary>
        /// Initialize Map Area for Unified School
        /// </summary>
        /// <param name="uniSchoolArea">the unified school area</param>
        private void InitializeUniSchoolExpand(UnifiedSchoolArea uniSchoolArea)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("StateCode", uniSchoolArea.StateCode);

            _locations = new List<ICoordinate>();
            foreach (UnifiedSchoolAreaCoordinate coordinate in
                uniSchoolArea.UnifiedSchoolAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for Secondary School
        /// </summary>
        /// <param name="secSchoolArea">the Secondary School Area</param>
        private void InitializeSecSchoolExpand(SecondarySchoolArea secSchoolArea)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("StateCode", secSchoolArea.StateCode);

            _locations = new List<ICoordinate>();
            foreach (SecondarySchoolAreaCoordinate coordinate in
                secSchoolArea.SecondarySchoolAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for Elementary School
        /// </summary>
        /// <param name="eleSchoolArea">the ElementarySchool Area</param>
        private void InitializeEleSchoolExpand(ElementarySchoolArea eleSchoolArea)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("StateCode", eleSchoolArea.StateCode);

            _locations = new List<ICoordinate>();
            foreach (ElementarySchoolAreaCoordinate coordinate in
                eleSchoolArea.ElementarySchoolAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for Upper Senate
        /// </summary>
        /// <param name="upperSenateArea">the senate area</param>
        private void InitializeSenateExpand(UpperSenateArea upperSenateArea)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("StateCode", upperSenateArea.StateCode);

            _locations = new List<ICoordinate>();
            foreach (UpperSenateAreaCoordinate coordinate in
                upperSenateArea.UpperSenateAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for Lower House
        /// </summary>
        /// <param name="houseArea">the house area</param>
        private void InitializeHouseExpand(LowerHouseArea houseArea)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("StateCode", houseArea.StateCode);

            _locations = new List<ICoordinate>();
            foreach (LowerHouseAreaCoordinate coordinate in
                houseArea.LowerHouseAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for Urban
        /// </summary>
        /// <param name="urbanArea">the Urban area</param>
        private void InitializeUrbanExpand(UrbanArea urbanArea)
        {
            this._attributes = new Dictionary<string, string>();

            _locations = new List<ICoordinate>();
            foreach (UrbanAreaCoordinate coordinate in
                urbanArea.UrbanAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for CBSA
        /// </summary>
        /// <param name="cbsaArea">the cbsa area</param>
        private void InitializeCbsaExpand(MetropolitanCoreArea cbsaArea)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("Status", cbsaArea.Status);

            _locations = new List<ICoordinate>();
            foreach (MetropolitanCoreAreaCoordinate coordinate in
                cbsaArea.MetropolitanCoreAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }
        }

        /// <summary>
        /// Initialize Map Area for County
        /// </summary>
        /// <param name="cbsaArea">the County area</param>
        private void InitializeCountyExpand(CountyArea countyArea)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("County", countyArea.Code);

            _locations = new List<ICoordinate>();
            foreach (CountyAreaCoordinate coordinate in countyArea.CountyAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for BlockGroup
        /// </summary>
        /// <param name="cbsaArea">the BlockGroup area</param>
        private void InitializeBGExpand(BlockGroup blockGroup)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("State", blockGroup.StateCode);
            this._attributes.Add("County", blockGroup.CountyCode);
            this._attributes.Add("Tract", blockGroup.TractCode);
            this._attributes.Add("BlockGroup", blockGroup.Code);
            //this._attributes.Add("CountyName", blockGroup.CountyCode);

            _locations = new List<ICoordinate>();
            foreach (BlockGroupCoordinate coordinate in blockGroup.BlockGroupCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for FiveZip
        /// </summary>
        /// <param name="cbsaArea">the FiveZip area</param>
        private void InitializeFiveZipExpand(FiveZipArea fiveZipArea)
        {
            _locations = new List<ICoordinate>();
            var items = fiveZipArea.FiveZipAreaCoordinates.OrderBy(t => t.Id);
            foreach (FiveZipAreaCoordinate coordinate in items)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }
        }

        /// <summary>
        /// Initialize Map Area for Tract
        /// </summary>
        /// <param name="cbsaArea">the Tract area</param>
        private void InitializeTractExpand(Tract tract)
        {
            this._attributes = new Dictionary<string, string>();
            this._attributes.Add("State", tract.StateCode);
            this._attributes.Add("County", tract.CountyCode);
            this._attributes.Add("Tract", tract.Code);
            //this._attributes.Add("CountyName", tract.CountyCode);

            _locations = new List<ICoordinate>();
            foreach (TractCoordinate coordinate in tract.TractCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude));
            }
        }

        /// <summary>
        /// Initialize Map Area for three zip
        /// </summary>
        /// <param name="threeZipArea">the three zip area</param>
        private void InitializeThreeZipExpand(ThreeZipArea threeZipArea)
        {
            _locations = new List<ICoordinate>();
            foreach (ThreeZipAreaCoordinate coordinate in threeZipArea.ThreeZipAreaCoordinates)
            {
                _locations.Add(new Coordinate(coordinate.Latitude, coordinate.Longitude, coordinate.ShapeId));
            }
        }

        /// <summary>
        /// Initialize Five Zip Area
        /// </summary>
        /// <param name="fiveZipArea">the five Zip Area </param>
        /// <param name="isExport">the export data</param>
        private void InitializeFiveZipArea(FiveZipArea fiveZipArea, bool isExport)
        {
            List<List<string>> relation = new List<List<string>>();

            relation.Add(CampaignRecord.CalculateIds(
                new List<int> { fiveZipArea.ThreeZipId }, Classifications.Z3));

            InitializeDefaultProperty(Classifications.Z5,
                fiveZipArea.Id.ToString(),
                fiveZipArea.Name,
                fiveZipArea.StateCode,
                fiveZipArea.Latitude,
                fiveZipArea.Longitude,
                fiveZipArea.MinLatitude,
                fiveZipArea.MinLongitude,
                fiveZipArea.MaxLatitude,
                fiveZipArea.MaxLongitude,
                isExport,
                relation);
            InitializeFiveZipExpand(fiveZipArea);
            this._isEnabled = fiveZipArea.IsEnabled;
            this._description = fiveZipArea.Description;
            this._attributes.Add("OTotal", fiveZipArea.OTotal.ToString());
        }

        /// <summary>
        /// Initialize block group Area
        /// </summary>
        /// <param name="fiveZipArea">the block group Area </param>
        /// <param name="isExport">the export data</param>
        private void InitializeBGArea(BlockGroup blockGroup, bool isExport)
        {
            List<List<string>> relation = new List<List<string>>();

            relation.Add(CampaignRecord.CalculateIds(blockGroup.ThreeZipIds, Classifications.Z3));
            relation.Add(CampaignRecord.CalculateIds(blockGroup.FiveZipIds, Classifications.Z5));
            relation.Add(CampaignRecord.CalculateIds(
                new List<int> { blockGroup.tractId }, Classifications.TRK));

            InitializeDefaultProperty(Classifications.BG,
                blockGroup.Id.ToString(),
                blockGroup.Name,
                blockGroup.StateCode,
                blockGroup.Latitude,
                blockGroup.Longitude,
                blockGroup.MinLatitude,
                blockGroup.MinLongitude,
                blockGroup.MaxLatitude,
                blockGroup.MaxLongitude,
                isExport,
                relation);
            InitializeBGExpand(blockGroup);
            this._isEnabled = blockGroup.IsEnabled;
            this._description = blockGroup.Description;
            this._attributes.Add("OTotal", blockGroup.OTotal.ToString());
        }

        /// <summary>
        /// Initialize tract Area
        /// </summary>
        /// <param name="fiveZipArea">the tract Area </param>
        /// <param name="isExport">the export data</param>
        private void InitializeTractArea(Tract tract, bool isExport)
        {
            List<List<string>> relation = new List<List<string>>();

            relation.Add(CampaignRecord.CalculateIds(tract.ThreeZipIds, Classifications.Z3));
            relation.Add(CampaignRecord.CalculateIds(tract.FiveZipIds, Classifications.Z5));

            InitializeTractExpand(tract);
            InitializeDefaultProperty(Classifications.TRK,
                tract.Id.ToString(),
                tract.Name,
                tract.StateCode,
                tract.Latitude,
                tract.Longitude,
                tract.MinLatitude,
                tract.MinLongitude,
                tract.MaxLatitude,
                tract.MaxLongitude,
                isExport,
                relation);
            this._isEnabled = tract.IsEnabled;
            this._description = tract.Description;
            this._attributes.Add("OTotal", tract.OTotal.ToString());
        }

        /// <summary>
        /// Initialize Map Area' color
        /// </summary>
        private void InitializeDefaultProperty(
            Classifications classification,
            String id,
            String name,
            String stateCode,
            Double pLatitude,
            Double pLongitude,
            Double pMinLatitude,
            Double pMinLongitude,
            Double pMaxLatitude,
            Double pMaxLongitude,
            bool isExport,
            List<List<string>> pParentIds
            )
        {
            this._classification = classification;
            //this._id = id;
            this._name = name;
            this._state = stateCode;

            if (pParentIds != null)
            {
                this._relation = new List<List<string>>();
                foreach (List<string> parentIds in pParentIds)
                {
                    List<string> ids = new List<string>();
                    foreach (string parentId in parentIds)
                        ids.Add(parentId);
                    this._relation.Add(ids);
                }
            }
            this._latitude = pLatitude;
            this._longitude = pLongitude;
            this._minLatitude = pMinLatitude;
            this._minLongitude = pMinLongitude;
            this._maxLatitude = pMaxLatitude;
            this._maxLongitude = pMaxLongitude;

            if (!isExport)
            {
                this._fillColor = MapColor.GetFillColor(this._classification);
                this._lineColor = MapColor.GetLineColor(this._classification);
            }
        }

        #endregion

        #region IArea Members

        public int Id
        {
            get
            {
                //if (this._classification == Classifications.Custom || this._classification == Classifications.Address)
                //{
                //    return this._id;
                //}
                //else
                //{
                //    string shapeId = "{0}${1}${2}";
                //    shapeId = String.Format(shapeId,
                //        ((int)_classification).ToString(),
                //        //_state,
                //        "06",
                //        _id);
                //    return shapeId;
                //}
                return this._id;
            }
            set { _id = value; }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public GPS.DomainLayer.Enum.Classifications Classification
        {
            get
            {
                return _classification;
            }
            set
            {
                _classification = value;
            }
        }

        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public List<ICoordinate> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
            }
        }

        /// <summary>
        /// fill color of the area
        /// </summary>
        public IGPSColor FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
            }
        }

        /// <summary>
        /// outline color of the area
        /// </summary>
        public IGPSColor LineColor
        {
            get
            {
                return _lineColor;
            }
            set
            {
                _lineColor = value;
            }
        }

        public Dictionary<string, string> Attributes
        {
            get
            {
                return _attributes;
            }
            set
            {
                _attributes = value;
            }
        }

       #endregion

        #region ICoordinateArea Members


        public double MinLongitude
        {
            get
            {
                return this._minLongitude;
            }
            set
            {
                this._minLongitude = value;
            }
        }

        public double MaxLongitude
        {
            get
            {
                return this._maxLongitude;
            }
            set
            {
                this._maxLongitude = value;
            }
        }

        public double MinLatitude
        {
            get
            {
                return this._minLatitude;
            }
            set
            {
                this._minLatitude = value;
            }
        }

        public double MaxLatitude
        {
            get
            {
                return this._maxLatitude;
            }
            set
            {
                this._maxLatitude = value;
            }
        }

        public double Latitude
        {
            get
            {
                return this._latitude;
            }
            set
            {
                this._latitude = value;
            }
        }

        public double Longitude
        {
            get
            {
                return this._longitude;
            }
            set
            {
                this._longitude = value;
            }
        }

        #endregion

        #region IArea Members


        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public Dictionary<int, Dictionary<int, bool>> Relations
        {
            get
            {
                return _relations;
            }
            set
            {
                _relations = value;
            }
        }

        #endregion
    }
}
