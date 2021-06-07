using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class SecondarySchoolAreaBoxMapping : AbstractAreaBoxMapping
    {
        public virtual SecondarySchoolArea SecondarySchoolArea { get; set; }
    }
}
