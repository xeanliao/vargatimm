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
    public class ExportSourceSpecificationTest
    {
        [Test]
        public void Can_Create_FiveZip_Export_Source_Specification()
        {
            IList<Int32> fiveZipIds = new List<Int32>() { 1 };
            IList<Int32> threeZipIds = new List<Int32>() { 1, 2 };
            IList<Int32> nonFiveZipIds = new List<Int32>() { 1, 2, 3 };

            ExportSourceSpecification<FiveZipArea> spec = new ExportSourceSpecification<FiveZipArea>()
            .SetSelectedFiveZipIds(fiveZipIds)
            .SetSelectedThreeZipIds(threeZipIds)
            .SetDeselectedFiveZipIds(nonFiveZipIds);
            Assert.AreEqual(fiveZipIds.Count(), spec.SetSelectedFiveZipIds(fiveZipIds).SelectedFiveZipIds.Count());
            Assert.AreEqual(threeZipIds.Count(), spec.SetSelectedThreeZipIds(threeZipIds).SelectedThreeZipIds.Count());
            Assert.AreEqual(nonFiveZipIds.Count(), spec.SetDeselectedFiveZipIds(nonFiveZipIds).DeselectedFiveZipIds.Count());
        }

        [Test]
        public void Ids_Contain_Int32_Min_Value_If_No_Ids_Provided()
        {
            ExportSourceSpecification<FakeShapeType> spec = new ExportSourceSpecification<FakeShapeType>();
            spec.SetSelectedBlockGroupIds(null)
                .SetSelectedCRouteIds(null)
                .SetSelectedFiveZipIds(null)
                .SetDeselectedBlockGroupIds(null)
                .SetDeselectedCRouteIds(null)
                .SetDeselectedFiveZipIds(null)
                .SetDeselectedTractIds(null)
                .SetSelectedThreeZipIds(null)
                .SetSelectedTractIds(null);

            Assert.IsTrue(spec.SelectedBlockGroupIds.Count() == 1 && spec.SelectedBlockGroupIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(spec.SelectedCRouteIds.Count() == 1 && spec.SelectedCRouteIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(spec.SelectedFiveZipIds.Count() == 1 && spec.SelectedFiveZipIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(spec.DeselectedBlockGroupIds.Count() == 1 && spec.DeselectedBlockGroupIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(spec.DeselectedCRouteIds.Count() == 1 && spec.DeselectedCRouteIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(spec.DeselectedFiveZipIds.Count() == 1 && spec.DeselectedFiveZipIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(spec.DeselectedTractIds.Count() == 1 && spec.DeselectedTractIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(spec.SelectedThreeZipIds.Count() == 1 && spec.SelectedThreeZipIds.ElementAt(0) == Int32.MinValue);
            Assert.IsTrue(spec.SelectedTractIds.Count() == 1 && spec.SelectedTractIds.ElementAt(0) == Int32.MinValue);
        }

        public void Can_Get_Export_Source()
        {
            IList<Int32> fiveZipIds = new List<Int32>() { 1 };
            IList<Int32> threeZipIds = new List<Int32>() { 1, 2 };
            IList<Int32> nonFiveZipIds = new List<Int32>() { 1, 2, 3 };

            IEnumerable<FiveZipArea> areas = new ExportSourceSpecification<FiveZipArea>()
                .SetSelectedFiveZipIds(fiveZipIds)
                .SetSelectedThreeZipIds(threeZipIds)
                .SetDeselectedFiveZipIds(nonFiveZipIds)
                .GetExportSource(WorkSpaceManager.Instance.NewWorkSpace().Repositories.FiveZipRepository);

        }

        class FakeShapeType { }
    }
}
