using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Distribution;
using GPS.DataLayer.ValueObjects;
using GPS.DataLayer.DataInfrastructure;

namespace GPS.DataLayer.Test
{
    [TestFixture]
    public class WorkSpaceUsagePatternsTest
    {
        const int BlockGroupId = Int32.MinValue + 2;
        int CampaignId;
        int UserId;
        int SubMapId;

        //[SetUp]
        //public void SetUp()
        //{
        //    CreateBlockGroupForTesting();
        //    GenerateNewCampaignId();
        //    GenerateNewUserId();
        //    GenerateNewSubMapId();
        //}

        //[TearDown]
        //public void Clean()
        //{
        //    DeleteBlockGroupCreatedForTesting();
        //}

        [Test]
        public void OneToMany_CanAddChildObjectsByParentObject()
        {
            // Add the campaign to database
            //
            AddCampaignToDatabase(CampaignId);

            // Add a CampaignFiveZipImported to existing Campaign
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);

                // Verify there is no CampaignBlockGroupImported associated with this Campaign
                //
                Assert.AreEqual(0, existing.CampaignBlockGroupImporteds.Count);

                // Add a new CampaignBlockGroupImported to the Campaign
                //
                CampaignBlockGroupImported cbgi = new CampaignBlockGroupImported()
                {
                    BlockGroup = new BlockGroup() { Id=BlockGroupId },
                    Total = 10,
                    Penetration = 3,
                    Campaign = existing
                };

                existing.CampaignBlockGroupImporteds.Add(cbgi);

                // Store child object to database by Updating parent object
                //
                ws.Repositories.CampaignRepository.Update(existing);
                ws.Commit();
            }

            // Verify the CampaignBlockGroupImported has been saved to database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                Assert.AreEqual(1, existing.CampaignBlockGroupImporteds.Count);
                Assert.AreEqual(BlockGroupId, existing.CampaignBlockGroupImporteds.ElementAt(0).BlockGroup.Id);
            }

            // Delete the Campaign from database
            //
            DeleteCampaignFromDatabase(CampaignId);
        }

        [Test]
        public void CanAddManyObjectsByStatelessSession()
        {
            // Add the campaign to database
            //
            AddCampaignToDatabase(CampaignId);

            // Add a CampaignFiveZipImported to existing Campaign
            //
            using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
            {
                using (ITransaction tx = bws.BeginTransaction())
                {
                    DateTime start = DateTime.Now;

                    // Add a new CampaignBlockGroupImported to the Campaign
                    //
                    for (int i = 0; i < 1000; i++)
                    {
                        CampaignBlockGroupImported cbgi = new CampaignBlockGroupImported()
                        {
                            BlockGroup = new BlockGroup() { Id = BlockGroupId },
                            Total = 10,
                            Penetration = 3,
                            Campaign = new Campaign() { Id = CampaignId }
                        };

                        bws.Repositories.BulkCampaignBGRepository.Add(cbgi);
                    }

                    // Store child object to database by Updating parent object
                    //
                    tx.Commit();

                    DateTime end = DateTime.Now;

                    System.Diagnostics.Trace.WriteLine(end - start, "Duration");
                }
            }

            // Verify the CampaignBlockGroupImported has been saved to database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                Assert.AreEqual(1000, existing.CampaignBlockGroupImporteds.Count);
                Assert.AreEqual(BlockGroupId, existing.CampaignBlockGroupImporteds.ElementAt(0).BlockGroup.Id);
            }

            // Delete the Campaign from database
            //
            DeleteCampaignFromDatabase(CampaignId);
        }

        [Test]
        public void CanDeleteManyObjectsByStatelessSession()
        {
            // Add the campaign to database
            //
            AddCampaignToDatabase(CampaignId);

            // Add a CampaignFiveZipImported to existing Campaign
            //
            using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
            {
                using (ITransaction tx = bws.BeginTransaction())
                {
                    DateTime start = DateTime.Now;

                    // Add a new CampaignBlockGroupImported to the Campaign
                    //
                    for (int i = 0; i < 1000; i++)
                    {
                        CampaignBlockGroupImported cbgi = new CampaignBlockGroupImported()
                        {
                            BlockGroup = new BlockGroup() { Id = BlockGroupId },
                            Total = 10,
                            Penetration = 3,
                            Campaign = new Campaign() { Id = CampaignId }
                        };

                        bws.Repositories.BulkCampaignBGRepository.Add(cbgi);
                    }

                    // Store child object to database by Updating parent object
                    //
                    tx.Commit();

                    DateTime end = DateTime.Now;

                    System.Diagnostics.Trace.WriteLine(end - start, "Duration");
                }
            }

            // Verify the CampaignBlockGroupImported has been saved to database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                Assert.AreEqual(1000, existing.CampaignBlockGroupImporteds.Count);
                Assert.AreEqual(BlockGroupId, existing.CampaignBlockGroupImporteds.ElementAt(0).BlockGroup.Id);
            }

            // Delete all CampaignFiveZipImporteds of the existing Campaign
            //
            using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
            {
                using (ITransaction tx = bws.BeginTransaction())
                {
                    bws.Repositories.BulkCampaignBGRepository.DeleteAllCampaignBlockGroupImportedsOfCampaign(CampaignId);
                    tx.Commit();
                }               
            }

            // Verify the CampaignBlockGroupImported has been removed from database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                Assert.AreEqual(0, existing.CampaignBlockGroupImporteds.Count);
            }

            // Delete the Campaign from database
            //
            DeleteCampaignFromDatabase(CampaignId);
        }

        [Test]
        public void OneToMany_CanDeleteChildObjectsByParentObject()
        {
            // Add the campaign to database
            //
            AddCampaignToDatabase(CampaignId);

            // Add a CampaignFiveZipImported to existing Campaign
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);

                // Verify there is no CampaignBlockGroupImported associated with this Campaign
                //
                Assert.AreEqual(0, existing.CampaignBlockGroupImporteds.Count);

                // Add a new CampaignBlockGroupImported to the Campaign
                //
                CampaignBlockGroupImported cbgi = new CampaignBlockGroupImported()
                {
                    BlockGroup = new BlockGroup() { Id = BlockGroupId },
                    Total = 10,
                    Penetration = 3,
                    Campaign = existing
                };

                existing.CampaignBlockGroupImporteds.Add(cbgi);

                // Store child object to database by Updating parent object
                //
                ws.Repositories.CampaignRepository.Update(existing);
                ws.Commit();
            }

            // Delete the CampaignFiveZipImported from Campaign
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);

                // Verify the CampaignBlockGroupImported is saved to database
                //
                Assert.AreEqual(1, existing.CampaignBlockGroupImporteds.Count);

                // Remove CampaignBlockGroupImporteds from the Campaign
                //
                existing.CampaignBlockGroupImporteds.Clear();

                // Remove child object from database by Updating parent object
                //
                ws.Repositories.CampaignRepository.Update(existing);
                ws.Commit();
            }

            // Verify the CampaignBlockGroupImported has been deleted from database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign existing = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                Assert.AreEqual(0, existing.CampaignBlockGroupImporteds.Count);
            }

            // Delete the Campaign from database
            //
            DeleteCampaignFromDatabase(CampaignId);
        }

        [Test]
        public void OneToOne_CanAddAndDeleteChildObjectByParentObject()
        {
            AddCampaignToDatabase(CampaignId);
            AddUserToDatabase(UserId);
            AddDistributionJobToDatabase(CampaignId);

            // Assign AuditorAssignment to DistributionJob
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                // Verify there is existing DistributionJob in the database
                //
                IEnumerable<DistributionJob> djs = 
                    ws.Repositories.DistributionJobRepository.GetDistributionJobsForCampaign(new Campaign() { Id = CampaignId });                
                Assert.AreEqual(1, djs.Count());

                // Assign a single child to the parent
                //
                DistributionJob dj = djs.ElementAt(0);
                System.Diagnostics.Trace.WriteLine(UserId, "UserId");
                User u = ws.Repositories.UserRepository.GetUser(UserId);
                Assert.IsNotNull(u);

                AuditorAssignment auditor = DistributionJobFactory.CreateAuditorFromUser(dj, u);
                dj.AssignAuditor(auditor);

                ws.Repositories.DistributionJobRepository.UpdateDistributionJob(dj);
                ws.Commit();
            }

            // Verify the AuditorAssignment is stored to the database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<DistributionJob> djs = 
                    ws.Repositories.DistributionJobRepository.GetDistributionJobsForCampaign(new Campaign() { Id = CampaignId });
                Assert.AreEqual(1, djs.Count());

                DistributionJob dj = djs.ElementAt(0);
                Assert.IsNotNull(dj.AuditorAssignment);
                Assert.IsNotNull(dj.AuditorAssignment.LoginUser);
                Assert.AreNotEqual(string.Empty, dj.AuditorAssignment.FullName);
            }

            // Delete the AuditorAssignment from database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<DistributionJob> djs = 
                    ws.Repositories.DistributionJobRepository.GetDistributionJobsForCampaign(new Campaign() { Id = CampaignId });
                Assert.AreEqual(1, djs.Count());

                DistributionJob dj = djs.ElementAt(0);
                dj.RemoveAuditor();

                ws.Repositories.DistributionJobRepository.UpdateDistributionJob(dj);
                ws.Commit();
            }

            // Verify the AuditorAssignment is deleted from the database
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                IEnumerable<DistributionJob> djs = 
                    ws.Repositories.DistributionJobRepository.GetDistributionJobsForCampaign(new Campaign() { Id = CampaignId });
                Assert.AreEqual(1, djs.Count());

                DistributionJob dj = djs.ElementAt(0);
                Assert.IsNull(dj.AuditorAssignment);
            }

            DeleteDistributionJobFromDatabase(CampaignId);
            DeleteUserFromDatabase(UserId);
            DeleteCampaignFromDatabase(CampaignId);
        }

        [Test]
        public void CanUseSequenceIdentifier()
        {
            // Add the campaign to database
            //
            AddCampaignToDatabase(CampaignId);
            // Add a Sub Map to the Campaign
            //
            AddSubMapToCampaign(SubMapId, CampaignId);

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (ITransaction tx = ws.BeginTransaction())
                {
                    Campaign c = ws.Repositories.CampaignRepository.GetEntity(CampaignId);
                    Assert.IsTrue(c.SubMaps.Count > 0);
                    var subMap = ws.Repositories.SubMapRepository.GetEntity(SubMapId);
                    for (int i = 0; i < 5000; i++)
                    {
                        // The new SubMapCoordinate object
                        //
                        SubMapCoordinate smc = new SubMapCoordinate()
                        {
                            SubMap = subMap,
                            Latitude = 1,
                            Longitude = 1
                        };
                        c.SubMaps.ElementAt(0).SubMapCoordinates.Add(smc);
                    }

                    ws.Repositories.CampaignRepository.Update(c);
                    tx.Commit();
                }
            }

            DeleteCampaignFromDatabase(CampaignId);
        }

        //private void GenerateNewCampaignId() { CampaignId = Guid.NewGuid().GetHashCode(); }

        //private void GenerateNewUserId() { UserId = Guid.NewGuid().GetHashCode(); }

        //private void GenerateNewSubMapId() { SubMapId = Guid.NewGuid().GetHashCode(); }

        private void AddSubMapToCampaign(int submapId, int campaignId)
        {
            SubMap sm = new SubMap()
            {
 
                OrderId = 1,
                Name = Guid.NewGuid().ToString(),
                Total = 2,
                Penetration = 1,
                Percentage = 0.4,
                ColorG = 1,
                ColorB = 1,
                ColorR = 1,
                ColorString = "color string"
            };

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign c = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                sm.Campaign = c;
                c.SubMaps.Add(sm);
                ws.Commit();
            }
        }

        private void AddCampaignToDatabase(int campaignId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign c = CreateCampaign(campaignId);
                ws.Repositories.CampaignRepository.Create(c);
                ws.Commit();
            }
        }

        private void AddDistributionJobToDatabase(int campaignId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                DistributionJob c = CreateDistributionJob(campaignId);
                ws.Repositories.DistributionJobRepository.AddDistributionJob(c);
                ws.Commit();
            }
        }

        private void DeleteDistributionJobFromDatabase(int campaignId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign c = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                IEnumerable<DistributionJob> djs = ws.Repositories.DistributionJobRepository.GetDistributionJobsForCampaign(c);
                foreach (DistributionJob dj in djs)
                {
                    ws.Repositories.DistributionJobRepository.DeleteDistributionJob(dj);
                }
                ws.Commit();
            }
        }

        private void DeleteCampaignFromDatabase(int campaignId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign c = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                ws.Repositories.CampaignRepository.Delete(c);
                ws.Commit();
            }
        }

        private void AddUserToDatabase(int userId)
        {
            System.Diagnostics.Trace.WriteLine(userId, "UserId");

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        string userName = Guid.NewGuid().ToString();
                        User u = new User()
                        {
                            Id = userId,
                            Enabled = true,
                            FullName = "someone",
                            Password = "",
                            Role = UserRoles.Admin,
                            UserCode = "123",
                            UserName = userName
                        };
                        ws.Repositories.UserRepository.AddUser(u);
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        Assert.Fail(ex.ToString());
                    }
                }                    
            }
        }

        private void DeleteUserFromDatabase(int userId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                User u = ws.Repositories.UserRepository.GetUser(userId);
                ws.Repositories.UserRepository.DeleteUser(u.UserName);
                ws.Commit();
            }
        }

        private Campaign CreateCampaign(int campaignId)
        {
            return new Campaign()
            {
                Id = campaignId,
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

        private DistributionJob CreateDistributionJob(int campaignId)
        {
            string djName = Guid.NewGuid().ToString();
            return DistributionJobFactory.CreateDistributionJob(new Campaign() { Id = campaignId }, djName);
        }

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
