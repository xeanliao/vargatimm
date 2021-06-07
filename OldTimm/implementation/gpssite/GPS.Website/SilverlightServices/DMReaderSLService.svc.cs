using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DataLayer;
using System.Collections.Generic;
using GPS.Website.TransferObjects;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Area;
using GPS.DataLayer.RepositoryImplementations;
using GPS.Utilities;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using CommonUtils;

namespace GPS.Website.SilverlightServices
{
    public class ZipHelper
    {
        //public static string Serialize<T>(T obj)
        //{
        //    System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
        //    MemoryStream ms = new MemoryStream();
        //    serializer.WriteObject(ms, obj);
        //    string retVal = Encoding.Default.GetString(ms.ToArray());
        //    ms.Dispose();
        //    return retVal;
        //}
        public static byte[] Compress<T>(T obj)
        {
            var serializer = new System.Runtime.Serialization.DataContractSerializer(obj.GetType());
            // System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            byte[] data = ms.ToArray();
            ms.Dispose();

            byte[] zipData;

            using (var outputStream = new MemoryStream())
            {
                using (var zip = new ZipOutputStream(outputStream))
                {
                    zip.IsStreamOwner = true;

                    zip.SetLevel(9); // high level

                    var zipEntry = new ZipEntry("content.dat")
                    {
                        DateTime = DateTime.Now,
                        Size = data.Length
                    };

                    zip.PutNextEntry(zipEntry);
                    zip.Write(data, 0, data.Length);

                    zip.Finish();
                }

                zipData = outputStream.ToArray();
                outputStream.Close();
            }


            return zipData;
        }

        //public static T Deserialize<T>(string json)
        //{
        //    T obj = Activator.CreateInstance<T>();
        //    MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
        //    System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
        //    obj = (T)serializer.ReadObject(ms);
        //    ms.Close();
        //    ms.Dispose();
        //    return obj;
        //}

        public static T Uncompress<T>(byte[] data)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(data);

            var unzipStream = new MemoryStream();

            using (var zip = new ZipInputStream(ms))
            {
                ZipEntry zipEntity;

                while ((zipEntity = zip.GetNextEntry()) != null)
                {
                    if (!zipEntity.IsFile) continue;

                    int size = 2048;
                    var buffer = new byte[2048];
                    while (true)
                    {
                        size = zip.Read(buffer, 0, buffer.Length);
                        if (size > 0)
                        {
                            unzipStream.Write(buffer, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            var serializer = new System.Runtime.Serialization.DataContractSerializer(obj.GetType());
            // System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            unzipStream.Position = 0;

            obj = (T)serializer.ReadObject(unzipStream);
            ms.Close();
            ms.Dispose();
            return obj;
        }
    }

    [ServiceContract(Namespace = "TIMM.Website.SilverlightServices")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DMReaderSLService 
    {
        [OperationContract]
        public byte[] GetDistributionMapsById(int id)
        {
            try
            {
                DistributionMapRepository dr = new DistributionMapRepository();
                GPS.DomainLayer.Entities.DistributionMap dmap = dr.GetEntity(id);

                ToDistributionMap todmap = AssemblerConfig.GetAssembler<ToDistributionMap, GPS.DomainLayer.Entities.DistributionMap>().AssembleFrom(dmap);
                byte[] ret = ZipHelper.Compress<ToDistributionMap>(todmap);
                return ret; 
                //return CompressedSerializer.Compress<ToDistributionMap> (todmap);
            }
            catch (Exception ex)
            {
                LogUtils.Error("WCF Unhandle Error", ex);
                return null;
            }
        }

        [OperationContract]
        public ToDistributionMap GetDistributionMapsObjectById(int id)
        {
            try
            {
                DistributionMapRepository dr = new DistributionMapRepository();
                GPS.DomainLayer.Entities.DistributionMap dmap = dr.GetEntity(id);

                return AssemblerConfig.GetAssembler<ToDistributionMap, GPS.DomainLayer.Entities.DistributionMap>().AssembleFrom(dmap);
            }
            catch (Exception ex)
            {
                LogUtils.Error("WCF Unhandle Error", ex);
                return null;
            }
        }

        [OperationContract]
        public byte[] GetCustomAreaByBox()
        {
            try
            {
                List<ToArea> areas = new List<ToArea>();
                CustomAreaRepository repository = new CustomAreaRepository();

                List<CustomArea> customAreas = repository.GetCustomAreas();
                foreach (CustomArea customArea in customAreas)
                {
                    MapArea area = new MapArea(customArea);
                    ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(area);

                    areas.Add(toarea);
                }

                NdAddressRepository rs = new NdAddressRepository();

                IList<NdAddress> ndAddresses = rs.GetNdAddresses();
                foreach (NdAddress nda in ndAddresses)
                {
                    MapArea area = new MapArea(nda);
                    ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(area);

                    areas.Add(toarea);
                }

                return CompressedSerializer.Compress<List<ToArea>>(areas);
            }
            catch (Exception ex)
            {
                LogUtils.Error("WCF Unhandle Error", ex);
                return null;
            }
        }

        [OperationContract]
        public List<ToArea> GetCustomAreaObjectByBox()
        {
            try
            {
                List<ToArea> areas = new List<ToArea>();
                CustomAreaRepository repository = new CustomAreaRepository();

                List<CustomArea> customAreas = repository.GetCustomAreas();
                foreach (CustomArea customArea in customAreas)
                {
                    MapArea area = new MapArea(customArea);
                    ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(area);

                    areas.Add(toarea);
                }

                NdAddressRepository rs = new NdAddressRepository();

                IList<NdAddress> ndAddresses = rs.GetNdAddresses();
                foreach (NdAddress nda in ndAddresses)
                {
                    MapArea area = new MapArea(nda);
                    ToArea toarea = AssemblerConfig.GetAssembler<ToArea, MapArea>().AssembleFrom(area);

                    areas.Add(toarea);
                }

                return areas;
            }
            catch (Exception ex)
            {
                LogUtils.Error("WCF Unhandle Error", ex);
                return null;
            }
        }

        //[OperationContract]
        //public List<ccGTURecord> GetCurrentGTUIDList(double maxlat, double maxlon, double minlat, double minlon)
        //{
        //    FakeGTU fgtu = new FakeGTU();
        //    int gtuCount = 0;
        //    System.Random r = new Random();
        //    gtuCount = r.Next(5, 21);
        //    return fgtu.makeGTUList(gtuCount, maxlat, maxlon, minlat, minlon);
        //}

        //[OperationContract]
        //public List<ccGTURecord> GetCurrentGTUs(List<ccGTURecord> gids)
        //{
        //    FakeGTU fgtu = new FakeGTU();
        //    return fgtu.MakeGTUs(gids);
        //}

        [OperationContract]
        public byte[] GetTask(int Id)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                    try
                    {
                        Task task = ws.Repositories.TaskRepository.GetTask(Id);
                        //ToTask totask = AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);
                        ////AssemblerConfig.GetAssembler<ToTaskgtuinfomapping, Taskgtuinfomapping>().AssembleFrom(task.Taskgtuinfomappings);
                        
                        //AssemblerConfig.GetAssembler<Gtu, ToGtu>().AssembleFrom(totask.Taskgtuinfomappings[0].GTU);
                        ////AssemblerConfig.GetAssembler<Gtuinfo, ToGtuinfo>().AssembleFrom(totask.Taskgtuinfomappings[0].Gtuinfos);
                        //AssemblerConfig.GetAssembler<Taskgtuinfomapping, ToTaskgtuinfomapping>().AssembleFrom(totask.Taskgtuinfomappings[0]);
                        //AssemblerConfig.GetAssembler<Task, ToTask>().AssembleFrom(totask);
                        ToTask tt =  AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);

                        byte[] ret = ZipHelper.Compress<ToTask>(tt);

                        return ret;
                    }
                    catch (Exception ex) {
                        LogUtils.Error("WCF Unhandle Error", ex);
                        return null; 
                    }
            }
        }

        [OperationContract]
        public ToTask GetTaskObject(int Id)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                try
                {
                    Task task = ws.Repositories.TaskRepository.GetTask(Id);
                    //ToTask totask = AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);
                    ////AssemblerConfig.GetAssembler<ToTaskgtuinfomapping, Taskgtuinfomapping>().AssembleFrom(task.Taskgtuinfomappings);

                    //AssemblerConfig.GetAssembler<Gtu, ToGtu>().AssembleFrom(totask.Taskgtuinfomappings[0].GTU);
                    ////AssemblerConfig.GetAssembler<Gtuinfo, ToGtuinfo>().AssembleFrom(totask.Taskgtuinfomappings[0].Gtuinfos);
                    //AssemblerConfig.GetAssembler<Taskgtuinfomapping, ToTaskgtuinfomapping>().AssembleFrom(totask.Taskgtuinfomappings[0]);
                    //AssemblerConfig.GetAssembler<Task, ToTask>().AssembleFrom(totask);
                    return AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);
                }
                catch (Exception ex)
                {
                    LogUtils.Error("WCF Unhandle Error", ex);
                    return null;
                }
            }
        }

        [OperationContract]
        public IDictionary<string,string> GetGtuUserNameDic(int Id)
        {
            IDictionary<string, string> gtuUserNameDic = new Dictionary<string, string>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                try
                {
                    Task task = ws.Repositories.TaskRepository.GetTask(Id);
                    ToTask toTask = AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);
                    if (toTask == null) return null;
                    if (toTask.Taskgtuinfomappings == null) return null;
                    if (toTask.Taskgtuinfomappings.Count() == 0) return null;
                    foreach (ToTaskgtuinfomapping tgm in toTask.Taskgtuinfomappings)
                    {
                        string gid = tgm.GTU.UniqueID;
                        string userName = getUserNameByUserId(tgm.UserId);
                        if(userName!=string.Empty)
                            gtuUserNameDic.Add(gid, userName);
                    }
                    return gtuUserNameDic;

                }
                catch (Exception ex)
                {
                    LogUtils.Error("WCF Unhandle Error", ex);
                    return null;
                }
            }
        }

        [OperationContract]
        public IEnumerable<ToTaskgtuinfomapping> GetGtuInfoListByTaskId(int tid)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Task task = ws.Repositories.TaskRepository.GetTask(tid);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToTaskgtuinfomapping, Taskgtuinfomapping>().AssembleFrom(task.Taskgtuinfomappings);
                    }
                    catch (Exception ex) { 
                        tx.Rollback();
                        LogUtils.Error("WCF Unhandle Error", ex); 
                        return null; 
                    }
                }
            }
        }

        [OperationContract]
        public IEnumerable<ToMonitorAddresses> GetMAddressListByTaskId(int tid)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Task task = ws.Repositories.TaskRepository.GetTask(tid);
                        int did = task.DmId;
                        IList<MonitorAddresses> maList = ws.Repositories.TaskRepository.GetMonitorAddressByDMId(did);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToMonitorAddresses, MonitorAddresses>().AssembleFrom(maList);
                    }
                    catch (Exception ex) { 
                        tx.Rollback();
                        LogUtils.Error("WCF Unhandle Error", ex); 
                        return null;
                    }
                }
            }
        }

        [OperationContract]
        public IEnumerable<ToMonitorAddresses> GetMAddressCOListByTaskId(int tid)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Task task = ws.Repositories.TaskRepository.GetTask(tid);
                        int did = task.DmId;
                        IList<MonitorAddresses> maList = ws.Repositories.TaskRepository.GetMonitorAddressByDMId(did);
                        tx.Commit();
                        
                        return AssemblerConfig.GetAssembler<ToMonitorAddresses, MonitorAddresses>().AssembleFrom(maList);
                    }
                    catch (Exception ex) { 
                        tx.Rollback();
                        LogUtils.Error("WCF Unhandle Error", ex); 
                        return null;
                    }
                }
            }
        }

        [OperationContract]
        public ToTask AddTaskTime(int taskId, int timeType)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Task returnTask = new Task();
                        Task task = ws.Repositories.TaskRepository.GetTask(taskId);
                        TaskTime taskTime = new TaskTime();
                        taskTime.TaskId = task.Id;
                        taskTime.Time = DateTime.Now;
                        taskTime.TimeType = timeType;
                        task.Tasktimes.Add(taskTime);
                        returnTask = ws.Repositories.TaskRepository.UpdateTaskWithChildren(task);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(returnTask);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        LogUtils.Error("WCF Unhandle Error", ex);
                        throw ex;
                    }
                }
            }
        }

        [OperationContract]
        public ToTask UpdateTaskObject(ToTask toTask)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        //AssemblerConfig.GetAssembler<Taskgtuinfomapping, ToTaskgtuinfomapping>().AssembleFrom(toTask.Taskgtuinfomappings);
                        Task returnTask = new Task();
                        Task task = AssemblerConfig.GetAssembler<Task, ToTask>().AssembleFrom(toTask);
                        //task.Taskgtuinfomappings.ToList().ForEach(tm=>tm.Gtuinfos.Clear());
                        ws.Repositories.TaskRepository.UpdateTaskWithChildrens(task);
                        tx.Commit();
                        returnTask = ws.Repositories.TaskRepository.GetTask(task.Id);
                        return AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(returnTask);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        LogUtils.Error("WCF Unhandle Error", ex);
                        throw ex;
                    }
                }
            }
        }

        [OperationContract]
        public byte[] UpdateTask(byte[] toTask)
        {
            LogUtils.Debug("DMReaderSLService::UpdateTask"); 

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        //AssemblerConfig.GetAssembler<Taskgtuinfomapping, ToTaskgtuinfomapping>().AssembleFrom(toTask.Taskgtuinfomappings);
                        Task returnTask = new Task();
                        ToTask toTaskDeserialized = ZipHelper.Uncompress<ToTask>(toTask); 
                        // ToTask toTaskDeserialized = CompressedSerializer.Decompress<ToTask>(toTask);
                        foreach (ToTaskgtuinfomapping tgim in toTaskDeserialized.Taskgtuinfomappings)
                        {
                            foreach (ToGtuinfo tgi in tgim.Gtuinfos)
                            {
                                if (tgi.Code == null)
                                {
                                    tgi.Code = "";
                                }
                                if (tgi.sVersion == null)
                                {
                                    tgi.sVersion = "";
                                }
                                if (tgi.sIPAddress == null)
                                {
                                    tgi.sIPAddress = "";
                                }
                            }
                        }
                        Task task = AssemblerConfig.GetAssembler<Task, ToTask>().AssembleFrom(toTaskDeserialized);
                        //task.Taskgtuinfomappings.ToList().ForEach(tm=>tm.Gtuinfos.Clear());
                        ws.Repositories.TaskRepository.UpdateTaskWithChildrens(task);
                        tx.Commit();
                        returnTask = ws.Repositories.TaskRepository.GetTask(task.Id);
                        return ZipHelper.Compress<ToTask>(AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(returnTask));
                        // return CompressedSerializer.Compress<ToTask>(AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(returnTask));
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        LogUtils.Error("WCF Unhandle Error", ex);
                        throw ex;
                    }
                }
            }
        
        }

        [OperationContract]
        public string getUserNameByUserId(int uid)
        {
            IList<string> users = new List<string>();
            UserRepository ur = new UserRepository();
            users= ur.GetUserNameById(uid);
            if (users.Count > 0)
                return users[0];
            else
                return string.Empty;
        }

        [OperationContract]
        public string loadHistoryDotToLayerThread(string gid)
        { return gid; }

        [OperationContract]
        public List<object> PointInPolygon(double[][] coordinates, double[] point, List<object> gti)
        {
            bool isIn = ShapeMethods.PointInPolygon(coordinates, point);
            if (!isIn)
                return gti;
            return null;
        }
    }
}
