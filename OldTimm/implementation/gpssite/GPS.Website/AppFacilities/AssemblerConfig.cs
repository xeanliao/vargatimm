using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace GPS.Website.AppFacilities
{
    public class AssemblerConfig
    {
        private static Otis.Configuration _config;

        private static Otis.Configuration Config
        {
            get 
            {
                if (null == _config)
                {
                    _config = new Otis.Configuration();
                    _config.AddAssemblyResources(Assembly.GetExecutingAssembly(), "otis.xml");
                }
                return _config;
            }
        }

        public static Assembler<Target, Source> GetAssembler<Target, Source>()
        {
            return new Assembler<Target, Source>(Config.GetAssembler<Target, Source>());
        }
    }
}
