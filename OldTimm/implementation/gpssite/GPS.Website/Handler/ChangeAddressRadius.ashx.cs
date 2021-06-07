using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using GPS.Website.TransferObjects;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.Website.AppFacilities;
using Newtonsoft.Json;
using GPS.DomainLayer.Area.Addresses;
using GPS.DomainLayer.Interfaces;
using GPS.DomainLayer.Area;
using GPS.DataLayer.DataInfrastructure;
using log4net;


namespace GPS.Website.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ChangeAddressRadius : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            int addressId = Convert.ToInt32(context.Request.QueryString["addressid"]);
            double lat = Convert.ToDouble(context.Request.QueryString["latitude"]);
            double lon = Convert.ToDouble(context.Request.QueryString["longitude"]);
            string rd = context.Request.QueryString["radiuses"];


            List<ToAddressRadius> radiuses = new List<ToAddressRadius>();

            string[] radius = rd.Split('^');
            for (int i = 0; i < radius.Length; i++)
            {
                string[] column = radius[i].Split('|');

                ToAddressRadius address = new ToAddressRadius();
                address.Id = Convert.ToInt32(column[0]);
                address.IsDisplay = column[1] == "true" ? true : false;
                address.Length = Convert.ToDouble(column[2]);
                address.LengthMeasuresId = Convert.ToInt32(column[3]);
                address.Relations = new Dictionary<int, Dictionary<int, string>>();
                radiuses.Add(address);               

            }

             context.Response.Write(JsonConvert.SerializeObject(ChangeAddressRadiusFunction(addressId, lat, lon, radiuses)));


        }



        public ToAddressRadius[] ChangeAddressRadiusFunction(int addressId, double lat, double lon, List<ToAddressRadius> radiuses)
        {
            AddressOperator oper = new AddressOperator();
            ICoordinate center = new Coordinate(lat, lon);
            foreach (ToAddressRadius radius in radiuses)
            {
                radius.Relations = oper.GetRadiusRelations(center, radius.Length, radius.LengthMeasuresId);
            }

            using (IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Address address = ws.Repositories.AddressRepository.GetEntity(addressId);

                if (address != null)
                {
                    foreach (ToAddressRadius toRadius in radiuses)
                    {
                        Radiuse radius = address.Radiuses.Where(t => t.Id == toRadius.Id).SingleOrDefault();
                        if (radius != null)
                        {
                            radius.Length = toRadius.Length;
                            radius.LengthMeasuresId = toRadius.LengthMeasuresId;
                            List<RadiusRecord> records = new List<RadiusRecord>();
                            foreach (int c in toRadius.Relations.Keys)
                            {
                                foreach (int id in toRadius.Relations[c].Keys)
                                {
                                    records.Add(new RadiusRecord()
                                    {
                                        AreaId = id,
                                        Classification = (GPS.DomainLayer.Enum.Classifications)c,
                                        //RadiusId = toRadius.Id
                                        Radiuse = radius
                                    });
                                }
                            }
                            if (records.Count > 0)
                            {
                                using (IBulkWorkSpace bws = WorkSpaceManager.Instance.NewBulkWorkSpace())
                                {
                                    using (ITransaction tx = bws.BeginTransaction())
                                    {
                                        try
                                        {
                                            bws.Repositories.BulkRadiusRecordRepository.DeleteRadiusRecords(toRadius.Id);
                                            bws.Repositories.BulkRadiusRecordRepository.InsertEntityList(records);
                                            tx.Commit();
                                        }
                                        catch (Exception ex)
                                        {
                                            tx.Rollback();
                                            ILog logger = LogManager.GetLogger(GetType());
                                            logger.Error("WCF Unhandle Error", ex);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    address.Latitude = lat;
                    address.Longitude = lon;
                    ws.Repositories.AddressRepository.Update(address);
                    ws.Commit();
                }
            }

            return radiuses.ToArray();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
