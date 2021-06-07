using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.Interfaces;
using GPS.DataLayer;

namespace GPS.DomainLayer.QuerySpecifications
{
    public class FiveZipsSatisfyingSubMapSpecification
    {
        #region Fields
        private readonly IEnumerable<Int32> _selectedFiveZipIds;
        private readonly IEnumerable<Int32> _selectedCRouteIds;
        private readonly IEnumerable<Int32> _deselectedCRouteIds;
        #endregion

        #region Public properties
        public IEnumerable<Int32> SelectedFiveZipIds { get { return _selectedFiveZipIds; } }
        public IEnumerable<Int32> SelectedCRouteIds { get { return _selectedCRouteIds; } }
        public IEnumerable<Int32> DeselectedCRouteIds { get { return _deselectedCRouteIds; } }
        #endregion

        #region Constructors
        public FiveZipsSatisfyingSubMapSpecification(
            IEnumerable<Int32> selectedFiveZipIds, IEnumerable<Int32> selectedCRouteIds, 
            IEnumerable<Int32> deselectedCRouteIds)
        {
            _selectedFiveZipIds = CorrectIds(selectedFiveZipIds);
            _selectedCRouteIds = CorrectIds(selectedCRouteIds);
            _deselectedCRouteIds = CorrectIds(deselectedCRouteIds);
        }
        #endregion

        #region Query methods
        public IEnumerable<FiveZipArea> GetSatisfyingAreas(IFiveZipRepository repository)
        {
            return repository.GetFiveZipsSatisfyingSubMap(this);
        }
        #endregion

        #region Implementations
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
    }
}
