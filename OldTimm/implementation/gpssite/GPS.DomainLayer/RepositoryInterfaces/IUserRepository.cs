using System;
using System.Collections.Generic;
namespace GPS.DataLayer
{
    public interface IUserRepository
    {
        GPS.DomainLayer.Entities.User AddUser(GPS.DomainLayer.Entities.User user);
        void DeleteUser(string userName);
        System.Collections.Generic.IList<GPS.DomainLayer.Entities.User> GetAllUsers();
        GPS.DomainLayer.Entities.User GetUser(string userName, string password);
        GPS.DomainLayer.Entities.User GetUser(string userName);
        GPS.DomainLayer.Entities.User GetUser(int id);
        GPS.DomainLayer.Entities.User UpdateUser(GPS.DomainLayer.Entities.User user);
        IList<GPS.DomainLayer.Entities.User> GetAllUsersByPrivilege(int privilege);
        IList<GPS.DomainLayer.Entities.User> GetAllUsersByGroup(int groupId);
        IList<GPS.DomainLayer.Entities.User> GetAllUsersByGroups(int[] gs);
        IList<GPS.DomainLayer.Entities.User> GetWalkersByCampaignid(int cId);
        IList<GPS.DomainLayer.Entities.User> GetDriversByCampaignid(int cId);
        IList<GPS.DomainLayer.Entities.User> GetAuditorsByCampaignid(int cId);
        IList<GPS.DomainLayer.Entities.User> GetWalkersByTaskid(int tId);
        IList<GPS.DomainLayer.Entities.User> GetDriversByTaskid(int tId);
        IList<GPS.DomainLayer.Entities.User> GetAuditorsByTaskid(int tId);
        IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByTaskidUserid(int tId, int uId);
        IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByUseridYear(int uId);
        IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByUseridAll(int uId);
        IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByCampaignidUserid(int cId, int uId);
        //IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByCampaignidUseridYear(int cId, int uId);
        //IList<GPS.DomainLayer.Entities.Gtuinfo> GetGtuinfosByCampaignidUseridAll(int cId, int uId);
        IList<decimal> GetSpeedByTaskidUserid(int tId, int uId);
        IList<decimal> GetGroundByTaskidUserid(int tId, int uId);
        IList<double> GetStopByTaskidUserid(int tId, int uId);
        IList<decimal> GetSpeedByCampaignidUserid(int cId, int uId);
        IList<decimal> GetGroundByCampaignidUserid(int cId, int uId);
        IList<double> GetStopByCampaignidUserid(int cId, int uId);

        IList<decimal> GetSpeedByUseridYear(int uId);
        IList<decimal> GetGroundByUseridYear(int uId);
        IList<double> GetStopByUseridYear(int uId);
        IList<decimal> GetSpeedByUseridAll(int uId);
        IList<decimal> GetGroundByUseridAll(int uId);
        IList<double> GetStopByUseridAll(int uId);
        IList<string> GetUserNameById(int uId);
    }
}
