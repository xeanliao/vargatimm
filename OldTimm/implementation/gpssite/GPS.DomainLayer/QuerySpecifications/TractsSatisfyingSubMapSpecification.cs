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
    public class TractsSatisfyingSubMapSpecification
    {
        #region Fields
        private readonly IEnumerable<Int32> _selectedTractIds;
        private readonly IEnumerable<Int32> _selectedBlockGroupIds;
        private readonly IEnumerable<Int32> _deselectedBlockGroupIds;
        #endregion

        #region Public properties
        public IEnumerable<Int32> SelectedTractIds { get { return _selectedTractIds; } }
        public IEnumerable<Int32> SelectedBlockGroupIds { get { return _selectedBlockGroupIds; } }
        public IEnumerable<Int32> DeselectedBlockGroupIds { get { return _deselectedBlockGroupIds; } }
        #endregion

        #region Constructors
        public TractsSatisfyingSubMapSpecification(
            IEnumerable<Int32> selectedTractIds, IEnumerable<Int32> selectedBlockGroupIds, 
            IEnumerable<Int32> deselectedBlockGroupIds)
        {
            _selectedTractIds = CorrectIds(selectedTractIds);
            _selectedBlockGroupIds = CorrectIds(selectedBlockGroupIds);
            _deselectedBlockGroupIds = CorrectIds(deselectedBlockGroupIds);
        }
        #endregion

        #region Query methods
        public IEnumerable<Tract> GetSatisfyingAreas(ITractRepository repository)
        {
            return repository.GetTractsSatisfyingSubMap(this);
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
