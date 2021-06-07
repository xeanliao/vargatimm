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
    public class BlockGroupsSatisfyingSubMapSpecification
    {
        #region Fields
        private readonly IEnumerable<Int32> _selectedTractIds;
        private readonly IEnumerable<Int32> _deselectedTractIds;
        private readonly IEnumerable<Int32> _selectedBGIds;
        private readonly IEnumerable<Int32> _deselectedBGIds;
        #endregion

        #region Public properties
        public IEnumerable<Int32> SelectedTractIds { get { return _selectedTractIds; } }
        public IEnumerable<Int32> DeselectedTractIds { get { return _deselectedTractIds; } }
        public IEnumerable<Int32> SelectedBGIds { get { return _selectedBGIds; } }
        public IEnumerable<Int32> DeselectedBGIds { get { return _deselectedBGIds; } }
        #endregion

        #region Constructors
        public BlockGroupsSatisfyingSubMapSpecification(
            IEnumerable<Int32> selectedTractIds, IEnumerable<Int32> deselectedTractIds, 
            IEnumerable<Int32> selectedBGIds, IEnumerable<Int32> deselectedBGIds)
        {
            _selectedTractIds = CorrectIds(selectedTractIds);
            _deselectedTractIds = CorrectIds(deselectedTractIds);
            _selectedBGIds = CorrectIds(selectedBGIds);
            _deselectedBGIds = CorrectIds(deselectedBGIds);
        }
        #endregion

        #region Query methods
        public IEnumerable<BlockGroup> GetSatisfyingAreas(IBlockGroupRepository repository)
        {
            return repository.GetBlockGroupsSatisfyingSubMap(this);
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
