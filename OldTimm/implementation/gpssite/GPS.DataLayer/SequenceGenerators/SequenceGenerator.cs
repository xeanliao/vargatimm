using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Id;

namespace GPS.DataLayer.SequenceGenerators
{
    public class SequenceGenerator : TableGenerator
    {
        private const Int32 SeedValue = 0;
        //private static readonly ILog Log = LogManager.GetLogger(typeof(FDPSequence));

        public override object Generate(ISessionImplementor session, object obj)
        {
            int counter = Convert.ToInt32(base.Generate(session, obj));
            return counter + SeedValue + 1;
        }
    }
}
