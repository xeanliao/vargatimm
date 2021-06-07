using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS.DomainLayer.Entities
{
    public class DistributionJobMap
    {
        #region basic properties
        private int _id;
        #endregion

        #region parents
        private DistributionJob _distributionJob;
        private DistributionMap _distributionMap;
        #endregion

        public virtual int Id
        {
            get { return this._id; }
        }

        public virtual DistributionJob DistributionJob
        {
            get { return _distributionJob; }
        }

        public virtual DistributionMap DistributionMap
        {
            get { return _distributionMap; }
        }

        #region constructors
        protected DistributionJobMap() { }
        
        public DistributionJobMap(int id, DistributionMap dm, DistributionJob dj) 
        {
            this._id = id;
            this._distributionMap = dm;
            this._distributionJob = dj;
        }
        #endregion
    }
}
