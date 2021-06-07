using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DataLayer;
using GPS.Website.TransferObjects;
using System.Collections;
using GPS.DataLayer.ValueObjects;
using System.Web.UI.WebControls;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;
using GPS.DomainLayer.Enum;
using log4net;


namespace GPS.Website.UserServices {
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ServiceContract(Namespace = "TIMM.Website.UserServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserReaderService {
        /// <summary>
        /// Return a list of all users.
        /// </summary>
        /// <returns>
        /// An <see cref="IList"/> containing all <see cref="User"/>s.
        /// </returns>
        [OperationContract]
        public IEnumerable<ToUser> GetAllUsers()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        IEnumerable<User> users = ws.Repositories.UserRepository.GetAllUsers();
                        return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(users);
                    }
                    catch (Exception ex) {
                        ILog logger = log4net.LogManager.GetLogger(typeof(UserReaderService));
                        logger.Error("GetAllUsers Failed.", ex);
                        return null; 
                    }
                }
            }
        }

        [OperationContract]
        public IEnumerable<ToUser> GetAllUsersWithoutAssignedGtu()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        var users = ws.Repositories.UserRepository.GetAllUsers();
                        var gtus = ws.Repositories.GtuRepository.GetAllGtus();

                        var filterUsers = users.Where(u => !gtus.Any(g => (g.User != null) && (g.User.Id == u.Id))).OrderBy(u=>u.FullName);

                        return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(filterUsers);
                    }
                    catch (Exception ex)
                    {
                        ILog logger = log4net.LogManager.GetLogger(typeof(UserReaderService));
                        logger.Error("GetAllUsers Failed.", ex);
                        return null;
                    }
                }
            }
        }

        [OperationContract]
        public IEnumerable<ToUser> GetAllUsersByPrivilege(int privilegeValue)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                
                    try
                    {
                        IEnumerable<User> users = null;

                        users = ws.Repositories.UserRepository.GetAllUsersByPrivilege(privilegeValue);

                        
                        return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(users);
                    }
                    catch (Exception ex)
                    {
                        ILog logger = log4net.LogManager.GetLogger(typeof(UserReaderService));
                        logger.Error("GetAllUsers Failed.", ex);
                        return null;
                    }
                
            }
        }

        [OperationContract]
        public IEnumerable<ToUser> GetAllUsersByGroupList(int[] glist)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        IEnumerable<User> users = null;

                        users = ws.Repositories.UserRepository.GetAllUsersByGroups(glist);

                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(users);
                    }
                    catch (Exception ex) { 
                        tx.Rollback();
                        ILog logger = log4net.LogManager.GetLogger(typeof(UserReaderService));
                        logger.Error("GetAllUsers Failed.", ex);
                        return null; }
                }
            }
        }


        [OperationContract]
        public IEnumerable<ToUser> GetAllUsersByGroup(int groupValue)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        IEnumerable<User> users = null;

                        if (groupValue == 0)
                        {
                            //users = ws.Repositories.UserRepository.GetAllUsers();
                            ToUser currentUser = GetCurrentUser();
                            //three different privileges
                            int[] groupIdList = new int[9];
                            if (currentUser.Groups.Length > 0)
                            {
                                int length = currentUser.Groups.Length;
                                bool manager = false;
                                bool supervisor = false;
                                bool admin = false;
                                for (int i = 0; i < length; i++)
                                {
                                    if (currentUser.Groups[i].Privileges.Length > 0)
                                    {
                                        var len = currentUser.Groups[i].Privileges.Length;
                                        for (var j = 0; j < len; j++)
                                        {
                                            //Distribution Manager
                                            if (currentUser.Groups[i].Privileges[j].Value == 12)
                                            {
                                                manager = true;
                                                //groupIdList[0] = 48;
                                                //groupIdList[1] = 49;
                                                //groupIdList[2] = 50;
                                                //users = ws.Repositories.UserRepository.GetAllUsersByGroups(groupIdList);
                                            }
                                            //Distribution Supervisor
                                            if (currentUser.Groups[i].Privileges[j].Value == 17)
                                            {
                                                supervisor = true;
                                                //groupIdList[0] = 48;
                                                //groupIdList[1] = 49;
                                                //groupIdList[2] = 50;
                                                //groupIdList[3] = 51;
                                                //users = ws.Repositories.UserRepository.GetAllUsersByGroups(groupIdList);
                                            }
                                            //Administrator
                                            if (currentUser.Groups[i].Privileges[j].Value == 18)
                                            {
                                                admin = true;
                                                //users = ws.Repositories.UserRepository.GetAllUsers();
                                            }
                                        }
                                    }
                                }
                                if (admin)
                                {
                                    groupIdList[0] = 46;
                                    groupIdList[1] = 47;
                                    groupIdList[2] = 48;
                                    groupIdList[3] = 49;
                                    groupIdList[4] = 50;
                                    groupIdList[5] = 51;
                                    groupIdList[6] = 52;
                                    groupIdList[7] = 53;
                                    groupIdList[8] = 1;
                                }
                                else if (supervisor)
                                {
                                    groupIdList[0] = 48;
                                    groupIdList[1] = 49;
                                    groupIdList[2] = 50;
                                    groupIdList[3] = 51;
                                    groupIdList[4] = 52;
                                }
                                else if (manager)
                                {
                                    groupIdList[0] = 48;
                                    groupIdList[1] = 49;
                                    groupIdList[2] = 50;
                                    groupIdList[3] = 51;
                                }
                                users = ws.Repositories.UserRepository.GetAllUsersByGroups(groupIdList);
                            }






                        }
                        else
                        {
                            users = ws.Repositories.UserRepository.GetAllUsersByGroup(groupValue);
                        }
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(users);
                    }
                    catch (Exception ex) { 
                        tx.Rollback();
                        ILog logger = log4net.LogManager.GetLogger(typeof(UserReaderService));
                        logger.Error("GetAllUsers Failed.", ex);
                        return null; }
                }
            }
        }

        
        /// <summary>
        /// Return a <see cref="User"/> by its user name.
        /// </summary>
        /// <param name="userName">The user name of the user to be fetched.</param>
        /// <returns>A <see cref="User"/> object matching the specified user name.</returns>
        [OperationContract]
        public ToUser GetUser(string userName)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                try
                {
                    User user = ws.Repositories.UserRepository.GetUser(userName);
                    if (null == user)
                        return null;
                    else
                        return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(user);
                }
                catch (Exception ex)
                {
                    ILog logger = log4net.LogManager.GetLogger(typeof(UserReaderService));
                    logger.Error(string.Format("GetUser Failed by username = '{0}'.", userName), ex);
                    return null;
                }
            }
        }

        [OperationContract]
        public ToUser GetUserById(int userid)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        User user = ws.Repositories.UserRepository.GetUser(userid);
                        tx.Commit();
                        if (null == user)
                            return null;
                        else
                            return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(user);
                    }
                    catch (Exception ex) { 
                        tx.Rollback();
                        ILog logger = log4net.LogManager.GetLogger(typeof(UserReaderService));
                        logger.Error("GetAllUsers Failed.", ex);
                        return null; }
                }
            }
        }

        [OperationContract]
        public ToUser GetCurrentUser()
        {
            try
            {                
                User user = GPS.DomainLayer.Security.LoginMember.CurrentMember;             
                ToUser toUser =  AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(user);
                return toUser;
            }
            catch (Exception ex) {
                ILog logger = log4net.LogManager.GetLogger(typeof(UserReaderService));
                logger.Error("GetAllUsers Failed.", ex);
                return null; }
        }

        [OperationContract]
        public IEnumerable<ToUserRole> UserEnumToList()
        {
            List<ToUserRole> userRoles = new List<ToUserRole>();

            foreach (int i in Enum.GetValues(typeof(GPS.DataLayer.ValueObjects.UserRoles)))
            {
                ToUserRole userRole = new ToUserRole();
                userRole.RoleName = Enum.GetName(typeof(GPS.DataLayer.ValueObjects.UserRoles), i);
                userRole.RoleValue = i;
                userRoles.Add(userRole);
            }

            return userRoles;
        }
    }
}
