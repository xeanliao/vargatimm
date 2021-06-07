using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.FixData
{
    class FixerFactory
    {
        public static FixerBase CreateFixer(FixTables table)
        {
            FixerBase fixer = null;
            switch (table)
            {
                case FixTables.FiveZipAreas:
                    fixer = new FiveZipFixer();
                    break;
                case FixTables.PremiumCRoutes:
                    fixer = new PremiumCRouteFixer();
                    break;
            }
            return fixer;
        }
    }
}
