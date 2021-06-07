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
    public class TractsSatisfyingSubMapSpecificationTest
    {
        [Test]
        public void CanConstructTractsSatisfyingSubMapSpecification()
        {
            // Case 1: non-empty list of ids
            //
            IEnumerable<Int32> selectedTractIds = new List<Int32>() { 1 };
            IEnumerable<Int32> selectedBlockGroupIds = new List<Int32>() { 1, 2, 3 };
            IEnumerable<Int32> deselectedBlockGroupIds = new List<Int32>() { 1, 2, 3, 4 };

            TractsSatisfyingSubMapSpecification spec = new TractsSatisfyingSubMapSpecification(
                selectedTractIds, selectedBlockGroupIds, deselectedBlockGroupIds);

            Assert.AreEqual(selectedTractIds, spec.SelectedTractIds);
            Assert.AreEqual(selectedBlockGroupIds, spec.SelectedBlockGroupIds);
            Assert.AreEqual(deselectedBlockGroupIds, spec.DeselectedBlockGroupIds);

            // Case 2: empty list of ids
            //
            TractsSatisfyingSubMapSpecification anotherSpec = new TractsSatisfyingSubMapSpecification(null, null, null);
            Assert.IsTrue(anotherSpec.SelectedTractIds.Count() == 1 && anotherSpec.SelectedTractIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.SelectedBlockGroupIds.Count() == 1 && anotherSpec.SelectedBlockGroupIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.DeselectedBlockGroupIds.Count() == 1 && anotherSpec.DeselectedBlockGroupIds.ElementAt(0) == Int32.MinValue);
        }

        [Test]
        public void CanGetTractsSatisfyingSubMap()
        {
            IEnumerable<Int32> selectedTractIds = new List<Int32>() { 1 };
            IEnumerable<Int32> selectedBlockGroupIds = new List<Int32>() { 1, 2, 3 };
            IEnumerable<Int32> deselectedBlockGroupIds = new List<Int32>() { 1, 2, 3, 4 };

            TractsSatisfyingSubMapSpecification spec = new TractsSatisfyingSubMapSpecification(
                selectedTractIds, selectedBlockGroupIds, deselectedBlockGroupIds);

            IEnumerable<Tract> result = spec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.TractRepository);
            Assert.IsNotNull(result);
        }
    }
}
