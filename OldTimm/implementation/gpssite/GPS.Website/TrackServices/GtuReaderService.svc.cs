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

namespace GPS.Website.TrackServices
{
    [ServiceContract(Namespace = "TIMM.Website.TrackServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GtuReaderService
    {
        [OperationContract]
        public IEnumerable<ToGtu> GetAllGtus()
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        IEnumerable<Gtu> gtus = ws.Repositories.GtuRepository.GetAllGtus();
                        tx.Commit();

                        var rtnGtus = AssemblerConfig.GetAssembler<ToGtu, Gtu>().AssembleFrom(gtus);
                        for (int i = 0; i < rtnGtus.Count(); i++)
                        {
                            if (gtus.ElementAt(i).User != null)
                            {
                                rtnGtus.ElementAt(i).UserName = gtus.ElementAt(i).User.FullName;
                            }
                        }

                        return rtnGtus;
                    }
                    catch (Exception ex) {
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        tx.Rollback(); return null;}
                }
            }
        }

        /// <summary>
        /// Return a <see cref="Gtus"/> by its Unique Id.
        /// </summary>
        /// <param name="userName">The Unique Id of the Gtu to be fetched.</param>
        /// <returns>A <see cref="Gtus"/> object matching the specified Unique Id.</returns>
        [OperationContract]
        public ToGtu GetGtu(string uniqueId)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        Gtu gtu = ws.Repositories.GtuRepository.GetGtu(uniqueId);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToGtu, Gtu>().AssembleFrom(gtu);
                    }
                    catch (Exception ex) {
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        tx.Rollback(); return null; }
                }
            }
            
        }


        /// <summary>
        /// Return a <see cref="Gtus"/> by its Unique Id.
        /// </summary>
        /// <param name="userName">The Unique Id of the Gtu to be fetched.</param>
        /// <returns>A <see cref="Gtus"/> object matching the specified Unique Id.</returns>
        [OperationContract]
        public IEnumerable<ToGtu> LoadGtuFromExcel(string filename)
        {
            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                using (GPS.DataLayer.DataInfrastructure.ITransaction tx = ws.BeginTransaction())
                {
                    try
                    {
                        var fileName = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["gtufilepath"]) + filename;
                        IEnumerable<Gtu> gtus = ws.Repositories.GtuRepository.LoadGtuFromExcel(fileName);
                        tx.Commit();
                        return AssemblerConfig.GetAssembler<ToGtu, Gtu>().AssembleFrom(gtus);
                    }
                    catch (Exception ex) {
                        GPS.Utilities.LogUtils.Error("WCF Unhandle error", ex);
                        tx.Rollback(); return null; }
                }
            }
        }
    }
}
