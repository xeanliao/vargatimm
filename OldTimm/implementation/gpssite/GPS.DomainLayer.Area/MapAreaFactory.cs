using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Enum;
using GPS.DataLayer;
using Newtonsoft.Json;
using System.Web;
using System.Data;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Area
{
    public class MapAreaFactory
    {

        /// <summary>
        /// Create Areas by classification and box
        /// </summary>
        /// <param name="classification">the classification</param>
        /// <param name="boxId">the box id</param>
        /// <returns>the areas</returns>
        public static IEnumerable<IArea> CreateAreaList(Classifications classification, int boxId)
        {
            IEnumerable<IArea> areas = null;
            switch (classification)
            {
                case Classifications.CBSA:
                    areas = GetCbsaAreas(boxId);
                    break;
                case Classifications.County:
                    areas = GetCountyAreas(boxId);
                    break;
                case Classifications.Urban:
                    areas = GetUrbanAreas(boxId);
                    break;
                case Classifications.SLD_House:
                    areas = GetLowerHouseAreas(boxId);
                    break;
                case Classifications.SLD_Senate:
                    areas = GetUpperSenateAreas(boxId);
                    break;
                case Classifications.SD_Elem:
                    areas = GetElementarySchoolAreas(boxId);
                    break;
                case Classifications.SD_Secondary:
                    areas = GetSecondarySchoolAreas(boxId);
                    break;
                case Classifications.SD_Unified:
                    areas = GetUnifiedSchoolAreas(boxId);
                    break;
                case Classifications.Custom:
                    areas = GetCustomAreas(boxId);
                    break;
                case Classifications.Address:
                    areas = GetNdAddresses(boxId);
                    break;
                default:
                    AreaOperatorBase oper = AreaOperatorFacory.CreateOperator(classification);
                    if (oper != null)
                    {
                        areas = oper.GetBoxAreas(boxId);
                    }
                    else
                    {
                        areas = new List<IArea>();
                    }
                    break;
            }

            return areas;
        }

        /// <summary>
        /// Create Areas by classification, box and campaign
        /// </summary>
        /// <param name="classification">the classification</param>
        /// <param name="boxId">the box id</param>
        /// <returns>the areas</returns>
        /// <param name="campaignId">the campaign</param>
        public static IEnumerable<IArea> CreateAreaList(Classifications classification,
           int boxId, int campaignId)
        {
            IEnumerable<IArea> areas = null;
            AreaOperatorBase oper = AreaOperatorFacory.CreateOperator(classification);
            if (oper != null)
            {
                areas = oper.GetBoxAreas(campaignId, boxId);
            }
            else
            {
                areas = new List<IArea>();
            }

            return areas;
        }

        #region Get Area By Box
  
        /// <summary>
        /// Get Three Zip Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetThreeZipAreasByBoxId(int boxId)
        {
            ThreeZipRepository rep = new ThreeZipRepository();
            var threeZipAreas = rep.GetEntityList(boxId).ToList();

            return ConvertEntityToAreas(threeZipAreas);
        }

        /// <summary>
        /// Convert Three Zip List to Areas
        /// </summary>
        /// <param name="entityList">the Three Zip List</param>
        /// <returns>the areas</returns>
        private static List<IArea> ConvertEntityToAreas(List<ThreeZipArea> entityList)
        {
            List<IArea> areas = new List<IArea>();
            foreach (var entity in entityList)
            {
                areas.Add(new MapArea(entity));
            }
            return areas;
        }

        /// <summary>
        /// Get CBSA Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetCbsaAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            CbsaBoxMappingRepository cbsaRep = new CbsaBoxMappingRepository();
            var cbsaList = cbsaRep.GetBoxMapping(boxId)
                .Select(b => b.MetropolitanCoreArea).Distinct().ToList();
            foreach (MetropolitanCoreArea cbsaArea in cbsaList)
            {
                areas.Add(new MapArea(cbsaArea));
            }

            return areas;
        }

        /// <summary>
        /// Get County Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetCountyAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            CountyAreaBoxMappingRepository countyAreaRep =
                new CountyAreaBoxMappingRepository();
            var countyAreas = countyAreaRep.GetBoxMapping(boxId)
                .Select(b => b.CountyArea).Distinct().ToList();
            foreach (CountyArea countyArea in countyAreas)
            {
                areas.Add(new MapArea(countyArea));
            }

            return areas;
        }

        /// <summary>
        /// Get Urban Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetUrbanAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            UrbanBoxMappingRepository urbanRep = new UrbanBoxMappingRepository();
            var urbanAreas = urbanRep.GetBoxMapping(boxId)
                .Select(b => b.UrbanArea).Distinct().ToList();
            foreach (UrbanArea urbanArea in urbanAreas)
            {
                areas.Add(new MapArea(urbanArea));
            }

            return areas;
        }

        /// <summary>
        /// Get Lower House Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetLowerHouseAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            LowerHouseBoxMappingRepository houseRep = new LowerHouseBoxMappingRepository();
            var houseAreas = houseRep.GetBoxMapping(boxId)
                .Select(b => b.LowerHouseArea).Distinct().ToList();
            foreach (LowerHouseArea houseArea in houseAreas)
            {
                areas.Add(new MapArea(houseArea));
            }

            return areas;
        }

        /// <summary>
        /// Get Upper Senate Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetUpperSenateAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            UpperSenateBoxMappingRepository senateRep = new UpperSenateBoxMappingRepository();
            var senateAreas = senateRep.GetBoxMapping(boxId)
                .Select(b => b.UpperSenateArea).Distinct().ToList();
            foreach (UpperSenateArea senateArea in senateAreas)
            {
                areas.Add(new MapArea(senateArea));
            }

            return areas;
        }

        /// <summary>
        /// Get Elementary School Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetElementarySchoolAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            ElementarySchoolBoxMappingRepository eleSchoolRep = new ElementarySchoolBoxMappingRepository();
            var eleSchoolAreas = eleSchoolRep.GetBoxMapping(boxId)
                .Select(b => b.ElementarySchoolArea).Distinct().ToList();
            foreach (ElementarySchoolArea eleSchool in eleSchoolAreas)
            {
                areas.Add(new MapArea(eleSchool));
            }

            return areas;
        }

        /// <summary>
        /// Get Secondary School Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetSecondarySchoolAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            SecondarySchoolBoxMappingRepository secSchoolRep = new SecondarySchoolBoxMappingRepository();
            var secSchoolAreas = secSchoolRep.GetBoxMapping(boxId)
                .Select(b => b.SecondarySchoolArea).Distinct().ToList();
            foreach (SecondarySchoolArea secSchool in secSchoolAreas)
            {
                areas.Add(new MapArea(secSchool));
            }

            return areas;
        }

        /// <summary>
        /// Get Unified School Areas by id
        /// </summary>
        /// <param name="boxId">the box'id</param>
        /// <returns>the areas</returns>
        private static List<IArea> GetUnifiedSchoolAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            UnifiedSchoolBoxMappingRepository uniSchoolRep = new UnifiedSchoolBoxMappingRepository();
            var uniSchoolAreas = uniSchoolRep.GetBoxMapping(boxId)
                .Select(b => b.UnifiedSchoolArea).Distinct().ToList();
            foreach (UnifiedSchoolArea uniSchool in uniSchoolAreas)
            {
                areas.Add(new MapArea(uniSchool));
            }

            return areas;
        }

        private static List<IArea> GetCustomAreas(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            CustomAreaRepository repository = new CustomAreaRepository();
            List<CustomArea> customAreas = repository.GetCustomAreas(boxId);
            foreach (CustomArea customArea in customAreas)
            {
                areas.Add(new MapArea(customArea));
            }
            return areas;
        }

        private static List<IArea> GetNdAddresses(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            NdAddressRepository repository = new NdAddressRepository();
            IList<NdAddress> addresses = repository.GetNdAddresses(boxId);
            foreach (NdAddress address in addresses)
            {
                areas.Add(new MapArea(address));
            }
            return areas;
        }

        private static List<IArea> GetPremiumCRoutes(int boxId)
        {
            List<IArea> areas = new List<IArea>();
            PremiumCRouteRepository repository = new PremiumCRouteRepository();
            List<PremiumCRoute> cRoutes = repository.GetPremiumCRoutes(boxId);
            foreach (PremiumCRoute cRoute in cRoutes)
            {
                areas.Add(new MapArea(cRoute));
            }
            return areas;
        }
        #endregion
    }
}
