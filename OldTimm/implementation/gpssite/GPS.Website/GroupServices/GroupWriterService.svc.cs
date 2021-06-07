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


namespace GPS.Website.GroupServices
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ServiceContract(Namespace = "TIMM.Website.GroupServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GroupWriterService
    {
        /// <summary>
        /// Add a group to the system.
        /// </summary>
        /// <param name="group">A <see cref="Group"/> object represents the group 
        /// being added.</param>
        /// <returns>The group just added successfully.</returns>
        [OperationContract]
        public ToGroup AddGroup(ToGroup togroup, int[] privilegeIds)
        {
            DataLayer.GroupRepository groupRep = new DataLayer.GroupRepository();
            DomainLayer.Entities.Group gp = groupRep.GetGroup(togroup.Id);
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Group u = AssemblerConfig.GetAssembler<Group, ToGroup>().AssembleFrom(togroup);
                        if (privilegeIds != null)
                        {
                            IList<Privilege> privilegeList = ws.Repositories.PrivilegeRepository.GetPrivilegeList(privilegeIds);
                            if (u.Privileges == null)
                                u.Privileges = new List<Privilege>();
                            foreach (Privilege g in privilegeList)
                            {
                                u.Privileges.Add(g);
                            }
                        }
                        Group group = ws.Repositories.GroupRepository.AddGroup(u);
                        tx.Commit();
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

        /// <summary>
        /// Delete a group specified by group name.
        /// </summary>
        /// <param name="groupName">The group name of the group to be deleted.</param>
        [OperationContract]
        public void DeleteGroup(int groupID)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        ws.Repositories.GroupRepository.DeleteGroup(groupID);
                        tx.Commit();
                    }
                    catch (Exception ex) { tx.Rollback();
                    ILog logger = log4net.LogManager.GetLogger(typeof(GroupWriterService));
                    logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Update a group
        /// </summary>
        /// <param name="group">group object</param>
        /// <returns>updated group (GPSGroup object) </returns>
        [OperationContract]
        public ToGroup UpdateGroup(ToGroup togroup, int[] privilegeIds)
        {
            DataLayer.GroupRepository groupRep = new DataLayer.GroupRepository();
            DomainLayer.Entities.Group gp = groupRep.GetGroup(togroup.Id);
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Group g = AssemblerConfig.GetAssembler<Group, ToGroup>().AssembleFrom(togroup);

                        IList<Privilege> privilegeList = ws.Repositories.PrivilegeRepository.GetPrivilegeList(privilegeIds);

                        if (g.Privileges == null)
                            g.Privileges = new List<Privilege>();
                        else
                            g.Privileges.Clear();

                        foreach (Privilege p in privilegeList)
                        {
                            g.Privileges.Add(p);
                        }

                        Group group = ws.Repositories.GroupRepository.UpdateGroup(g);
                        tx.Commit();
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
    }
}

