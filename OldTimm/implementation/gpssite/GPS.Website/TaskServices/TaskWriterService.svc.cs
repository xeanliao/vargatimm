using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using GPS.DataLayer;
using GPS.Website.TransferObjects;
using GPS.Website.AppFacilities;
using System.Collections.Generic;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.DataInfrastructure;
using System.Drawing;
using System.Web;
using System.Drawing.Imaging;
using GPS.DataLayer.RepositoryImplementations;
using log4net;

namespace GPS.Website.TaskServices
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ServiceContract(Namespace = "TIMM.Website.TaskServices")] 
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TaskWriterService
    {
        [OperationContract]
        public ToTask AddTask(ToTask toTask)
        {            
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Task task = AssemblerConfig.GetAssembler<Task, ToTask>().AssembleFrom(toTask);
                        //task.Date = DateTime.Parse(toTask.Date);
                        task = ws.Repositories.TaskRepository.AddTask(task);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Service Unhandle Error", ex);
                        return null;
                    }
                }
            }
        }

        [OperationContract]
        public void DismissTasks(Int32[] taskIds,int userId)
        {
            int status = 1;
            IDictionary<int, List<int>> CampTaskDct = new Dictionary<int, List<int>>();
            DataLayer.UserRepository userRep = new DataLayer.UserRepository();
            DomainLayer.Entities.User user = userRep.GetUser(userId);
            TaskRepository taskRep = new TaskRepository();
            CampTaskDct=taskRep.GetCampTaskMapping(taskIds);
            CampaignRepository campaignRep = new CampaignRepository();
            foreach (int campaign in CampTaskDct.Keys)
            {
                StatusInfo si = new StatusInfo(status);
                DomainLayer.Entities.Campaign cam = campaignRep.GetEntity(campaign);
                if (CampTaskDct[campaign].Count > 0)
                {
                    DeleteTasks(CampTaskDct[campaign].ToArray());
                    cam.Users.Clear();
                    cam.Users.Add(user, si);
                    campaignRep.Update(cam); 
                    
                }
            }
        } 

    
        [OperationContract]
        public void DeleteTask(int TaskID)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        ws.Repositories.TaskRepository.DeleteTask(TaskID);
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback(); 
                        ILog logger = log4net.LogManager.GetLogger(typeof(TaskWriterService));
                        logger.Error("WCF Unhandl Error", ex);
                    }
                }
            }
        }

        [OperationContract]
        public void DeleteTasks(Int32[] taskIds)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                foreach (Int32 id in taskIds)
                {
                    using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                    {
                        try
                        {
                            ws.Repositories.TaskRepository.DeleteTask(id);
                            tx.Commit();
                        }
                        catch (Exception ex) { 
                            tx.Rollback();
                        ILog logger = log4net.LogManager.GetLogger(typeof(TaskWriterService));
                        logger.Error("WCF Unhandl Error", ex);
                        }
                    }
                }
            }
        }

        [OperationContract]
        public void MarkFinish(Int32[] taskIds)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                foreach (Int32 id in taskIds)
                {
                    using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                    {
                        try
                        {
                            ws.Repositories.TaskRepository.MarkFinish(id);
                            tx.Commit();
                        }
                        catch (Exception ex)
                        {
                            tx.Rollback(); ILog logger = log4net.LogManager.GetLogger(typeof(TaskWriterService));
                            logger.Error("WCF Unhandl Error", ex);
                        }
                    }
                }
            } 
        }
       
        [OperationContract]
        public ToTask UpdateTask(ToTask toTask)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Task task = ws.Repositories.TaskRepository.GetTask(toTask.Id);
                        task.AuditorId = toTask.AuditorId;
                        task.Date = DateTime.Parse(toTask.Date);
                        task.Email = toTask.Email;
                        task.Telephone = toTask.Telephone;
                        task = ws.Repositories.TaskRepository.UpdateTask(task);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToTask, Task>().AssembleFrom(task);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ILog logger = LogManager.GetLogger(GetType());
                        logger.Error("WCF Service Unhandle Error", ex);
                        return null;
                    }
                }
            }
        }

        private byte GetColorRamdom(int colmin, int colmax)
        {
            System.Random r = new Random(GetRandomSeed());
            return Convert.ToByte(r.Next(colmin, colmax));
        }

        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private Color GetRandomColor()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            int seed =  BitConverter.ToInt32(bytes, 0);

            byte r = GetColorRamdom(80, 256);
            byte g = GetColorRamdom(80, 256);
            byte b = GetColorRamdom(80, 256);
            Color color = Color.FromArgb(255, r, g, b);
            return color;
            //return 
        }

        [OperationContract]
        public string IsValidSelectedGTUs(int[] taskIds, int[] gtusId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                //remove restriction for 308                   
                //if (gtusId == null) gtusId = new int[0];
                //IList<Task> taskList = ws.Repositories.TaskRepository.GetTasks(taskIds);

                //foreach (Task task in taskList)
                //{
                //    List<Taskgtuinfomapping> mappingList = new List<Taskgtuinfomapping>();
                //    IList<Task> list = ws.Repositories.TaskRepository.GetTasks(task.Date);
                //    foreach (Task t in list)
                //    {
                //        if (t.Taskgtuinfomappings != null && t.Id!=task.Id)
                //        {
                //            mappingList.AddRange(t.Taskgtuinfomappings);
                //        }
                //    }

                    
                //    for(int i=0;i<gtusId.Length;i++)
                //    {
                //        int index = mappingList.FindIndex(m => m.GTU.Id == gtusId[i]);
                //        if (index > -1)
                //        {
                //            //Gtu g = ws.Repositories.GtuRepository.GetGtu(gtusId[i]);
                //            Gtu g = mappingList[index].GTU;
                //            Task t = ws.Repositories.TaskRepository.GetTask(mappingList[index].TaskId);
                //            string uid = g.UniqueID;
                //            if (uid.Length > 6)
                //                uid = g.UniqueID.Substring(g.UniqueID.Length - 6, 6);
                //            return "GTU:'" + uid + "' has been already assigned to Task:'" + t.Name + "' on " + t.Date.ToShortDateString() + "!";
                //        }
                //    }
                //}
                return "";
            }
        }

        [OperationContract]
        public void UpdateTasksWithGTUs(int[] taskIds, int[] gtusId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        if (gtusId == null) gtusId = new int[0];
                        IList<Task> taskList = ws.Repositories.TaskRepository.GetTasks(taskIds);
                        
                        foreach (Task task in taskList)
                        {
                            if (task.Taskgtuinfomappings == null)
                            {
                                task.Taskgtuinfomappings = new List<Taskgtuinfomapping>();
                            }
                            //timm 307 - we don't clear historical data on assignment. 
                            //else
                            //{
                            //    task.Taskgtuinfomappings.Clear();
                            //}

                            IList<Gtu> gtuList = ws.Repositories.GtuRepository.GetGtus(gtusId);                            
                            foreach (Gtu gtu in gtuList)
                            {
                                // check for GTUs
                                Taskgtuinfomapping mapping = task.Taskgtuinfomappings.FirstOrDefault(infomap => infomap.GTU.UniqueID == gtu.UniqueID);

                                if (mapping == null)
                                {
                                    Color color = this.GetRandomColor();
                                    mapping = new Taskgtuinfomapping();
                                    //mapping.GTUId = gtu.Id;
                                    mapping.GTU = gtu;
                                    mapping.UserId = gtu.User == null ? 0 : gtu.User.Id;
                                    mapping.UserColor = System.Drawing.ColorTranslator.ToHtml(color);
                                    mapping.TaskId = task.Id;
                                    task.Taskgtuinfomappings.Add(mapping);
                                }
                                                   
                                                                
                                //string colorName = ColorTranslator.ToHtml(color).TrimStart('#');
                                //string filePath =HttpContext.Current.Server.MapPath("~/Files/GtuDots") + "\\" + colorName + ".png";
                                //if (!System.IO.File.Exists(filePath))
                                //{
                                //    Bitmap bmp = new Bitmap(20, 20);
                                //    Graphics g = Graphics.FromImage(bmp);
                                //    Brush brush = new SolidBrush(color);
                                //    g.FillEllipse(brush, new Rectangle(1, 1, 18, 18));
                                //    bmp.Save(filePath, ImageFormat.Png);

                                //    brush.Dispose();
                                //    g.Dispose();
                                //    bmp.Dispose();
                                //}
                                
                            }                   
                            
                            ws.Repositories.TaskRepository.UpdateTaskWithChildren(task);
                        }

                        tx.Commit();
                    }
                    catch(Exception ex)
                    {
                        tx.Rollback();
                        Utilities.LogUtils.Error("WCF Unhandle Error", ex);
                        
                    }
                }
            }
        }
    }
}
