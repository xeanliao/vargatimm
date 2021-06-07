using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using NUnit.Mocks;
using GPS.Website.TransferObjects;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Area.Addresses;
using GPS.DomainLayer.Area;

namespace GPS.Website.Testing
{
    [TestFixture]
    public class ToMappingsTest
    {
        private const string dir = @"D:\projects\gps\checkout\trunk\implementation\gpssite\GPS.Website.Testing\bin\Debug\";
        private Assembly website;
        private Otis.Configuration cfg;

        [SetUp]
        public void Setup()
        {
            website = Assembly.LoadFile(dir + "GPS.Website.dll");
            cfg = new Otis.Configuration();
            cfg.AddAssemblyResources(website, "otis.xml");
        }

        [TearDown]
        public void Clean()
        {
            cfg = null;
        }

        [Test]
        public void can_assemble_ToCampaign_from_Campaign()
        {
            Otis.IAssembler<ToCampaign, Campaign> asm = cfg.GetAssembler<ToCampaign, Campaign>();

            Campaign c = new Campaign()
            {
                Id = 1,
                Name = "cname",
                Description = "cdescription",
                CampaignRecords = new List<CampaignRecord>(),
                CampaignClassifications = new List<CampaignClassification>(),
                CampaignPercentageColors = new List<CampaignPercentageColor>()
            };

            ToCampaign tc = asm.AssembleFrom(c);

            Assert.AreEqual(c.Id, tc.Id);
            Assert.AreEqual(c.Name, tc.Name);
            Assert.AreEqual(c.Description, tc.Description);
        }

        [Test]
        public void can_assemble_ToAddress_from_MapAddress()
        {
            Otis.IAssembler<ToAddress, MapAddress> asm = cfg.GetAssembler<ToAddress, MapAddress>();

            MapAddress c = new MapAddress()
            {
                Id = 1,
                Street = "cname",
                ZipCode = "cdescription",
                OriginalLatitude = 2,
                OriginalLongitude = 3,
                Color = "color",
                Latitude = 1,
                Longitude = 1,
                Radiuses = new List<MapAddressRadius>() 
                { 
                    new MapAddressRadius()
                    {
                        Id = 1
                    }
                }
            };

            ToAddress tc = asm.AssembleFrom(c);

            Assert.AreEqual(c.Id, tc.Id);
            Assert.AreEqual(c.Street, tc.Street);
            Assert.AreEqual(c.ZipCode, tc.ZipCode);
            Assert.AreEqual(c.OriginalLatitude, tc.OriginalLatitude);
            Assert.AreEqual(c.OriginalLongitude, tc.OriginalLongitude);
            Assert.AreEqual(c.Color, tc.Color);
            Assert.AreEqual(c.Latitude, tc.Latitude);
            Assert.AreEqual(c.Longitude, tc.Longitude);
            Assert.IsNotNull(tc.Radiuses);
            Assert.AreEqual(c.Radiuses.Count, tc.Radiuses.Length);
        }

        [Test]
        public void can_assemble_ToAddressRadius_from_MapAddressRadius()
        {
            Otis.IAssembler<ToAddressRadius, MapAddressRadius> asm = cfg.GetAssembler<ToAddressRadius, MapAddressRadius>();

            MapAddressRadius mar = new MapAddressRadius()
            {
                Id = 1,
                IsDisplay = false,
                Length = 2,
                LengthMeasuresId = 3,
                Relations = FakeRelations()
            };

            ToAddressRadius tar = asm.AssembleFrom(mar);

            Assert.AreEqual(mar.Id, tar.Id);
            Assert.AreEqual(mar.IsDisplay, tar.IsDisplay);
            Assert.AreEqual(mar.LengthMeasuresId, tar.LengthMeasuresId);
            Assert.AreEqual(mar.Length, tar.Length);
            Assert.IsNotNull(tar.Relations);
            Assert.AreEqual(mar.Relations.Count, tar.Relations.Count);
        }

        [Test]
        public void CanAssembleAreaRecordFromToAreaRecord()
        {
            Otis.IAssembler<AreaRecord, ToAreaRecord> asm = cfg.GetAssembler<AreaRecord, ToAreaRecord>();

            ToAreaRecord tar = new ToAreaRecord()
            {
                AreaId = 1,
                Classification = 1,
                Relations = null,
                Value = true
            };

            AreaRecord ar = asm.AssembleFrom(tar);

            Assert.AreEqual(ar.AreaId, tar.AreaId);
        }

        private Dictionary<int, Dictionary<int, string>> FakeRelations()
        {
            Dictionary<int, Dictionary<int, string>> dic = new Dictionary<int, Dictionary<int, string>>();

            for (int i = 0; i < 2; i++)
            {
                dic.Add(i, FakeIntStringDictionary());
            }

            return dic;
        }

        private Dictionary<int, string> FakeIntStringDictionary()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();

            for (int i = 0; i < 3; i++)
            {
                dic.Add(i, "item " + i);
            }

            return dic;
        }
    }
}
