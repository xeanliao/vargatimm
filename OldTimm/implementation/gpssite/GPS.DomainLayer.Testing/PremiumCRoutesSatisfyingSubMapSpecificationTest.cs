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
    public class PremiumCRoutesSatisfyingSubMapSpecificationTest
    {
        [Test]
        public void CanConstructPremiumCRoutesSatisfyingSubMapSpecification()
        {
            // Case 1: non-empty list of ids
            //
            IEnumerable<Int32> selectedFiveZipIds = new List<Int32>() { 1 };
            IEnumerable<Int32> deselectedFiveZipIds = new List<Int32>() { 2, 3 };
            IEnumerable<Int32> selectedCRouteIds = new List<Int32>() { 2, 4, 5 };
            IEnumerable<Int32> deselectedCRouteIds = new List<Int32>() { 3, 4, 5, 6 };

            PremiumCRoutesSatisfyingSubMapSpecification spec = new PremiumCRoutesSatisfyingSubMapSpecification(
                selectedFiveZipIds, deselectedFiveZipIds, selectedCRouteIds, deselectedCRouteIds);

            Assert.AreEqual(selectedFiveZipIds, spec.SelectedFiveZipIds);
            Assert.AreEqual(deselectedFiveZipIds, spec.DeselectedFiveZipIds);
            Assert.AreEqual(selectedCRouteIds, spec.SelectedCRouteIds);
            Assert.AreEqual(deselectedCRouteIds, spec.DeselectedCRouteIds);

            // Case 2: empty list of ids
            //
            PremiumCRoutesSatisfyingSubMapSpecification anotherSpec = new PremiumCRoutesSatisfyingSubMapSpecification(null, null, null, null);
            Assert.IsTrue(anotherSpec.SelectedFiveZipIds.Count() == 1 && anotherSpec.SelectedFiveZipIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.DeselectedFiveZipIds.Count() == 1 && anotherSpec.DeselectedFiveZipIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.SelectedCRouteIds.Count() == 1 && anotherSpec.SelectedCRouteIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(anotherSpec.DeselectedCRouteIds.Count() == 1 && anotherSpec.DeselectedCRouteIds.ElementAt(0) == Int32.MinValue);
        }

        [Test]
        public void CanGetPremiumCRoutesSatisfyingSubMap()
        {
            IEnumerable<Int32> selectedFiveZipIds = new List<Int32>() { 1 };
            IEnumerable<Int32> deselectedFiveZipIds = new List<Int32>() { 2, 3 };
            IEnumerable<Int32> selectedCRouteIds = new List<Int32>() { 2, 4, 5 };
            IEnumerable<Int32> deselectedCRouteIds = new List<Int32>() { 3, 4, 5, 6 };

            PremiumCRoutesSatisfyingSubMapSpecification spec = new PremiumCRoutesSatisfyingSubMapSpecification(
                selectedFiveZipIds, deselectedFiveZipIds, selectedCRouteIds, deselectedCRouteIds);

            IEnumerable<PremiumCRoute> result = spec.GetSatisfyingAreas(WorkSpaceManager.Instance.NewWorkSpace().Repositories.PremiumCRouteRepository);
            Assert.IsNotNull(result);
        }
    }
}
