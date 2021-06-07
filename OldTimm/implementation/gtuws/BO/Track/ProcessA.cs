using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTU.ModelLayer.Common;

namespace GTU.BusinessLayer.Track
{
    public class ProcessA : IProcess
    {
        public ProcessA(IProcess node)
        {
            this.Node = node;
        }

        #region IProcess Members

        public bool Process(object param)
        {
            // todo

            return this.Node.Process(param);

        }

        #endregion

        #region IProcess Members

        public IProcess Node
        {
            get;
            set;
        }

        #endregion
    }
}
