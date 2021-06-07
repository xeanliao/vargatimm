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
using log4net;


namespace GPS.Website.GroupServices
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ServiceContract(Namespace = "TIMM.Website.GroupServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GroupReaderService
    {
        [OperationContract]
        public IEnumerable<ToGroup> GetAllGroups()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                
                ToUser currentUser = new ToUser();
                try
                {
                    User user = GPS.DomainLayer.Security.LoginMember.CurrentMember;
                    currentUser = AssemblerConfig.GetAssembler<ToUser, User>().AssembleFrom(user);
                }
                catch (Exception ex)
                {
                    ILog logger = LogManager.GetLogger(GetType());
                    logger.Error("WCF Unhandle Error", ex); 
                    return null;
                }
                //three different privileges
                //string groupValue = "DistributionManager";
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
                                    //groupValue = "DistributionManager";
                                    manager = true;
                                    
                                }
                                //Distribution Supervisor
                                if (currentUser.Groups[i].Privileges[j].Value == 17)
                                {
                                    //groupValue = "DistributionSupervisor";
                                    supervisor = true;
                                    
                                }
                                //Administrator
                                if (currentUser.Groups[i].Privileges[j].Value == 18)
                                {
                                    //groupValue = "Administrator";
                                    admin = true;
                                   
                                }
                            }
                        }
                    }
                    
                    if (admin) {
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
                }
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        //IEnumerable<Group> groups = ws.Repositories.GroupRepository.GetAllGroups();

                        IEnumerable<Group> groups = ws.Repositories.GroupRepository.GetGroupList(groupIdList);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToGroup, Group>().AssembleFrom(groups);
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
        /// Return a <see cref="User"/> by its group Id.
        /// </summary>
        /// <param name="userName">The Id of the group to be fetched.</param>
        /// <returns>A <see cref="User"/> object matching the specified group Id.</returns>
        [OperationContract]
        public ToGroup GetGroup(int groupID)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Group group = ws.Repositories.GroupRepository.GetGroup(groupID);
                        tx.Commit();
                        if (null == group)
                            return null;
                        else
                            return AssemblerConfig.GetAssembler<ToGroup, Group>().AssembleFrom(group);
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

        [OperationContract]
        public ToGroup GetGroupForValidate(ToGroup togroup)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Group g = AssemblerConfig.GetAssembler<Group, ToGroup>().AssembleFrom(togroup);
                        Group group = ws.Repositories.GroupRepository.GetGroupForValidate(g);
                        tx.Commit();
                        if (null == group)
                            return null;
                        else
                            return AssemblerConfig.GetAssembler<ToGroup, Group>().AssembleFrom(group);
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

        [OperationContract]
        public IEnumerable<ToPrivilege> GetAllPrivileges()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        IEnumerable<Privilege> privileges = ws.Repositories.PrivilegeRepository.GetAllPrivileges();
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToPrivilege, Privilege>().AssembleFrom(privileges);
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

    }

    //[ServiceContract(Namespace = "TIMM.Website.GroupServices")]
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    //public class PrivilegeReaderService
    //{
    //    [OperationContract]
    //    public IEnumerable<ToPrivilege> GetAllPrivileges()
    //    {
    //        using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
    //        {
    //            using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
    //            {
    //                try
    //                {
    //                    IEnumerable<Privilege> privileges = ws.Repositories.PrivilegeRepository.GetAllPrivileges();
    //                    tx.Commit();
    //                    return AssemblerConfig.GetAssembler<ToPrivilege, Privilege>().AssembleFrom(privileges);
    //                }
    //                catch (Exception) { tx.Rollback(); return null; }
    //            }
    //        }
    //    }

        /// <summary>
        /// Return a <see cref="User"/> by its privilege Id.
        /// </summary>
        /// <param name="userName">The Id of the privilege to be fetched.</param>
        /// <returns>A <see cref="User"/> object matching the specified privilege Id.</returns>
        //[OperationContract]
        //public ToPrivilege GetPrivilege(int privilegeID)
        //{
        //    using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
        //    {
        //        using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
        //        {
        //            try
        //            {
        //                Privilege privilege = ws.Repositories.PrivilegeRepository.GetPrivilege(privilegeID);
        //                tx.Commit();
        //                if (null == privilege)
        //                    return null;
        //                else
        //                    return AssemblerConfig.GetAssembler<ToPrivilege, Privilege>().AssembleFrom(privilege);
        //            }
        //            catch (Exception) { tx.Rollback(); return null; }
        //        }
        //    }
        //}

    //}
}
