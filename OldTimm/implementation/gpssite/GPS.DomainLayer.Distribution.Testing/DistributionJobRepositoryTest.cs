using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Mocks;
using NHibernate;
using NHibernate.Cfg;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Distribution;

namespace GPS.DomainLayer.Distribution.Testing
{
    [TestFixture]
    public class DistributionJobRepositoryTest
    {
        #region fields
        private IWorkSpace _workspace;
        private ICampaignRepository _campaignRepository;
        private Campaign _campaign;
        private SubMap _submap;
        private DistributionMap _distributionMap;
        private IDistributionJobRepository _distributionJobRepository;
        private DistributionJob _distributionJob;
        private User _loginUser;
        private Gtu _auditorGtu;
        private Gtu _driverGtu;
        private Gtu _walkerGtu;
        #endregion

        #region setup code
        [SetUp]        
        public void Setup()
        {
            _workspace = WorkSpaceManager.Instance.NewWorkSpace();
            _campaignRepository = _workspace.Repositories.CampaignRepository;
            _distributionJobRepository = _workspace.Repositories.DistributionJobRepository;

            // Fake campaign, sub map, and distribution map
            CreateCampaign();
            CreateSubMap();
            CreateDistributionMap();

            // Persist the campaign
            _campaignRepository.Create(_campaign);
            _workspace.Commit();

            // Make sure the campaign is persisted successfully
            Campaign campaignInDb = _workspace.Repositories.CampaignRepository.GetEntity(_campaign.Id);
            
            Assert.AreEqual(_campaign.SubMaps.Count, campaignInDb.SubMaps.Count);
            Assert.AreEqual(1, campaignInDb.SubMaps[0].DistributionMaps.Count);

            // Fake the distribution job
            CreateDistributionJob();

            _distributionJobRepository.AddDistributionJob(_distributionJob);
            _workspace.Commit();

            _loginUser = CreateUser();

            // Create GTUs
            IGtuRepository gtuRep = _workspace.Repositories.GtuRepository;
            
            _auditorGtu = CreateGtu();
            gtuRep.AddGtu(_auditorGtu);

            _driverGtu = CreateGtu();
            gtuRep.AddGtu(_driverGtu);

            _walkerGtu = CreateGtu();
            gtuRep.AddGtu(_walkerGtu);

            _workspace.Commit();
        }

        [TearDown]
        public void Clean()
        {
            _distributionJobRepository.DeleteDistributionJob(_distributionJob);

            _campaignRepository.Delete(_campaign);

            _workspace.Repositories.GtuRepository.DeleteGtu(_auditorGtu.UniqueID);
            _workspace.Repositories.GtuRepository.DeleteGtu(_driverGtu.UniqueID);
            _workspace.Repositories.GtuRepository.DeleteGtu(_walkerGtu.UniqueID);

            _workspace.Repositories.UserRepository.DeleteUser(_loginUser.UserName);

            _workspace.Commit();
            _workspace.Close();
        }

        #endregion

        [Test]
        public void Can_Add_DistributionJob_without_Participants()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                // Make sure the distribution job is persisted successfully
                DistributionJob distributionJobInDb =
                    ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id);

                Assert.AreEqual(_distributionJob.Id, distributionJobInDb.Id);
                Assert.AreEqual(_distributionJob.Name, distributionJobInDb.Name);
                Assert.AreEqual(0, _distributionJob.DistributionMaps.Count());
                Assert.AreEqual(0, distributionJobInDb.DistributionMaps.Count());
            }
        }

        [Test]
        public void Can_Save_Auditor_together_with_DistributionJob()
        {
            AuditorAssignment auditor = DistributionJobFactory.CreateAuditorFromUser(_distributionJob, _loginUser);
            _distributionJob.AssignAuditor(auditor);
            _distributionJobRepository.UpdateDistributionJob(_distributionJob);
            _workspace.Commit();

            AuditorAssignment anotherAuditor = DistributionJobFactory.CreateAuditorFromUser(_distributionJob, _loginUser);
            _distributionJob.AssignAuditor(anotherAuditor);
            _distributionJobRepository.UpdateDistributionJob(_distributionJob);
            _workspace.Commit();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                AuditorAssignment p = 
                    ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id).AuditorAssignment;
                Assert.IsNotNull(p);
                Assert.AreEqual(p.LoginUser.Id, _distributionJob.AuditorAssignment.LoginUser.Id);
                Assert.AreEqual(p.DjRole, _distributionJob.AuditorAssignment.DjRole);
            }

            _distributionJob.AssignGtuToAuditor(_auditorGtu);
            _workspace.Commit();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                DistributionJob djInDb =
                    ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id);
                Assert.IsNotNull(djInDb);
                Assert.IsNotNull(djInDb.AuditorAssignment);
                Assert.IsNotNull(djInDb.AuditorAssignment.Gtu);
            }
        }

        [Test]
        public void Can_Save_Drivers_together_with_DistributionJob()
        {
            DriverAssignment driver = DistributionJobFactory.CreateDriverFromUser(_distributionJob, _loginUser);
            _distributionJob.AssignOneMoreDriver(driver);
            _distributionJobRepository.UpdateDistributionJob(_distributionJob);
            _workspace.Commit();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                DistributionJob p =
                    ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id);
                DriverAssignment assignedDriver = _distributionJob.DriverAssignments.ElementAt(0);

                Assert.IsNotNull(p);
                Assert.AreEqual(driver.DistributionJob.Id, assignedDriver.DistributionJob.Id);
                Assert.AreEqual(driver.FullName, assignedDriver.FullName);
                Assert.AreEqual(driver.DjRole, assignedDriver.DjRole);
            }

            AuditorAssignment auditor = DistributionJobFactory.CreateAuditorFromUser(_distributionJob, _loginUser);
            _distributionJob.AssignAuditor(auditor);
            _distributionJob.AssignGtuToAuditor(_auditorGtu);
            _distributionJob.AssignGtuToDriver(driver, _driverGtu);
            _workspace.Commit();

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                DistributionJob djInDb =
                    ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id);

                Assert.IsNotNull(djInDb);
                Assert.IsNotNull(djInDb.AuditorAssignment);
                Assert.IsNotNull(djInDb.AuditorAssignment.Gtu);
                Assert.IsNotNull(djInDb.DriverAssignments.ElementAt(0).Gtu);
            }
        }

        [Test]
        public void Can_Save_Walkers_together_with_DistributionJob()
        {
            WalkerAssignment walker = DistributionJobFactory.CreateWalker(_distributionJob, Guid.NewGuid().ToString());
            _distributionJob.AssignOneMoreWalker(walker);
            _distributionJobRepository.UpdateDistributionJob(_distributionJob);
            _workspace.Commit();

            WalkerAssignment assignedWalker = _distributionJob.WalkerAssignments.ElementAt(0);

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                DistributionJob djInDb =
                    ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id);
                WalkerAssignment p = djInDb.WalkerAssignments.ElementAt(0);
                Assert.IsNotNull(p);
                Assert.AreEqual(p.DjRole, assignedWalker.DjRole);
            }
        }

        [Test]
        public void Can_Fetch_DistributionJobs_for_Campaign()
        {
            DistributionJob a = DistributionJobFactory.CreateDistributionJob(_campaign, Guid.NewGuid().ToString());
            _distributionJobRepository.AddDistributionJob(a);

            DistributionJob b = DistributionJobFactory.CreateDistributionJob(_campaign, Guid.NewGuid().ToString());
            _distributionJobRepository.AddDistributionJob(b);

            IEnumerable<DistributionJob> djs = _distributionJobRepository.GetDistributionJobsForCampaign(_campaign);
            Assert.IsTrue(djs.Count() >= 2);

            _distributionJobRepository.DeleteDistributionJob(a);
            _distributionJobRepository.DeleteDistributionJob(b);
            _workspace.Commit();
        }

        [Test]
        public void CanGetDistributionJobByID()
        {
            DistributionJob a = DistributionJobFactory.CreateDistributionJob(_campaign, Guid.NewGuid().ToString());
            _distributionJobRepository.AddDistributionJob(a);
            _workspace.Commit();

            DistributionJob djInDb = _distributionJobRepository.GetDistributionJob(a.Id);
            Assert.IsNotNull(djInDb);

            _distributionJobRepository.DeleteDistributionJob(djInDb);
            _workspace.Commit();
        }

        [Test]
        public void CanRemoveAuditorFromDistributionJob()
        {
            AuditorAssignment a = DistributionJobFactory.CreateAuditorFromUser(_distributionJob, _loginUser);
            _distributionJob.AssignAuditor(a);
            _distributionJobRepository.UpdateDistributionJob(_distributionJob);
            _workspace.Commit();

            _distributionJob.RemoveAuditor();
            _distributionJobRepository.UpdateDistributionJob(_distributionJob);
            _workspace.Commit();

            Assert.IsNull(_distributionJob.AuditorAssignment);

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                DistributionJob djInDb = ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id);
                Assert.IsNull(djInDb.AuditorAssignment);
            }
        }

        [Test]
        public void CanReplaceDriversForDistributionJob()
        {
            #region Case 1: the DJ contains no preexisting Drivers
            // Case 1: the DJ contains no preexisting Drivers
            //
            IEnumerable<DriverAssignment> drivers = FakeDrivers();
            _distributionJob.ReplaceDriversWith(drivers);

            // Make sure drivers are added to DJ successfully
            // 
            Assert.AreEqual(drivers.Count(), _distributionJob.DriverAssignments.Count());
            foreach (DriverAssignment d in drivers)
            {
                Assert.IsTrue(_distributionJob.DriverAssignments.Contains(d));
            }

            // Persist the DJ together with drivers
            // 
            _distributionJobRepository.UpdateDistributionJob(_distributionJob);

            // Make sure drivers are persisted successfully
            //
            IEnumerable<DriverAssignment> driversInDb = null;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                driversInDb = ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id).DriverAssignments;

                Assert.AreEqual(drivers.Count(), driversInDb.Count());
                foreach (DriverAssignment d in drivers)
                {
                    Assert.IsNotNull(driversInDb.Where(t => t.FullName.ToLower() == d.FullName.ToLower()));
                }
            }
            #endregion

            #region Case 2: replace existing Drivers with an extended driver group
            // Case 2: replace existing Drivers with an extended driver group
            //
            IEnumerable<DriverAssignment> newDrivers = FakeDrivers(); // a new collection of drivers
            for (int i = 0; i < newDrivers.Count(); i++)
            {
                newDrivers.ElementAt(i).ClonePrototype(drivers.ElementAt(i)); // copy properties to the new collection of drivers
            }

            IList<DriverAssignment> bigGroup = new List<DriverAssignment>(newDrivers);
            DriverAssignment newDriver = DistributionJobFactory.CreateDriver(_distributionJob, "new driver");
            bigGroup.Add(newDriver);

            _distributionJob.ReplaceDriversWith(bigGroup);

            // Make sure drivers are added to DJ successfully
            // 
            Assert.AreEqual(bigGroup.Count(), _distributionJob.DriverAssignments.Count());
            foreach (DriverAssignment d in bigGroup)
            {
                Assert.IsTrue(_distributionJob.DriverAssignments.Contains(d));
            }

            // Persist the DJ together with drivers
            // 
            _distributionJobRepository.UpdateDistributionJob(_distributionJob);

            // Make sure drivers are persisted successfully
            //
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                driversInDb = ws.Repositories.DistributionJobRepository.GetDistributionJob(_distributionJob.Id).DriverAssignments;

                Assert.AreEqual(bigGroup.Count(), driversInDb.Count());
                foreach (DriverAssignment d in bigGroup)
                {
                    Assert.IsNotNull(driversInDb.Where(t => t.FullName.ToLower() == d.FullName.ToLower()));
                }
            }

            #endregion

            #region Case 3: replace existing Drivers with a sub group of existing drivers
            // Case 3: replace existing Drivers with a sub group of existing drivers
            //
            IList<DriverAssignment> smallGroup = new List<DriverAssignment>(bigGroup.Take(1));
            _distributionJob.ReplaceDriversWith(smallGroup);

            // Make sure the small group drivers are added to DJ successfully
            // 
            Assert.AreEqual(smallGroup.Count(), _distributionJob.DriverAssignments.Count());
            foreach (DriverAssignment d in smallGroup)
            {
                Assert.IsTrue(_distributionJob.DriverAssignments.Contains(d));
            }

            #endregion
        }

        [Test]
        public void CanReplaceWalkersForDistributionJob()
        {
            // Case 1: the DJ contains no preexisting Walkers
            //
            IEnumerable<WalkerAssignment> walkers = FakeWalkers();
            _distributionJob.ReplaceWalkersWith(walkers);

            // Make sure drivers are added to DJ successfully
            // 
            Assert.AreEqual(walkers.Count(), _distributionJob.WalkerAssignments.Count());
            foreach (WalkerAssignment w in walkers)
            {
                Assert.IsTrue(_distributionJob.WalkerAssignments.Contains(w));
            }

            // Case 2: replace existing Walkers with an extended walker group
            //
            IEnumerable<WalkerAssignment> newWalkers = FakeWalkers(); // a new collection of walkers
            for (int i = 0; i < newWalkers.Count(); i++)
            {
                newWalkers.ElementAt(i).ClonePrototype(walkers.ElementAt(i)); // copy properties to the new collection of walkers
            }

            IList<WalkerAssignment> bigGroup = new List<WalkerAssignment>(newWalkers);
            WalkerAssignment newWalker = DistributionJobFactory.CreateWalker(_distributionJob, "new walker");
            bigGroup.Add(newWalker);

            _distributionJob.ReplaceWalkersWith(bigGroup);

            // Make sure drivers are added to DJ successfully
            // 
            Assert.AreEqual(bigGroup.Count(), _distributionJob.WalkerAssignments.Count());
            foreach (WalkerAssignment d in bigGroup)
            {
                Assert.IsTrue(_distributionJob.WalkerAssignments.Contains(d));
            }

            // Case 3: replace existing Walkers with a sub group of existing Walkers
            //
            IEnumerable<WalkerAssignment> smallGroup = new List<WalkerAssignment>(bigGroup.Take(1));
            _distributionJob.ReplaceWalkersWith(smallGroup);

            Assert.AreEqual(smallGroup.Count(), _distributionJob.WalkerAssignments.Count());
            foreach (WalkerAssignment d in smallGroup)
            {
                Assert.IsTrue(_distributionJob.WalkerAssignments.Contains(d));
            }
        }

        #region helper methods
        private User CreateUser()
        {
            IUserRepository userRep = _workspace.Repositories.UserRepository;
            string userName = Guid.NewGuid().ToString();
            User loginUser = new User() 
            { 
                Id = Guid.NewGuid().GetHashCode(), 
                Enabled = true, 
                FullName = "someone", 
                Password = "", 
                Role = UserRoles.Admin, 
                UserCode = "", 
                UserName = userName 
            };
            userRep.AddUser(loginUser);
            _workspace.Commit();
            return loginUser;
        }

        private void CreateCampaign()
        {
            _campaign = new Campaign()
            {
                Id = Guid.NewGuid().GetHashCode(),
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

        private void CreateSubMap()
        {
            _submap = new SubMap()
            {
                OrderId = 1,
                Name = Guid.NewGuid().ToString(),
                Total = 2,
                Penetration = 1,
                Percentage = 0.4,
                ColorG = 1,
                ColorB = 1,
                ColorR = 1,
                ColorString = "color string",
                Campaign = _campaign
            };
            if (null == _campaign.SubMaps)
            {
                _campaign.SubMaps = new List<SubMap>() { _submap };
            }
            else
            {
                _campaign.SubMaps.Add(_submap);
            }
        }

        private void CreateDistributionMap()
        {
            _distributionMap = new DistributionMap()
            {
                Id = Guid.NewGuid().GetHashCode(),
                Name = Guid.NewGuid().ToString(),
                SubMapId = _submap.Id
            };
            if (null == _submap.DistributionMaps)
            {
                _submap.DistributionMaps = new List<DistributionMap>() { _distributionMap };
            }
            else
            {
                _submap.DistributionMaps.Add(_distributionMap);
            }
        }

        private void CreateDistributionJob()
        {
            string djName = Guid.NewGuid().ToString();
            _distributionJob = DistributionJobFactory.CreateDistributionJob(_campaign, djName);
        }

        private Gtu CreateGtu()
        {
            return new Gtu()
            {
                IsEnabled = true,
                Model = Guid.NewGuid().ToString(),
                UniqueID = Guid.NewGuid().ToString()
            };
        }

        private IEnumerable<WalkerAssignment> FakeWalkers()
        {
            IList<WalkerAssignment> walkers = new List<WalkerAssignment>()
            {
                DistributionJobFactory.CreateWalker(_distributionJob, Guid.NewGuid().ToString()),
                DistributionJobFactory.CreateWalker(_distributionJob, Guid.NewGuid().ToString()),
                DistributionJobFactory.CreateWalker(_distributionJob, Guid.NewGuid().ToString())
            };
            return walkers;
        }

        private IEnumerable<DriverAssignment> FakeDrivers()
        {
            IList<DriverAssignment> drivers = new List<DriverAssignment>()
            {
                DistributionJobFactory.CreateDriver(_distributionJob, Guid.NewGuid().ToString()),
                DistributionJobFactory.CreateDriver(_distributionJob, Guid.NewGuid().ToString()),
                DistributionJobFactory.CreateDriver(_distributionJob, Guid.NewGuid().ToString())
            };
            return drivers;
        }
        #endregion
    }
}