using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.FixInner
{
    class FixerInnerFactory
    {
        public static FixerInnerBase CreateFixer(FixInnerTables table)
        {
            FixerInnerBase fixer = null;
            switch (table)
            {
                case FixInnerTables.FiveZipAreas:
                    fixer = new FiveZipInnerFixer();
                    break;
                case FixInnerTables.PremiumCRoutes:
                    fixer = new PremiumCRouteInnerFixer();
                    break;
            }
            return fixer;
        }
    }
}