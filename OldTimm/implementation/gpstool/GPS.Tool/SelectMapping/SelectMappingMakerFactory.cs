using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.Tool.SelectMapping
{
    class SelectMappingMakerFactory
    {
        public static MakerBase CreateMaker(SelectMappingTables table)
        {
            MakerBase maker = null;
            switch (table) { 
                case SelectMappingTables.PremiumCRouteSelectMappings:
                    maker = new PremiumCRouteSelectMappingMaker();
                    break;
                case SelectMappingTables.BlockGroupSelectMappings:
                    maker = new BlockGroupSelectMappingMaker();
                    break;
            }
            return maker;
        }
    }
}
