using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMM.Jobs.NonDeliverableAddresses
{
    public class ResultAddressEventArgs : EventArgs
    {
        public ResultAddress ResultInfo { get; set; }

        public ResultAddressEventArgs(ResultAddress result)
        {
            ResultInfo = result;
        }
    }
}
