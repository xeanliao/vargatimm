using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.Interfaces;
using GPS.DataLayer;

namespace GPS.DomainLayer.QuerySpecifications
{
    public class ImportedDataSpecification
    {
        #region Interfaces
        public IEnumerable<ToAreaData> GetImportedData(IEnumerable<CsvAreaRecord> records, IFiveZipRepository repository)
        {
            // Fetch FiveZipArea according to Codes
            String[] codes = records.Select<CsvAreaRecord, String>(t => t.Code).ToArray();
            IEnumerable<FiveZipArea> items = repository.GetShapesAccordingToCodes(codes);

            // Transform FiveZipArea to target type
            return new List<FiveZipArea>(items).ConvertAll(new Converter<FiveZipArea, ToAreaData>(
                delegate(FiveZipArea fza)
                {
                    CsvAreaRecord r = records.Last(t => t.Code.Equals(fza.Code));
                    return ConvertToTad(fza, r);
                }));
        }

        public IEnumerable<ToAreaData> GetImportedData(IEnumerable<CsvAreaRecord> records, ITractRepository repository)
        {
            // Fetch Tract according to Codes
            String[] codes = records.Select<CsvAreaRecord, String>(t => t.Code).ToArray();
            IEnumerable<Tract> items = repository.GetShapesAccordingToCodes(codes);

            // Transform Tract to target type
            return new List<Tract>(items).ConvertAll(new Converter<Tract, ToAreaData>(
                delegate(Tract trk)
                {
                    CsvAreaRecord r = records.Last(t => t.Code.Equals(trk.ArbitraryUniqueCode));
                    return ConvertToTad(trk, r);
                }));
        }

        public IEnumerable<ToAreaData> GetImportedData(IEnumerable<CsvAreaRecord> records, IBlockGroupRepository repository)
        {
            // Fetch BlockGroup according to Codes
            String[] codes = records.Select<CsvAreaRecord, String>(t => t.Code).ToArray();
            IEnumerable<BlockGroup> items = repository.GetShapesAccordingToCodes(codes);

            // Transform BlockGroup to target type
            return new List<BlockGroup>(items).ConvertAll(new Converter<BlockGroup, ToAreaData>(
                delegate(BlockGroup blockGroup)
                {
                    CsvAreaRecord r = records.Last(t => t.Code.Equals(blockGroup.ArbitraryUniqueCode));
                    return ConvertToTad(blockGroup, r);
                }));
        }

        public IEnumerable<ToAreaData> GetImportedData(IEnumerable<CsvAreaRecord> records, IPremiumCRouteRepository repository)
        {
            // Fetch PremiumCRoute according to Codes
            String[] codes = records.Select<CsvAreaRecord, String>(t => t.Code).ToArray();
            IEnumerable<PremiumCRoute> items = repository.GetShapesAccordingToCodes(codes);

            // Transform PremiumCRoute to target type
            return new List<PremiumCRoute>(items).ConvertAll(new Converter<PremiumCRoute, ToAreaData>(
                delegate(PremiumCRoute cRoute)
                {
                    CsvAreaRecord r = records.Last(t => t.Code.Equals(cRoute.Code));
                    return ConvertToTad(cRoute, r);
                   
                }));
        }

        #endregion

        #region Implementation
        private ToAreaData ConvertToTad(FiveZipArea fza, CsvAreaRecord r)
        {
            Int32 count = Int32.Parse(r.Penetration);
            Int32 total = Int32.Parse(r.Total);

            return new ToAreaData()
            {
                Id = fza.Id,
                Count = count,
                Total = total,
                Penetration = 0 != total ? (Double)count / (Double)total : 0,
                BoxIds = fza.FiveZipBoxMappings.Select<FiveZipBoxMapping, Int32>(t => t.BoxId),
                PremiumCRouteSelectMappings = fza.PremiumCRouteSelectMappings.Select<PremiumCRouteSelectMapping, ToPremiumCRouteSelectMapping>(ConvertToPcrsm).Distinct(),
                BlockGroupSelectMappings = null
            };
        }

        private ToAreaData ConvertToTad(PremiumCRoute fza, CsvAreaRecord r)
        {
            Int32 count = Int32.Parse(r.Penetration);
            Int32 total = Int32.Parse(r.Total);

            return new ToAreaData()
            {
                Id = fza.Id,
                Count = count,
                Total = total,
                Penetration = 0 != total ? (Double)count / (Double)total : 0,
                BoxIds = fza.PremiumCRouteBoxMappings.Select<PremiumCRouteBoxMapping, Int32>(t => t.BoxId),
                PremiumCRouteSelectMappings = fza.PremiumCRouteSelectMappings.Select<PremiumCRouteSelectMapping, ToPremiumCRouteSelectMapping>(ConvertToPcrsm).Distinct(),
                BlockGroupSelectMappings = null
            };
        }

        private ToAreaData ConvertToTad(Tract fza, CsvAreaRecord r)
        {
            Int32 count = Int32.Parse(r.Penetration);
            Int32 total = Int32.Parse(r.Total);

            return new ToAreaData()
            {
                Id = fza.Id,
                Count = count,
                Total = total,
                Penetration = 0 != total ? (Double)count / (Double)total : 0,
                BoxIds = fza.TractBoxMappings.Select<TractBoxMapping, Int32>(t => t.BoxId),
                PremiumCRouteSelectMappings = null,
                BlockGroupSelectMappings = fza.BlockGroupSelectMappings.Select<BlockGroupSelectMapping, ToBlockGroupSelectMapping>(ConvertToBgsm).Distinct()
            };
        }

        private ToAreaData ConvertToTad(BlockGroup fza, CsvAreaRecord r)
        {
            Int32 count = Int32.Parse(r.Penetration);
            Int32 total = Int32.Parse(r.Total);

            return new ToAreaData()
            {
                Id = fza.Id,
                Count = count,
                Total = total,
                Penetration = 0 != total ? (Double)count / (Double)total : 0,
                BoxIds = fza.BlockGroupBoxMappings.Select<BlockGroupBoxMapping, Int32>(t => t.BoxId),
                PremiumCRouteSelectMappings = null,
                BlockGroupSelectMappings = fza.BlockGroupSelectMappings.Select<BlockGroupSelectMapping, ToBlockGroupSelectMapping>(ConvertToBgsm).Distinct()
            };
        }

        private static ToPremiumCRouteSelectMapping ConvertToPcrsm(PremiumCRouteSelectMapping pcrsm)
        {
            return new ToPremiumCRouteSelectMapping(pcrsm.ThreeZipAreaId,
                                                    pcrsm.FiveZipAreaId,
                                                    pcrsm.PremiumCRouteId);
        }

        private static ToBlockGroupSelectMapping ConvertToBgsm(BlockGroupSelectMapping pcrsm)
        {
            return new ToBlockGroupSelectMapping(pcrsm.ThreeZipArea.Id, 
                                                 pcrsm.FiveZipArea.Id, 
                                                 pcrsm.Tract.Id, 
                                                 pcrsm.BlockGroup.Id);
        }
        #endregion
    }
}
