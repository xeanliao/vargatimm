using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.QuerySpecifications
{
    /// <summary>
    /// Query specification for Export Sources such as <see cref="FiveZipArea"/>, etc.
    /// </summary>
    /// <typeparam name="ShapeType"></typeparam>
    public class ExportSourceSpecification<ShapeType> : IExportSourceSpecification
    {
        #region Implementations
        private IList<Int32> _threeZipIds;
        private IList<Int32> _fiveZipIds;
        private IList<Int32> _tractIds;
        private IList<Int32> _blockGroupIds;
        private IList<Int32> _cRouteIds;
        private IList<Int32> _nonFiveZipIds;
        private IList<Int32> _nonTractIds;
        private IList<Int32> _nonBlockGroupIds;
        private IList<Int32> _nonCRouteIds;

        /// <summary>
        /// If no ids are provided, return an array with only one non-existing identifier.
        /// 
        /// THIS ASSUMES THE IDENTIFIERS IN THE DATABASE ARE ALL NON-ZERO, OTHERWISE, THE
        /// LOGIC HERE MUST BE MODIFIED TO FIT THE ACTUAL NEED.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private IList<Int32> CorrectIds(IEnumerable<Int32> ids)
        {
            if (null == ids || ids.Count() == 0) return new List<Int32>() { Int32.MinValue };
            return new List<Int32>(ids);
        }

        #endregion

        #region Constructors
        public ExportSourceSpecification()
        {
            _threeZipIds = CorrectIds(null);
            _fiveZipIds = CorrectIds(null);
            _tractIds = CorrectIds(null);
            _blockGroupIds = CorrectIds(null);
            _cRouteIds = CorrectIds(null);
            _nonFiveZipIds = CorrectIds(null);
            _nonTractIds = CorrectIds(null);
            _nonBlockGroupIds = CorrectIds(null);
            _nonCRouteIds = CorrectIds(null);
        }
        #endregion

        #region Interfaces
        public IEnumerable<Int32> SelectedThreeZipIds 
        {
            get { return _threeZipIds; }
        }
        
        public IEnumerable<Int32> SelectedFiveZipIds
        {
            get { return _fiveZipIds; }
        }
        
        public IEnumerable<Int32> SelectedTractIds
        {
            get { return _tractIds; }
        }
        
        public IEnumerable<Int32> SelectedBlockGroupIds
        {
            get { return _blockGroupIds; }
        }
        
        public IEnumerable<Int32> SelectedCRouteIds
        {
            get { return _cRouteIds; }
        }

        public IEnumerable<Int32> DeselectedFiveZipIds
        {
            get { return _nonFiveZipIds; }
        }

        public IEnumerable<Int32> DeselectedTractIds
        {
            get { return _nonTractIds;}
        }

        public IEnumerable<Int32> DeselectedBlockGroupIds
        {
            get{return _nonBlockGroupIds;}
        }

        public IEnumerable<Int32> DeselectedCRouteIds
        {
            get { return _nonCRouteIds; }
        }

        public ExportSourceSpecification<ShapeType> SetSelectedFiveZipIds(IEnumerable<Int32> ids) 
        {
            _fiveZipIds = CorrectIds(ids);
            return this; 
        }

        public ExportSourceSpecification<ShapeType> SetSelectedThreeZipIds(IEnumerable<Int32> ids) 
        {
            _threeZipIds = CorrectIds(ids);
            return this;
        }

        public ExportSourceSpecification<ShapeType> SetSelectedTractIds(IEnumerable<Int32> ids)
        {
            _tractIds = CorrectIds(ids);
            return this;
        }

        public ExportSourceSpecification<ShapeType> SetSelectedBlockGroupIds(IEnumerable<Int32> ids)
        {
            _blockGroupIds = CorrectIds(ids);
            return this;
        }

        public ExportSourceSpecification<ShapeType> SetSelectedCRouteIds(IEnumerable<Int32> ids)
        {
            _cRouteIds = CorrectIds(ids);
            return this;
        }

        public ExportSourceSpecification<ShapeType> SetDeselectedFiveZipIds(IEnumerable<Int32> ids)
        {
            _nonFiveZipIds = CorrectIds(ids);
            return this;
        }

        public ExportSourceSpecification<ShapeType> SetDeselectedTractIds(IEnumerable<Int32> ids)
        {
            _nonTractIds = CorrectIds(ids);
            return this;
        }

        public ExportSourceSpecification<ShapeType> SetDeselectedBlockGroupIds(IEnumerable<Int32> ids)
        {
            _nonBlockGroupIds = CorrectIds(ids);
            return this;
        }

        public ExportSourceSpecification<ShapeType> SetDeselectedCRouteIds(IEnumerable<Int32> ids)
        {
            _nonCRouteIds = CorrectIds(ids);
            return this;
        }
        
        public IEnumerable<ShapeType> GetExportSource(IExportSourceRepository<ShapeType> rep)
        {
            return rep.GetExportSource(this);
        }
        #endregion
    }
}
