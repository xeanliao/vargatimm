using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPS.Website.AppFacilities
{
    public class Assembler<Target, Source>
    {
        #region Constructor
        public Assembler(Otis.IAssembler<Target, Source> assembler)
        {
            this._assembler = assembler;
        }
        #endregion

        #region Interface
        public Target AssembleFrom(Source source)
        {
            return this._assembler.AssembleFrom(source);
        }

        public IEnumerable<Target> AssembleFrom(IEnumerable<Source> source)
        {
            return new List<Source>(source).ConvertAll<Target>(
                delegate(Source s) { return this._assembler.AssembleFrom(s); });
        }
        #endregion

        #region Implementation
        private Otis.IAssembler<Target, Source> _assembler;
        #endregion
    }
}
