using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.REST.Helper
{
    public static class AreaHelper
    {
        public static int? SumTotal(string description, List<AbstractArea> areas)
        {
            switch (description)
            {
                case "APT + HOME":
                    return areas.Sum(i => (i.APT_COUNT ?? 0) + (i.HOME_COUNT ?? 0));
                case "APT ONLY":
                    return areas.Sum(i => (i.APT_COUNT ?? 0));
                case "HOME ONLY":
                    return areas.Sum(i => (i.HOME_COUNT ?? 0));
                default:
                    return areas.Sum(i => (i.APT_COUNT ?? 0) + (i.HOME_COUNT ?? 0));
            }
        }

        public static string CalcPercent(int? total, int? totalAdjust, int? count, int? countAdjust)
        {
            var fixTotal = (total ?? 0) + (totalAdjust ?? 0);
            var fixCount = (count ?? 0) + (countAdjust ?? 0);
            var percent = fixTotal == 0 ? 0D : Math.Round((double)fixCount / (double)fixTotal, 4) * 100D;
            return $"{percent}%";
        }
    }
}