using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GPS.DataLayer;
using GPS.DataLayer.DataInfrastructure;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;

namespace GPS.DataLayer.Test
{
    [TestFixture]
    public class DataImportTest
    {
        const int FiveZipAreaId = Int32.MinValue + 1;
        const int BlockGroupId = Int32.MinValue + 2;

        //[SetUp]
        //public void SetUp()
        //{
        //    CreateFiveZipAreaForTesting();
        //    CreateBlockGroupForTesting();
        //}

        //[TearDown]
        //public void Clean()
        //{
        //    DeleteBlockGroupCreatedForTesting();
        //    DeleteFiveZipAreaCreatedForTesting();
        //}

        [Test]
        public void CanAddFiveZipImportedToDatabase()
        {
            int CampaignId;

            // Add the campaign to database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign c = CreateCampaign();
                ws.Repositories.CampaignRepository.Create(c);
                ws.Commit();
                CampaignId = c.Id;
            }

            // Add a CampaignFiveZipImported to existing Campaign
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                CampaignFiveZipImported cfzi = new CampaignFiveZipImported()
                {
                    FiveZipArea = new FiveZipArea() { Id = FiveZipAreaId },
                    Total = 10,
                    Penetration = 2,
                    Campaign = existing
                };

                existing.CampaignFiveZipImporteds.Add(cfzi);

                ws.Repositories.CampaignRepository.Update(existing);
                ws.Commit();
            }

            // Verify the CampaignFiveZipImported has been saved to database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                Assert.AreEqual(1, existing.CampaignFiveZipImporteds.Count);
                Assert.AreEqual(FiveZipAreaId, existing.CampaignFiveZipImporteds.ElementAt(0).FiveZipArea.Id);
            }

            // Delete the Campaign from database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign c = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                ws.Repositories.CampaignRepository.Delete(c);
                ws.Commit();
            }
        }

        [Test]
        public void CanAddBlockGroupImportedToDatabase()
        {

            // Add the campaign to database
            //
            Campaign c = CreateCampaign();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                
                ws.Repositories.CampaignRepository.Create(c);
                ws.Commit();
            }

            // Add a CampaignFiveZipImported to existing Campaign
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(c.Id);
                CampaignBlockGroupImported cbgi = new CampaignBlockGroupImported()
                {
                    BlockGroup = new BlockGroup() { Id=BlockGroupId },
                    Total = 10,
                    Penetration = 3,
                    Campaign = existing
                };

                existing.CampaignBlockGroupImporteds.Add(cbgi);

                ws.Repositories.CampaignRepository.Update(existing);
                ws.Commit();
            }

            // Verify the CampaignFiveZipImported has been saved to database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(c.Id);
                Assert.AreEqual(1, existing.CampaignBlockGroupImporteds.Count);
                Assert.AreEqual(BlockGroupId, existing.CampaignBlockGroupImporteds.ElementAt(0).BlockGroup.Id);
            }

            // Delete the Campaign from database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign deleteCampaign = ws.Repositories.CampaignRepository.GetEntity(c.Id);
                ws.Repositories.CampaignRepository.Delete(deleteCampaign);
                ws.Commit();
            }
        }

        private Campaign CreateCampaign()
        {
            return new Campaign()
            {
                Name = Guid.NewGuid().ToString(),
                Description = "Campaign for DistributionJobRepositoryTest",
                CustemerName = "Harry Hacker",
                ClientCode = "HHH",
                Latitude = 1,
                Longitude = 1,
                Logo = "",
                UserName = "someone not really existing",
                Date = DateTime.Now,
                ZoomLevel = 1,
                ClientName = Guid.NewGuid().ToString(),
                ContactName = Guid.NewGuid().ToString(),
                AreaDescription = Guid.NewGuid().ToString()
            };
        }

        //private void CreateFiveZipAreaForTesting()
        //{
        //    using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
        //    {
        //        using (ITransaction tx = ws.BeginTransaction())
        //        {
        //            try
        //            {
        //                FiveZipArea fza = new FiveZipArea()
        //                {
        //                    Id = FiveZipAreaId,
        //                    Code = "123",
        //                    Description = Guid.NewGuid().ToString(),
        //                    IsEnabled = true,
        //                    IsInnerShape = false,
        //                    Latitude = 1.0,
        //                    Longitude = 1.0,
        //                    LSAD = "123",
        //                    LSADTrans = "123",
        //                    MaxLatitude = 1.0,
        //                    MaxLongitude = 1.0,
        //                    MinLatitude = 1.0,
        //                    MinLongitude = 1.0,
        //                    Name = "Created for testing",
        //                    PartCount = 1,
        //                    Penetration = 1,
        //                    StateCode = "06"
        //                };

        //                ws.Repositories.FiveZipRepository.AddFiveZipArea(fza);

        //                tx.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                tx.Rollback();
        //            }
        //        }
        //    }
        //}

        //private void CreateBlockGroupForTesting()
        //{
        //    using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
        //    {
        //        using (ITransaction tx = ws.BeginTransaction())
        //        {
        //            try
        //            {
        //                BlockGroup blockGroup = new BlockGroup()
        //                {
        //                    Id = BlockGroupId,
        //                    Code = "123",
        //                    Description = Guid.NewGuid().ToString(),
        //                    IsEnabled = true,
        //                    Latitude = 1.0,
        //                    Longitude = 1.0,
        //                    LSAD = "123",
        //                    LSADTrans = "123",
        //                    MaxLatitude = 1.0,
        //                    MaxLongitude = 1.0,
        //                    MinLatitude = 1.0,
        //                    MinLongitude = 1.0,
        //                    Name = "Created for testing",
        //                    Penetration = 1,
        //                    StateCode = "06",
        //                    CountyCode = "1",
        //                    TractCode = "1",
        //                    ArbitraryUniqueCode = "1"                            
        //                };

        //                ws.Repositories.BlockGroupRepository.AddBlockGroup(blockGroup);

        //                tx.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                tx.Rollback();
        //            }
        //        }
        //    }
        //}

        //private void DeleteFiveZipAreaCreatedForTesting()
        //{
        //    using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
        //    {
        //        using (ITransaction tx = ws.BeginTransaction())
        //        {
        //            try
        //            {
        //                ws.Repositories.FiveZipRepository.DeleteFiveZipArea(FiveZipAreaId);
        //                tx.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                tx.Rollback();
        //            }
        //        }
        //    }
        //}

        //private void DeleteBlockGroupCreatedForTesting()
        //{
        //    using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
        //    {
        //        using (ITransaction tx = ws.BeginTransaction())
        //        {
        //            try
        //            {
        //                ws.Repositories.BlockGroupRepository.DeleteBlockGroup(BlockGroupId);
        //                tx.Commit();
        //            }
        //            catch (Exception)
        //            {
        //                tx.Rollback();
        //            }
        //        }
        //    }
        //}
    }
}
