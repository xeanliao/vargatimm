using System;
using System.Collections.Generic;
using GPS.DomainLayer.Entities;
namespace GPS.DataLayer
{
    public interface ITaskRepository
    {
        Task AddTask(Task task);
        Task UpdateTask(Task task);
        Task UpdateTaskWithChildren(Task Task);
        Task UpdateTaskWithChildrens(Task Task);
        void DeleteTask(int id);
        void MarkFinish(int id);
        IList<Task> GetAllTasks();
        IList<Task> GetTasks(int[] ids);
        IList<Task> GetTasks(DateTime date);
        Task GetTask(int id);
        IList<Task> GetTaskGtuMappingsByUser(int uid);
        IEnumerable<Task> GetTasksByGtu(Gtu gtu);
        IList<Task> GetAllReports();
        int GetStartOrStopByTaskId(int tid);
        IList<string> GetUserColorByTaskId(int tid);
        IList<Address> GetAddressByTaskId(int tid);
        IList<MonitorAddresses> GetMonitorAddressByDMId(int Did);
        IDictionary<int, List<int>> GetCampTaskMapping(int[] taskids);
    }
}