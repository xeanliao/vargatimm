using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;

namespace GPS.DataLayer.Test
{
    [TestFixture]
    public class WorkSpaceTest
    {
        public void test()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                var r = ws.Repositories.TaskRepository as GPS.DataLayer.RepositoryImplementations.TaskRepository;
                r.GetTaskByCamp(1923643364);
            }
        }

        [Test]
        public void CanCommitTransaction()
        {
            bool success = true;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        ws.Repositories.UserRepository.AddUser(new User()
                        {
                            Enabled = true,
                            FullName = Guid.NewGuid().ToString(),
                            Password = "pwd",
                            Role = UserRoles.None,
                            UserCode = "",
                            UserName = Guid.NewGuid().ToString()
                        });
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ws.Clear();

                        success = false;
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
            }

            Assert.IsTrue(success);
        }

        [Test]
        public void CanRollbackTransaction()
        {
            bool exceptionCaptured = false;

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        ws.Repositories.UserRepository.AddUser(new User()
                        {
                            Enabled = true,
                            FullName = Guid.NewGuid().ToString(),
                            Password = "pwd",
                            Role = UserRoles.None,
                            UserCode = "",
                            UserName = null
                        });
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ws.Clear();

                        exceptionCaptured = true;
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
            }

            Assert.IsTrue(exceptionCaptured);
        }
    }
}
