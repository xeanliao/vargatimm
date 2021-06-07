using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTU.ModelLayer.Common
{
    public interface IProcess
    {
        //IProcess Node { get; set; }
        Boolean Process(Object param);
    }
}
