using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DataLayer;
using GPS.Website.AppFacilities;
using GPS.Website.TransferObjects;
using GPS.DomainLayer.Entities;
using System.Collections.Generic;
using GPS.Utilities;

namespace GPS.Website.TaskServices
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ServiceContract(Namespace = "TIMM.Website.TaskServices")]    
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TaskReaderService
    {
        [OperationContract]
        public IEnumerable<ToAddress> GetAddressByTaskId(int tid) {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {

                List<ToAddress> list = new List<ToAddress>();
                try
                {
                    IList<Address> aList = ws.Repositories.TaskRepository.GetAddressByTaskId(tid);
                    foreach (Address a in aList)
                    {
                        ToAddress toAddress = AssemblerConfig.GetAssembler<ToAddress, Address>().AssembleFrom(a);
                        list.Add(toAddress);
                    }
                    return list;
                }
                catch (Exception ex) {
                    GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                    return null; }

            }
        }

        [OperationContract]
        public IEnumerable<ToMonitorAddresses> GetMonitorAddressByTaskId(int tid)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {

                List<ToMonitorAddresses> list = new List<ToMonitorAddresses>();
                try
                {
                    Task task = ws.Repositories.TaskRepository.GetTask(tid);
                    IList<MonitorAddresses> aList = ws.Repositories.TaskRepository.GetMonitorAddressByDMId(task.DmId);
                    foreach (MonitorAddresses a in aList)
                    {
                        ToMonitorAddresses toAddress = AssemblerConfig.GetAssembler<ToMonitorAddresses, MonitorAddresses>().AssembleFrom(a);
                        list.Add(toAddress);
                    }
                    return list;
                }
                catch (Exception ex) {
                    GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                    return null; }

            }
        }

        [OperationContract]
        public IEnumerable<ToTask> GetAllTasks()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {

                List<ToTask> list = new List<ToTask>();
                try
                {
                    IList<Task> taskList = ws.Repositories.TaskRepository.GetAllTasks();
                    foreach (Task t in taskList)
                    {
                        if (t.Taskgtuinfomappings != null)
                        {
                            foreach (Taskgtuinfomapping m in t.Taskgtuinfomappings)
                            {
                                m.Gtuinfos = null;
                            }
                        }
                        ToTask toTask = AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(t);
                        list.Add(toTask);
                    }
                    return list;
                }
                catch (Exception ex) {
                    GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                    return null; }

            }
        }

        [OperationContract]
        public IEnumerable<ToTask> GetAllReports()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {

                List<ToTask> list = new List<ToTask>();
                try
                {
                    IList<Task> taskList = ws.Repositories.TaskRepository.GetAllReports();
                    foreach (Task t in taskList)
                    {
                        if (t.Taskgtuinfomappings != null)
                        {
                            foreach (Taskgtuinfomapping m in t.Taskgtuinfomappings)
                            {
                                m.Gtuinfos = null;
                            }
                        }
                        ToTask toTask = AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(t);
                        list.Add(toTask);
                    }
                    return list;
                }
                catch (Exception  ex) {
                    LogUtils.Error("WCF Unhandle Error", ex);
                    DBLog.LogError(ex.ToString());
                    return null; 
                }

            }
        }


        [OperationContract]
        public ToTask GetTask(int taskID)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                //using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                //{
                    try
                    {
                        Task task = ws.Repositories.TaskRepository.GetTask(taskID);
                        //tx.Commit();
                        //if (null == task)
                        //    return null;
                        //else
                        return AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);
                    }
                    catch (Exception ex) { 
                        //tx.Rollback(); 
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        return null; 
                    }
               // }
            }
        }

        [OperationContract]
        public IList<ToTask> GetTaskGtuMappingsByUser(int userid) { 
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                    try
                    {
                        IList<Task> tasks = ws.Repositories.TaskRepository.GetTaskGtuMappingsByUser(userid);
                        IList<ToTask> list = new List<ToTask>();
                        foreach (Task t in tasks)
                        {
                            list.Add(AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(t));
                        }
                        return list;
                    }
                    catch (Exception ex) {
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        return null; 
                    }
            }
        
        }

        [OperationContract]
        public int GetStartOrStopByTaskId(int[] tids) {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                int timeType = 0;
                try
                {
                    foreach (int tid in tids) {
                        timeType = ws.Repositories.TaskRepository.GetStartOrStopByTaskId(tid);
                        if (timeType==0)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                }
                return timeType;
            }
            
        }

        [OperationContract]
        public IList<ToTask> GetTasks(int[] taskIDs)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        IList<Task> tasks = ws.Repositories.TaskRepository.GetTasks(taskIDs);
                        tx.Commit();
                        if (null == tasks)
                            return null;
                        else
                        {
                            IList<ToTask> list = new List<ToTask>();
                            foreach (Task t in tasks)
                            {
                                list.Add(AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(t));
                            }
                            return list;
                        }
                    }
                    catch (Exception ex) {
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        tx.Rollback(); return null; }
                }
            }
        }

        [OperationContract]
        public bool IsTaskDateDuplicate(int[] taskIDs)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        IList<Task> tasks = ws.Repositories.TaskRepository.GetTasks(taskIDs);
                        tx.Commit();
                        if (null == tasks)
                            return false;
                        else
                        {
                            List<DateTime> dateList = new List<DateTime>();
                            foreach (Task t in tasks)
                            {
                                if (dateList.Contains(t.Date))
                                {
                                    return true;
                                }
                                dateList.Add(t.Date);
                            }
                        }
                        return false;
                    }
                    catch (Exception ex) {
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        tx.Rollback(); return false; }
                }
            }
        }
    }
}
