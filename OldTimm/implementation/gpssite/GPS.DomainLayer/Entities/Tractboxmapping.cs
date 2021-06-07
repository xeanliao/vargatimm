using System; 

namespace GPS.DomainLayer.Entities 
{
    public class TractBoxMapping : AbstractAreaBoxMapping
    {
        public virtual int TractId { get; set; }
    }
}
