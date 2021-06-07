using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GPS.DataLayer;

namespace GPS.DataLayer.Test
{
    [TestFixture]
    public class WorkSpaceManagerTest
    {
        [Test]
        public void CanGetWorkSpace()
        {
            IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace();
            Assert.IsNotNull(ws);
            Assert.IsTrue(ws.IsConnected);
            Assert.IsFalse(ws.IsClosed);
            Assert.IsNotNull(ws.Repositories);
        }

        [Test]
        public void CanConnectAndDisconnect()
        {
            IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace();
            Assert.IsTrue(ws.IsConnected);

            ws.Disconnect();
            Assert.IsFalse(ws.IsConnected);
            Assert.IsFalse(ws.IsClosed);

            ws.Connect();
            Assert.IsTrue(ws.IsConnected);
            Assert.IsFalse(ws.IsClosed);
        }

        [Test]
        public void CanClose()
        {
            IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace();
            Assert.IsFalse(ws.IsClosed);

            ws.Close();
            Assert.IsTrue(ws.IsClosed);
        }

        [Test]
        public void CanGetManyWorkSpaces()
        {
            for (int i = 0; i < 1000; i++)
            {
                WorkSpaceManager.Instance.NewWorkSpace();
            }
        }
    }
}
