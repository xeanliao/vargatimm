using System; 
using System.Collections.Generic; 
using System.Text; 

namespace GPS.DomainLayer.Entities 
{
    public class ElementarySchoolAreaBoxMapping : AbstractAreaBoxMapping
    {
        #region parent

        public virtual ElementarySchoolArea ElementarySchoolArea
        {
            get;
            set;
        }

        #endregion
    }
}
