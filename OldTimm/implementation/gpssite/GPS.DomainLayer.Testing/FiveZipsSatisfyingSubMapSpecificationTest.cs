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
    public class FiveZipsSatisfyingSubMapSpecificationTest
    {
        [Test]
        public void CanConstructFiveZipsSatisfyingSubMapSpecification()
        {
            // Case 1: non-empty list of ids
            //
            IEnumerable<Int32> selectedFiveZipIds = new List<Int32>() { 1 };
            IEnumerable<Int32> selectedCRouteIds = new List<Int32>() { 2 };
            IEnumerable<Int32> deselectedCRouteIds = new List<Int32>() { 3 };

            FiveZipsSatisfyingSubMapSpecification spec = new FiveZipsSatisfyingSubMapSpecification(
                selectedFiveZipIds, selectedCRouteIds, deselectedCRouteIds);

            Assert.AreEqual(selectedFiveZipIds, spec.SelectedFiveZipIds);
            Assert.AreEqual(selectedCRouteIds, spec.SelectedCRouteIds);
            Assert.AreEqual(deselectedCRouteIds, spec.DeselectedCRouteIds);

            // Case 2: empty list of ids
            //
            FiveZipsSatisfyingSubMapSpecification anotherSpec = new FiveZipsSatisfyingSubMapSpecification(null, null, null);
            Assert.IsTrue(anotherSpec.SelectedFiveZipIds.Count() == 1 && anotherSpec.SelectedFiveZipIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.SelectedCRouteIds.Count() == 1 && anotherSpec.SelectedCRouteIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.DeselectedCRouteIds.Count() == 1 && anotherSpec.DeselectedCRouteIds.ElementAt(0) == Int32.MinValue);
        }

        [Test]
        public void CanGetFiveZipsSatisfyingSubMap()
        {
            IEnumerable<Int32> selectedFiveZipIds = new List<Int32>() { 1 };
            IEnumerable<Int32> selectedCRouteIds = new List<Int32>() { 2 };
            IEnumerable<Int32> deselectedCRouteIds = new List<Int32>() { 3 };

            FiveZipsSatisfyingSubMapSpecification spec = new FiveZipsSatisfyingSubMapSpecification(
                selectedFiveZipIds, selectedCRouteIds, deselectedCRouteIds);

            IEnumerable<FiveZipArea> result = spec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.FiveZipRepository);
            Assert.IsNotNull(result);
        }
    }
}
