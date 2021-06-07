using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class ElementarySchoolAreaCoordinate : AbstractAreaCoordinate
    {
        #region parent
        public virtual ElementarySchoolArea ElementarySchoolArea { get; set; }
        #endregion
    }
}
