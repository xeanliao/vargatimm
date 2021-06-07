using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIMM.GPS.RESTService
{
    public class BusinessException : Exception
    {
        public string Code { get; private set; }

        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string code, string message)
            : base(message)
        {
            this.Code = code;
        }
    }
}
