using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DataLayer;
using GPS.Website.TransferObjects;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;
using System.Collections.Generic;
using log4net;



namespace GPS.Website.UserServices {
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ServiceContract(Namespace = "TIMM.Website.UserServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserWriterService {

        private static ILog m_Logger = LogManager.GetLogger(typeof(UserWriterService));

        /// <summary>
        /// Add a user to the system.
        /// </summary>
        /// <param name="user">A <see cref="User"/> object represents the user 
        /// being added.</param>
        /// <returns>The user just added successfully.</returns>
        [OperationContract]
        public ToUser AddUser(ToUser touser, int[] groupIds)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        User u = AssemblerConfig.GetAssembler<User, ToUser>().AssembleFrom(touser);

                        if (groupIds != null)
                        {
                            IList<Group> groupList = ws.Repositories.GroupRepository.GetGroupList(groupIds);
                            if (u.Groups == null)
                                u.Groups = new List<Group>();
                            foreach (Group g in groupList)
                            {
                                u.Groups.Add(g);
                            }
                        }
                        User user = ws.Repositories.UserRepository.AddUser(u);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(user);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Unhandle Error", ex);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Delete a user specified by user name.
        /// </summary>
        /// <param name="userName">The user name of the user to be deleted.</param>
        [OperationContract]
        public void DeleteUser(string userName) {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        ws.Repositories.UserRepository.DeleteUser(userName);
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        m_Logger.Error("WCF Unhandle Error", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">user object</param>
        /// <returns>updated user (GPSUser object) </returns>
        [OperationContract]
        public ToUser UpdateUser(ToUser touser, int[] groupIds)
        {
            DataLayer.UserRepository userRep = new DataLayer.UserRepository();
            DomainLayer.Entities.User ug = userRep.GetUser(touser.Id);
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        User u = AssemblerConfig.GetAssembler<User, ToUser>().AssembleFrom(touser);

                        IList<Group> groupList = ws.Repositories.GroupRepository.GetGroupList(groupIds);

                        if (u.Groups == null)
                            u.Groups = new List<Group>();
                        else
                            u.Groups.Clear();

                        foreach (Group g in groupList)
                        {
                            u.Groups.Add(g);
                        }

                        User user = ws.Repositories.UserRepository.UpdateUser(u);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(user);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        m_Logger.Error("WCF Unhandle Error", ex);
                        return null;
                        
                    }
                }
            }
        }
               

        [OperationContract]
        public void RemoveUserFromGroups(int[] GroupNames, int userId, int status)
        {
            int i = 0;
            DataLayer.UserRepository userRep = new DataLayer.UserRepository();
            DomainLayer.Entities.User user = userRep.GetUser(userId);
            while (i < GroupNames.Count())
            {
                DataLayer.GroupRepository groupRep = new DataLayer.GroupRepository();
                DomainLayer.Entities.Group group = groupRep.GetGroup(GroupNames[i]);
                if (group != null)
                {
                    group.Users.Clear();
                    group.Users.Add(user);
                }
                i++;
            }
        }

    }
}

