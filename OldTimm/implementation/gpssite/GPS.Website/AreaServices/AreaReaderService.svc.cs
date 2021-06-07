using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Area;
using System.Collections.Generic;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Area.Addresses;
using GPS.Website.TransferObjects;
using System.Configuration;
using System.Web;
using GPS.Website.AppFacilities;
using GPS.DomainLayer.Area.AreaOperators;
using GPS.DomainLayer.Area.AreaMerge;

namespace GPS.Website.AreaServices
{
    [ServiceContract(Namespace = "TIMM.Website.AreaServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AreaReaderService
    {
        /// <summary>
        /// Convert a list of <see cref="IArea"/> to a list of <see cref="GPSArea"/>.
        /// </summary>
        /// <param name="areas">An enumerable <see cref="IArea"/> list.</param>
        /// <returns>An enumerable <see cref="GPSArea"/> list.</returns>
        private IEnumerable<GPSArea> ConvertToGPSArea(IEnumerable<IArea> areas)
        {
            List<GPSArea> gpsAreas = new List<GPSArea>();

            foreach (IArea area in areas)
            {
                GPSArea gpsArea = new GPSArea();
                gpsArea.Id = area.Id;
                gpsArea.Name = area.Name;
                gpsArea.Classification = (Int32)area.Classification;
                //gpsArea.Relation = area.Relation;
                gpsArea.Location = new Double[] { area.Latitude, area.Longitude };
                gpsAreas.Add(gpsArea);
            }

            return gpsAreas;
        }
        /// <summary>
        /// Fetch the 5 digit zip areas which has the specified 5 digit zip code.
        /// </summary>
        /// <param name="zipCode">The 5 digit zip code.</param>
        /// <returns>A list of <see cref="GPSArea"/>.</returns> 
        [OperationContract]
        public IEnumerable<GPSArea> GetFiveZipByCode(String zipCode)
        {
            return ConvertToGPSArea(MapAreaManager.GetAreasByFiveZipCode(zipCode));
            //FiveZipAreaOperator oper = new FiveZipAreaOperator();
            //oper.Gets(zipCode);
        }

        /// <summary>
        /// Fetch the 5 digit zip areas which has the specified 5 digit zip code.
        /// </summary>
        /// <param name="zipCode">The 5 digit zip code.</param>
        /// <returns>A list of <see cref="GPSArea"/>.</returns> 
        [OperationContract]
        public IEnumerable<ToArea> GetFiveZipAreas(String zipCode)
        {
            FiveZipAreaOperator oper = new FiveZipAreaOperator();
            IEnumerable<MapArea> areas = oper.Gets(zipCode);
            Assembler<ToArea, MapArea> asm = AssemblerConfig.GetAssembler<ToArea, MapArea>();
            return asm.AssembleFrom(areas);
        }

        [OperationContract]
        public IEnumerable<ToArea> GetBlockGroupAreas(String bgCode)
        {
            BlockGroupOperator oper = new BlockGroupOperator();
            IEnumerable<MapArea> areas = oper.Gets(bgCode);
            Assembler<ToArea, MapArea> asm = AssemblerConfig.GetAssembler<ToArea, MapArea>();
            return asm.AssembleFrom(areas);
        }


        [OperationContract]
        public IEnumerable<ToArea> GetCrouteAreas(String code)
        {
            PremiumCRouteOperator oper = new PremiumCRouteOperator();
            IEnumerable<MapArea> areas = oper.Gets(code);
            Assembler<ToArea, MapArea> asm = AssemblerConfig.GetAssembler<ToArea, MapArea>();
            return asm.AssembleFrom(areas);
        }

        /// <summary>
        /// Fetch the 5 digit zip areas which has the specified 5 digit zip code.
        /// </summary>
        /// <param name="zipCode">The 5 digit zip code.</param>
        /// <returns>A list of <see cref="GPSArea"/>.</returns> 
        [OperationContract]
        public IEnumerable<ToArea> GetCampaignFiveZipAreas(int campaignId, String zipCode)
        {
            FiveZipAreaOperator oper = new FiveZipAreaOperator();
            IEnumerable<MapArea> areas = oper.Gets(campaignId, zipCode);
            Assembler<ToArea, MapArea> asm = AssemblerConfig.GetAssembler<ToArea, MapArea>();
            return asm.AssembleFrom(areas);
        }

        /// <summary>
        /// Fetch the 5 digit zip areas which has the specified 5 digit zip code.
        /// </summary>
        /// <param name="zipCode">The 5 digit zip code.</param>
        /// <returns>A list of <see cref="GPSArea"/>.</returns> 
        [OperationContract]
        public IEnumerable<ToArea> GetCampaignCRouteAreas(int campaignId, String geoCode)
        {
            PremiumCRouteOperator oper = new PremiumCRouteOperator();
            IEnumerable<MapArea> areas = oper.Gets(campaignId, geoCode);
            Assembler<ToArea, MapArea> asm = AssemblerConfig.GetAssembler<ToArea, MapArea>();
            return asm.AssembleFrom(areas);
        }

        /// <summary>
        /// Fetch the Census Tracts which matches the specified locator.
        /// </summary>
        /// <param name="locator">
        /// A <see cref="TractLocator"/> which specifies the properties of the 
        /// target Census Tracts.
        /// </param>
        /// <returns>
        /// A list of <see cref="GPSArea"/> matching the specified locator.
        /// </returns>
        [OperationContract]
        public IEnumerable<GPSArea> GetTracts(TractLocator locator)
        {
            return ConvertToGPSArea(MapAreaManager.GetTractAreas(locator.StateCode, locator.CountyCode, locator.TractCode));
        }

        /// <summary>
        /// Fetch the Census Block Groups which matches the specified locator.
        /// </summary>
        /// <param name="locator">
        /// A <see cref="BlockGroupLocator"/> which specifies the properties of the 
        /// target Census Block Groups.
        /// </param>
        /// <returns>
        /// A list of <see cref="GPSArea"/> matching the specified locator.
        /// </returns>
        [OperationContract]
        public IEnumerable<GPSArea> GetBlockGroups(BlockGroupLocator locator)
        {
            return ConvertToGPSArea(MapAreaManager.GetBlockGroupAreas(locator.StateCode, locator.CountyCode, locator.TractCode, locator.BgCode));
        }

        [OperationContract]
        public List<ToAddressRadius> ChangeRadius(double lat, double lon, List<ToAddressRadius> radiuses)
        {
            AddressOperator oper = new AddressOperator();
            ICoordinate center = new Coordinate(lat, lon);
            foreach (ToAddressRadius radius in radiuses)
            {
                radius.Relations = oper.GetRadiusRelations(center, radius.Length, radius.LengthMeasuresId);
            }
            return radiuses;
        }

        [OperationContract]
        public ToMergeResult MergeAreas(int campaignId, List<ToAreaRecord> toRecords)
        {
            IEnumerable<AreaRecord> records = AssemblerConfig.GetAssembler<AreaRecord, ToAreaRecord>().AssembleFrom(toRecords);
            MergeOperator oper = new MergeOperator();
            MergeResult result = oper.MergeAreas(campaignId, ref records);
            return AssemblerConfig.GetAssembler<ToMergeResult, MergeResult>().AssembleFrom(result);
        }
    }

    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class GPSArea
    {
        [DataMember]
        public Int32 Id
        {
            get;
            set;
        }
        [DataMember]
        public String Name
        {
            get;
            set;
        }
        [DataMember]
        public Int32 Classification
        {
            get;
            set;
        }
        [DataMember]
        public Double[] Location
        {
            get;
            set;
        }
        [DataMember]
        public List<List<String>> Relation
        {
            get;
            set;
        }
    }

    /// <summary>
    /// <see cref="TractLocator"/> represents an object containing the properties 
    /// used to locate a Census Tract in the system.
    /// </summary>
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class TractLocator
    {
        /// <summary>
        /// The state code of the tract.
        /// </summary>
        [DataMember]
        public String StateCode
        {
            get;
            set;
        }

        /// <summary>
        /// The county code of the tract.
        /// </summary>
        [DataMember]
        public String CountyCode
        {
            get;
            set;
        }

        /// <summary>
        /// The tract code of the tract.
        /// </summary>
        [DataMember]
        public String TractCode
        {
            get;
            set;
        }
    }

    /// <summary>
    /// <see cref="TractLocator"/> represents an object containing the properties 
    /// used to locate a Census Block Group in the system.
    /// </summary>
    /// <remarks>
    /// A Census Block Group has the same State Code, County Code and 
    /// Tract Code as the Census Tract that covers it. So this class derives
    /// from class <see cref="TractLocator"/>
    /// </remarks>
    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class BlockGroupLocator : TractLocator
    {
        /// <summary>
        /// The Block Group code of the Block Group.
        /// </summary>
        [DataMember]
        public String BgCode
        {
            get;
            set;
        }
    }
}
