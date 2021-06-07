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
using System.IO;

namespace GPS.Website.TrackServices
{
    [ServiceContract(Namespace = "TIMM.Website.TrackServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GtuWriterService
    {
        /// <summary>
        /// Delete a GUT specified by unique id.
        /// </summary>
        /// <param name="uniqueId">The unique id of the GUT to be deleted.</param>
        [OperationContract]
        public void DeleteGtu(string uniqueId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        GtuRepository gtuRep = new GtuRepository();
                        ws.Repositories.GtuRepository.DeleteGtu(uniqueId);
                        tx.Commit();
                    }
                    catch (Exception ex) {
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        tx.Rollback();}
                }
            }
        }

        //[OperationContract]
        //public ToGtu UnassignGtu(string uniqueId)
        //{
        //    using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
        //    {
        //        using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
        //        {
        //            try
        //            {
        //                Gtu gtu = ws.Repositories.GtuRepository.GetGtu(uniqueId);

        //                return AssemblerConfig.GetAssembler<ToGtu, Gtu>().AssembleFrom(gtu);
        //            }
        //            catch (Exception ex)
        //            {
        //                GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
        //                tx.Rollback(); return null;
        //            }
        //        }
        //    }
        //}

        //[OperationContract]
        //public int ImportGtuInfo(int iTaskGtuInfoId, string sFileContent)
        //{
        //    sFileContent = sFileContent.Replace("\r", "");
        //    string[] GtuInfoLines = sFileContent.Split('\n');

        //    List<Gtuinfo> gtuInfoList = new List<Gtuinfo>();
        //    using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
        //    {
        //        using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
        //        {
        //            for (int i = 1; i < GtuInfoLines.Length; i++)
        //            {
        //                string[] gtuInfoData = GtuInfoLines[i].Split(',');
        //                if (gtuInfoData.Length < 3)
        //                    continue;

        //                Gtuinfo oGtuInfo = new Gtuinfo();
        //                oGtuInfo.TaskgtuinfoId = iTaskGtuInfoId;
        //                oGtuInfo.dwLongitude = Convert.ToDouble(gtuInfoData[0]);
        //                oGtuInfo.dwLatitude = Convert.ToDouble(gtuInfoData[1]);
        //                oGtuInfo.dtSendTime = Convert.ToDateTime(gtuInfoData[2]);

        //                gtuInfoList.Add(oGtuInfo);
        //            }

        //            GtuInfoRepository gtuRep = new GtuInfoRepository();
        //            gtuRep.AddGtuInfo(gtuInfoList);

        //            return gtuInfoList.Count;
        //        }
        //    }
        //}


        [OperationContract]
        public int ImportGtuInfo(SortedList<string, int> taskGtuMap, string sFileContent)
        {
            StringReader reader = new StringReader(sFileContent);
            List<string> GtuInfoLines = new List<string>();
            string sLine = "";
            while ((sLine = reader.ReadLine()) != null)
                GtuInfoLines.Add(sLine);

            if (GtuInfoLines.Count == 0)
                return 0;

            char seperator;
            sLine = GtuInfoLines[0];
            if (sLine.IndexOf("\t") > 0)
                seperator = '\t';
            else if (sLine.IndexOf(",") > 0)
                seperator = ',';
            else
                throw new Exception("Please seperate column with ',' or TAB");

            List<Gtuinfo> gtuInfoList = new List<Gtuinfo>();
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    for (int i = 0; i < GtuInfoLines.Count; i++)
                    {
                        string[] gtuInfoData = GtuInfoLines[i].Split(seperator);
                        if (gtuInfoData.Length < 4)
                            continue;

                        string gtuNumber = gtuInfoData[0];
                        if (taskGtuMap.ContainsKey(gtuNumber) == false)
                            continue;

                        Gtuinfo oGtuInfo = new Gtuinfo();
                        oGtuInfo.TaskgtuinfoId = taskGtuMap[gtuNumber];
                        oGtuInfo.dwLongitude = Convert.ToDouble(gtuInfoData[1]);
                        oGtuInfo.dwLatitude = Convert.ToDouble(gtuInfoData[2]);
                        oGtuInfo.dtSendTime = Convert.ToDateTime(gtuInfoData[3]);

                        gtuInfoList.Add(oGtuInfo);
                    }

                    GtuInfoRepository gtuRep = new GtuInfoRepository();
                    gtuRep.AddGtuInfo(gtuInfoList);

                    return gtuInfoList.Count;
                }
            }
        }

        /// <summary>
        /// Update GTU
        /// </summary>
        /// <param name="ToGtu">ToGtu object</param>
        /// <returns>Updated ToGtu object</returns>
        [OperationContract]
        public ToGtu UpdateGtu(ToGtu togtu)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Gtu u = AssemblerConfig.GetAssembler<Gtu, ToGtu>().AssembleFrom(togtu);

                        User user = ws.Repositories.UserRepository.GetUser(togtu.UserId);

                        List<Task> taskList = ws.Repositories.TaskRepository.GetTasksByGtu(u).ToList<Task>();

                        foreach (Task t in taskList)
                        {
                            Taskgtuinfomapping mapping = t.Taskgtuinfomappings.Where<Taskgtuinfomapping>(map => map.TaskId == t.Id).SingleOrDefault<Taskgtuinfomapping>();
                            mapping.UserId = user == null ? -1 : user.Id;
                            ws.Repositories.TaskRepository.UpdateTaskWithChildren(t);
                        }
                        
                        u.User = user;
                        Gtu gtu = ws.Repositories.GtuRepository.UpdateGtu(u);
                        tx.Commit();

                        // change UserName to FullName
                        var rtnGtu = AssemblerConfig.GetAssembler<ToGtu, Gtu>().AssembleFrom(gtu);
                        rtnGtu.UserName = gtu.User == null ? string.Empty : gtu.User.FullName;

                        return rtnGtu;                        
                    }
                    catch (Exception ex) {
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        tx.Rollback(); return null;}
                }
            }
        }
    }
}

