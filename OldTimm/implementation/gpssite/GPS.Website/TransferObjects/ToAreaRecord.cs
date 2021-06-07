using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GPS.DomainLayer.Area;
using GPS.DomainLayer.Enum;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Area.AreaOperators;

namespace GPS.Website.TransferObjects
{
    [DataContract(Namespace = "TIMM.Website.TransferObjects")]
    public class ToAreaRecord
    {
        [DataMember]
        public Int32 Classification { get; set; }
        [DataMember]
        public Int32 AreaId { get; set; }
        [DataMember]
        public Boolean Value { get; set; }
        [DataMember]
        public Dictionary<int, Dictionary<int, bool>> Relations { get; set; }


        /// <summary>
        /// Only used by the Otis transformer to transform the Relations property.
        /// </summary>
        /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
        /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
        public static void Convert(ref ToAreaRecord target, ref AreaRecord source)
        {
            target.Classification = (int)source.Classification;
            target.Relations = source.Relations;
        }

        /// <summary>
        /// Only used by the Otis transformer to transform the Relations property.
        /// </summary>
        /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
        /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
        public static void Convert(ref ToAreaRecord target, CampaignRecord source)
        {
            target.Classification = source.Classification;
            Classifications classification = (Classifications)source.Classification;
            switch (classification)
            {
                case Classifications.Z5:
                    FiveZipAreaOperator fOper = new FiveZipAreaOperator();
                    target.Relations = fOper.GetRelations(fOper.GetItem(source.AreaId).PremiumCRouteSelectMappings);
                    break;
                case Classifications.TRK:
                    TractOperator tOper = new TractOperator();
                    target.Relations = tOper.GetRelations(tOper.GetItem(source.AreaId).BlockGroupSelectMappings);
                    break;
                case Classifications.BG:
                    BlockGroupOperator bOper = new BlockGroupOperator();
                    target.Relations = bOper.GetRelations(bOper.GetItem(source.AreaId).BlockGroupSelectMappings);
                    break;
                case Classifications.PremiumCRoute:
                    PremiumCRouteOperator cOper = new PremiumCRouteOperator();
                    target.Relations = cOper.GetRelations(cOper.GetItem(source.AreaId).PremiumCRouteSelectMappings);
                    break;
            }

            //target.Relations = source.Relations;
        }

        /// <summary>
        /// Only used by the Otis transformer to transform the Relations property.
        /// </summary>
        /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
        /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
        public static void Convert(ref ToAreaRecord target, ref SubMapRecord source)
        {
            target.Classification = (int)source.Classification;
            switch (source.Classification)
            {
                case Classifications.Z5:
                    FiveZipAreaOperator fOper = new FiveZipAreaOperator();
                    target.Relations = fOper.GetRelations(fOper.GetItem(source.AreaId).PremiumCRouteSelectMappings);
                    break;
                case Classifications.TRK:
                    TractOperator tOper = new TractOperator();
                    target.Relations = tOper.GetRelations(tOper.GetItem(source.AreaId).BlockGroupSelectMappings);
                    break;
                case Classifications.BG:
                    BlockGroupOperator bOper = new BlockGroupOperator();
                    target.Relations = bOper.GetRelations(bOper.GetItem(source.AreaId).BlockGroupSelectMappings);
                    break;
                case Classifications.PremiumCRoute:
                    PremiumCRouteOperator cOper = new PremiumCRouteOperator();
                    target.Relations = cOper.GetRelations(cOper.GetItem(source.AreaId).PremiumCRouteSelectMappings);
                    break;
            }

            //target.Relations = source.Relations;
        }

        /// <summary>
        /// Only used by the Otis transformer to transform the Relations property.
        /// </summary>
        /// <param name="target">A <see cref="ToAddressRadius"/>, the DTO.</param>
        /// <param name="source">A <see cref="MapAddressRadius"/>, the source.</param>
        public static void Convert(ref ToAreaRecord target, ref DistributionMapRecords source)
        {
            target.Classification = (int)source.Classification;
            //switch (source.Classification)
            //{
            //    case Classifications.Z5:
            //        FiveZipAreaOperator fOper = new FiveZipAreaOperator();
            //        target.Relations = fOper.GetRelations(fOper.GetItem(source.AreaId).PremiumCRouteSelectMappings);
            //        break;
            //    case Classifications.TRK:
            //        TractOperator tOper = new TractOperator();
            //        target.Relations = tOper.GetRelations(tOper.GetItem(source.AreaId).BlockGroupSelectMappings);
            //        break;
            //    case Classifications.BG:
            //        BlockGroupOperator bOper = new BlockGroupOperator();
            //        target.Relations = bOper.GetRelations(bOper.GetItem(source.AreaId).BlockGroupSelectMappings);
            //        break;
            //    case Classifications.PremiumCRoute:
            //        PremiumCRouteOperator cOper = new PremiumCRouteOperator();
            //        target.Relations = cOper.GetRelations(cOper.GetItem(source.AreaId).PremiumCRouteSelectMappings);
            //        break;
            //}

            //target.Relations = source.Relations;
        }

        /// <summary>
        /// Only used by the Otis transformer to transform the role property.
        /// </summary>
        /// <param name="target">A <see cref="ToGtu"/>, the DTO.</param>
        /// <param name="source">A <see cref="Gtu"/>, the source.</param>
        public static void ConvertBack(ref AreaRecord target, ref ToAreaRecord source)
        {
            target.Classification = (Classifications)source.Classification;
        }
    }
}
