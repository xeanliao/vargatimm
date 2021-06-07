using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Cfg;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.QuerySpecifications;
using GPS.DomainLayer.Interfaces;

namespace GPS.DataLayer.Test
{
    [TestFixture]
    public class ImportedDataSpecificationTest
    {
        #region fields
        private ISession _session;
        #endregion

        #region setup code
        [SetUp]        
        public void Setup()
        {
            ISessionFactory sessionFactory = (new Configuration()).Configure().BuildSessionFactory();
            _session = sessionFactory.OpenSession();
        }

        [TearDown]
        public void Clean()
        {
            _session.Close();
        }

        #endregion

        [Test]
        public void CanFetchToAreaDataForFiveZips()
        {
            CsvAreaRecord r = new CsvAreaRecord() { Code = "90001", Total = "100", Penetration = "30" };
            ImportedDataSpecification spec = new ImportedDataSpecification();

            IEnumerable<ToAreaData> items = spec.GetImportedData(new List<CsvAreaRecord>() { r }, new FiveZipRepository());
            
            Assert.IsTrue(items.Count() > 0);            
            foreach (ToAreaData i in items)
            {
                Assert.IsTrue(i.PremiumCRouteSelectMappings.Count() > 0);
                foreach (ToPremiumCRouteSelectMapping m in i.PremiumCRouteSelectMappings)
                {
                    Assert.IsTrue(m.ThreeZipId != 0);
                    Assert.IsTrue(m.FiveZipId != 0);
                    Assert.IsTrue(m.PremiumCRouteId != 0);
                }
                Assert.AreEqual(i.Total.ToString(), r.Total);
                Assert.AreEqual(i.Count.ToString(), r.Penetration);
                Assert.IsTrue(i.BoxIds.Count() > 0);
                Assert.AreEqual(0.3, i.Penetration);
            }
        }

        [Test]
        public void CanFetchToAreaDataForTracts()
        {
            CsvAreaRecord r = new CsvAreaRecord() { Code = "01077011300", Total = "100", Penetration = "30" };
            ImportedDataSpecification spec = new ImportedDataSpecification();

            IEnumerable<ToAreaData> items = spec.GetImportedData(new List<CsvAreaRecord>() { r }, new TractRepository());

            Assert.IsTrue(items.Count() > 0);
            foreach (ToAreaData i in items)
            {
                Assert.IsTrue(i.BlockGroupSelectMappings.Count() > 0);
                foreach (ToBlockGroupSelectMapping m in i.BlockGroupSelectMappings)
                {
                    Assert.IsTrue(m.ThreeZipId != 0);
                    Assert.IsTrue(m.FiveZipId != 0);
                    Assert.IsTrue(m.TractId != 0);
                    Assert.IsTrue(m.BlockGroupId != 0);
                }
                Assert.AreEqual(i.Total.ToString(), r.Total);
                Assert.AreEqual(i.Count.ToString(), r.Penetration);
                Assert.IsTrue(i.BoxIds.Count() > 0);
                Assert.AreEqual(0.3, i.Penetration);
            }
        }

        [Test]
        public void CanFetchToAreaDataForBlockGroups()
        {
            CsvAreaRecord r = new CsvAreaRecord() { Code = "010770113001", Total = "100", Penetration = "30" };
            ImportedDataSpecification spec = new ImportedDataSpecification();

            IEnumerable<ToAreaData> items = spec.GetImportedData(new List<CsvAreaRecord>() { r }, new BlockGroupRepository());

            Assert.IsTrue(items.Count() > 0);
            foreach (ToAreaData i in items)
            {
                Assert.IsTrue(i.BlockGroupSelectMappings.Count() > 0);
                Assert.AreEqual(i.Total.ToString(), r.Total);
                Assert.AreEqual(i.Count.ToString(), r.Penetration);
                Assert.IsTrue(i.BoxIds.Count() > 0);
                Assert.AreEqual(0.3, i.Penetration);
            }
        }

        [Test]
        public void CanFetchToAreaDataForPremiumCRoutes()
        {
            CsvAreaRecord r = new CsvAreaRecord() { Code = "00601C001", Total = "100", Penetration = "30" };
            ImportedDataSpecification spec = new ImportedDataSpecification();

            IEnumerable<ToAreaData> items = spec.GetImportedData(new List<CsvAreaRecord>() { r }, new PremiumCRouteRepository());

            Assert.IsTrue(items.Count() > 0);
            foreach (ToAreaData i in items)
            {
                Assert.IsTrue(i.PremiumCRouteSelectMappings.Count() > 0);
                Assert.AreEqual(i.Total.ToString(), r.Total);
                Assert.AreEqual(i.Count.ToString(), r.Penetration);
                Assert.IsTrue(i.BoxIds.Count() > 0);
                Assert.AreEqual(0.3, i.Penetration);
            }
        }
    }
}