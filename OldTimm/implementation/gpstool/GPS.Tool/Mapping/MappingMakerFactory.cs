using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.Tool.Mapping.Maker;

namespace GPS.Tool.Mapping
{
    class MappingMakerFactory
    {
        public static MappingMaker CreateMaker(MappingTables table)
        {
            MappingMaker maker = null;
            switch (table)
            {
                case MappingTables.BlockGroupMappings:
                    maker = new BlockGroupMappingMaker();
                    break;
                case MappingTables.MetropolitanCoreAreaBlockGroupMappings:
                    maker = new MetropolitanCoreAreaBlockGroupMappingMaker();
                    break;
                case MappingTables.MetropolitanCoreAreaZipCodeMappings:
                    maker = new MetropolitanCoreAreaZipCodeMappingMaker();
                    break;
                case MappingTables.UrbanAreaBlockGroupMappings:
                    maker = new UrbanAreaBlockGroupMappingMaker();
                    break;
                case MappingTables.UrbanAreaZipCodeMappings:
                    maker = new UrbanAreaZipCodeMappingMaker();
                    break;
                case MappingTables.BlockGroupBoxMappings:
                    maker = new BlockGroupBoxMappingMaker();
                    break;
                case MappingTables.TractBoxMappings:
                    maker = new TractBoxMappingMaker();
                    break;
                case MappingTables.FiveZipBoxMappings:
                    maker = new FiveZipBoxMappingMaker();
                    break;
                case MappingTables.ThreeZipBoxMappings:
                    maker = new ThreeZipBoxMappingMaker();
                    break;
                case MappingTables.CBSABoxMapping:
                    maker = new CBSABoxMappingMaker();
                    break;
                case MappingTables.UrbanAreaBoxMapping:
                    maker = new UrbanAreaBoxMappingMaker();
                    break;
                case MappingTables.CountyAreaBoxMapping:
                    maker = new CountyAreaBoxMappingMaker();
                    break;
                case MappingTables.ElementarySchoolBoxMapping:
                    maker = new ElementarySchoolBoxMappingMaker();
                    break;
                case MappingTables.SecondarySchoolBoxMapping:
                    maker = new SecondarySchoolBoxMappingMaker();
                    break;
                case MappingTables.UnifiedSchoolBoxMapping:
                    maker = new UnifiedSchoolBoxMappingMaker();
                    break;
                case MappingTables.LowerHouseBoxMapping:
                    maker = new LowerHousrBoxMappingMaker();
                    break;
                case MappingTables.UpperSenateBoxMapping:
                    maker = new UpperSenateBoxMappingMaker();
                    break;
                case MappingTables.PreminumZipBoxMapping:
                    maker = new PreminumZipBoxMappingMaker();
                    break;
                case MappingTables.PreminumCRouteBoxMapping:
                    maker = new PreminumCRouteBoxMappingMaker();
                    break;
                default:
                    break;
            }
            return maker;
        }
    }
}
