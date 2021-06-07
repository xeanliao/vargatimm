using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMM.GTUService;
using GTUService.TIMM;
using System.Net;

namespace UnitTest
{
    public class GTUInfoDaoTest
    {
        private static GTU _Info;
        private static int _TaskId;
        static GTUInfoDaoTest()
        {
            _Info = new GTU();
            _Info.Speed = 10;
            _Info.Heading = 20;
            IPHostEntry ipEntry = Dns.GetHostByName(Dns.GetHostName());
            _Info.IPAddress = (ipEntry.AddressList.Length == 0) ? "" : ipEntry.AddressList[0].ToString();
            _Info.AreaCode = 30;
            _Info.NetworkCode = 4;
            _Info.CellID = 50;
            _Info.GPSFix = 7;
            _Info.Accuracy = 9;
            _Info.Count = 5;
            _Info.LocationID = 6;
            _Info.Version = "1";
            _Info.CurrentCoordinate = new Coordinate();
            _Info.CurrentCoordinate.Altitude = 111;
            _Info.CurrentCoordinate.Latitude = 222;
            _Info.CurrentCoordinate.Longitude = 333;
            _Info.SendTime = DateTime.Now;
            _Info.ReceivedTime = DateTime.Now;
            _Info.PowerInfo = PowerInfo.ON;
            _Info.Code = DateTime.Now.ToString("yyyyMMddHHmmss");

            //_TaskId = 27;
    }

        public void CURDGTUInfo()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            
            dao.InsertGTUInfo(_Info, 41);


            dao.UpdateGTU(_Info);
        }

        public void CheckInsertAndIsExistGTU()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            string code = DateTime.Now.ToString("yyyyMMddhhmmss");
            dao.InsertGTU(code);

            dao.IsExistGTU(code);
        }

        public void AvailableMappingId()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            dao.AvailableMappingId(_Info);

        }

        public void GetGTULstByTaskId()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            dao.getGTULstByTaskId(_TaskId);
        }

        public void GetMailAddress()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            dao.getMailAddress(_TaskId);
        }

        public void GetDMName()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            dao.getDMName(_TaskId);    
        }

        public void GetDmCollectionByTaskId()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            dao.getDmCollectionByTaskId(_TaskId);
        }

        public void GetNDAreaIdsByTaskId()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            dao.getNDAreaIdsByTaskId(_TaskId);
        }

        public void getNDAreaIdsByBoxes()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            dao.getNDAreaIdsByBoxes(new List<int>() { 339988160 });
        }

        public void GetUserForEmail()
        {
            IGTUInfoDAO dao = DaoFactory.CreateInstance<IGTUInfoDAO>();
            dao.getUserForEmail("20110506164158");
            
        }
    }
}
