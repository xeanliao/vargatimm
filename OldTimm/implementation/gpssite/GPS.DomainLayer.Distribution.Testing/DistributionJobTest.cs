using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Mocks;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Distribution;

namespace GPS.DomainLayer.Distribution.Testing
{
    [TestFixture]
    public class DistributionJobTest
    {
        private Campaign _campaign;
        private DistributionJob _distributionJob;

        [SetUp]
        public void Setup()
        {
            _campaign = new Campaign() { Id = Guid.NewGuid().GetHashCode() };
            _distributionJob =  DistributionJobFactory.CreateDistributionJob(_campaign, Guid.NewGuid().ToString());
        }

        [TearDown]
        public void Clean()
        {
            _distributionJob = null;
        }

        [Test]
        public void Can_Create_DistributionJob()
        {
            string djName = "MyDJ";
            DistributionJob dj = DistributionJobFactory.CreateDistributionJob(_campaign, djName);

            Assert.IsNotNull(dj);
            Assert.IsNotNull(dj.Campaign);
            Assert.AreEqual(0, dj.DistributionMaps.Count());
            Assert.AreEqual(dj.Name, djName);
            Assert.IsNull(dj.AuditorAssignment);
            Assert.IsNull(dj.DriverAssignments);
            Assert.IsNull(dj.WalkerAssignments);
        }

        [Test]
        public void Can_Assign_Login_User_As_Auditor()
        {
            User loginUser = new User() { Id = 1 };

            AuditorAssignment auditor = DistributionJobFactory.CreateAuditorFromUser(_distributionJob, loginUser);
            _distributionJob.AssignAuditor(auditor);

            Assert.AreEqual(loginUser.Id, _distributionJob.AuditorAssignment.LoginUser.Id);
            Assert.AreEqual(loginUser.FullName, _distributionJob.AuditorAssignment.FullName);
            Assert.AreEqual(UserRoles.Auditor, _distributionJob.AuditorAssignment.DjRole);
        }

        [Test]
        public void Can_Create_Login_User_As_Walker()
        {
            User loginUser = new User() { Id = 1 };
            WalkerAssignment walker = DistributionJobFactory.CreateWalkerFromUser(_distributionJob, loginUser);
            Assert.AreEqual(loginUser, walker.LoginUser);
            Assert.AreEqual(loginUser.FullName, walker.FullName);
            Assert.AreEqual(_distributionJob, walker.DistributionJob);
        }

        [Test]
        public void Can_Assign_Driver()
        {
            IEnumerable<DriverAssignment> drivers = FakeDrivers();
            foreach (DriverAssignment d in drivers)
            {
                _distributionJob.AssignOneMoreDriver(d);
            }

            foreach (DriverAssignment d in drivers)
            {
                Assert.IsTrue(_distributionJob.DriverAssignments.Contains(d));
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DriverFullNameShouldBeUniqueForTheDistributionJob()
        {
            DistributionJob dj = DistributionJobFactory.CreateDistributionJob(new Campaign(), Guid.NewGuid().ToString());
            DriverAssignment driver = DistributionJobFactory.CreateDriverFromUser(dj, new User() { FullName="user1" });
            DriverAssignment anotherDriver = DistributionJobFactory.CreateDriverFromUser(dj, new User() { FullName = "user1" });

            Assert.AreEqual(driver.FullName, anotherDriver.FullName);

            dj.AssignOneMoreDriver(driver);
            dj.AssignOneMoreDriver(anotherDriver);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WalkerFullNameShouldBeUniqueForTheDistributionJob()
        {
            DistributionJob dj = DistributionJobFactory.CreateDistributionJob(new Campaign(), Guid.NewGuid().ToString());
            WalkerAssignment walker = DistributionJobFactory.CreateWalkerFromUser(dj, new User() { FullName = "user1" });
            WalkerAssignment anotherWalker = DistributionJobFactory.CreateWalker(dj, "user1");

            Assert.AreEqual(walker.FullName, anotherWalker.FullName);

            dj.AssignOneMoreWalker(walker);
            dj.AssignOneMoreWalker(anotherWalker);
        }

        [Test]
        public void Can_Assign_Walker()
        {
            IEnumerable<WalkerAssignment> walkers = FakeWalkers();
            foreach (WalkerAssignment d in walkers)
            {
                _distributionJob.AssignOneMoreWalker(d);
            }

            foreach (WalkerAssignment d in walkers)
            {
                Assert.IsTrue(_distributionJob.WalkerAssignments.Contains(d));
                Assert.AreEqual(UserRoles.Walker, d.DjRole);
            }           
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Can_Not_Assign_Gtu_To_Auditor_If_No_Auditor()
        {
            Gtu g = new Gtu();
            _distributionJob.AssignGtuToAuditor(g);
        }

        [Test]
        public void Can_Assign_Gtu_To_Auditor()
        {
            User loginUser = new User() { Id = 1 };
            
            AuditorAssignment auditor = DistributionJobFactory.CreateAuditorFromUser(_distributionJob, loginUser);
            _distributionJob.AssignAuditor(auditor);

            Gtu g = new Gtu();
            _distributionJob.AssignGtuToAuditor(g);

            Assert.AreEqual(g, _distributionJob.AuditorAssignment.Gtu);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Can_Not_Assign_Gtu_To_Driver_If_Driver_Is_Not_Assigned()
        {
            Gtu g = new Gtu();
            DriverAssignment driver = DistributionJobFactory.CreateDriver(_distributionJob, Guid.NewGuid().ToString()); // this driver is NOT assigned to the DJ
            _distributionJob.AssignGtuToDriver(driver, g);
        }

        [Test]
        public void can_assign_gtu_to_driver()
        {
            DriverAssignment driver = DistributionJobFactory.CreateDriver(_distributionJob, Guid.NewGuid().ToString());
            _distributionJob.AssignOneMoreDriver(driver);
            
            Assert.IsTrue(_distributionJob.DriverAssignments.Contains(driver));

            Gtu g = new Gtu();
            _distributionJob.AssignGtuToDriver(driver, g);

            Assert.AreEqual(g, driver.Gtu);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void can_not_assign_gtu_to_walker_if_walker_is_not_assigned()
        {
            Gtu g = new Gtu();
            WalkerAssignment walker = DistributionJobFactory.CreateWalker(_distributionJob, Guid.NewGuid().ToString()); // this walker is NOT assigned to DJ
            _distributionJob.AssignGtuToWalker(walker, g);
        }

        [Test]
        public void can_assign_gtu_to_walker()
        {
            WalkerAssignment walker = DistributionJobFactory.CreateWalker(_distributionJob, Guid.NewGuid().ToString());
            _distributionJob.AssignOneMoreWalker(walker);
            
            Assert.IsTrue(_distributionJob.WalkerAssignments.Contains(walker));

            Gtu g = new Gtu();
            _distributionJob.AssignGtuToWalker(walker, g);

            Assert.AreEqual(g, walker.Gtu);
        }

        #region helper methods
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
