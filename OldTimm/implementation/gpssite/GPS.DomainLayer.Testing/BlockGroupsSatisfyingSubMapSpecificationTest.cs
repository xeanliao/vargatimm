using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.QuerySpecifications;

namespace GPS.DataLayer.Test
{
    [TestFixture]
    public class BlockGroupsSatisfyingSubMapSpecificationTest
    {
        [Test]
        public void CanConstructBlockGroupsSatisfyingSubMapSpecification()
        {
            // Case 1: non-empty list of ids
            //
            IEnumerable<Int32> selectedTractIds = new List<Int32>() { 1 };
            IEnumerable<Int32> deselectedTractIds = new List<Int32>() { 2, 3 };
            IEnumerable<Int32> selectedBGIds = new List<Int32>() { 2, 4, 5 };
            IEnumerable<Int32> deselectedBGIds = new List<Int32>() { 3, 4, 5, 6 };

            BlockGroupsSatisfyingSubMapSpecification spec = new BlockGroupsSatisfyingSubMapSpecification(
                selectedTractIds, deselectedTractIds, selectedBGIds, deselectedBGIds);

            Assert.AreEqual(selectedTractIds, spec.SelectedTractIds);
            Assert.AreEqual(deselectedTractIds, spec.DeselectedTractIds);
            Assert.AreEqual(selectedBGIds, spec.SelectedBGIds);
            Assert.AreEqual(deselectedBGIds, spec.DeselectedBGIds);

            // Case 2: empty list of ids
            //
            BlockGroupsSatisfyingSubMapSpecification anotherSpec = new BlockGroupsSatisfyingSubMapSpecification(null, null, null, null);
            Assert.IsTrue(anotherSpec.SelectedTractIds.Count() == 1 && anotherSpec.SelectedTractIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.DeselectedTractIds.Count() == 1 && anotherSpec.DeselectedTractIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.SelectedBGIds.Count() == 1 && anotherSpec.SelectedBGIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.DeselectedBGIds.Count() == 1 && anotherSpec.DeselectedBGIds.ElementAt(0) == Int32.MinValue);
        }

        [Test]
        public void CanGetBlockGroupsSatisfyingSubMap()
        {
            IEnumerable<Int32> selectedTractIds = new List<Int32>() { 1 };
            IEnumerable<Int32> deselectedTractIds = new List<Int32>() { 2, 3 };
            IEnumerable<Int32> selectedBGIds = new List<Int32>() { 2, 4, 5 };
            IEnumerable<Int32> deselectedBGIds = new List<Int32>() { 3, 4, 5, 6 };

            BlockGroupsSatisfyingSubMapSpecification spec = new BlockGroupsSatisfyingSubMapSpecification(
                selectedTractIds, deselectedTractIds, selectedBGIds, deselectedBGIds);

            IEnumerable<BlockGroup> result = spec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.BlockGroupRepository);
            Assert.IsNotNull(result);
        }
    }
}
