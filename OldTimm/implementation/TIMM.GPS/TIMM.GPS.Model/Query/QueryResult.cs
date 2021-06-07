using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TIMM.GPS.Model
{
    public class QueryResult<T>
    {
        public QueryResult()
        {
            Result = new List<T>();
        }

        public int TotalRecord { get; set; }
        public List<T> Result { get; set; }
    }
}
