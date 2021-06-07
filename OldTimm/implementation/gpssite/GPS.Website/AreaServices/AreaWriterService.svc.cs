using System.ServiceModel;
using System.ServiceModel.Activation;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using GPS.DomainLayer.Enum;
using System.Collections.Generic;
using GPS.DomainLayer.Area;
using GPS.DomainLayer.Interfaces;
using System.Data;
using FileHelpers.DataLink;
using System.Configuration;
using GPS.Website.TransferObjects;
using GPS.DomainLayer.Area.Import;
using GPS.Website.AppFacilities;
using System.Web;
using System.IO;
using GPS.DomainLayer.Entities;
using GPS.DomainLayer.Security;
using GPS.DataLayer;

namespace GPS.Website.AreaServices
{
    [ServiceContract(Namespace = "TIMM.Website.AreaServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AreaWriterService
    {
        /// <summary>
        /// Add a custom area if the area does not exist.
        /// </summary>
        /// <remarks>A custom area uniquely identifies itself by its name.</remarks>
        /// <param name="name">The name of the custom area, required.</param>
        /// <param name="total">The H/H of the custom area, required.</param>
        /// <param name="description">A description of the custom area.</param>
        /// <param name="locations">The polygon coordinates of the custom area.</param>
        /// <returns>A <see cref="ResultMessage"/> indicating whether the custom area 
        /// is added successfully.</returns>
        [OperationContract]
        public ResultMessage AddCustomArea(String name, Int32 total, String description, Double[][] locations)
        {
            if (MapAreaManager.ExistCustomAreaName(name))
            {
                return new ResultMessage(false, "");
            }
            else
            {
                Int32 id = MapAreaManager.AddCustomArea(name, total, description, locations);
                return new ResultMessage(true, id.ToString());
            }
        }
        /// <summary>
        /// Add an non-deliverable address
        /// </summary>
        /// <param name="street">street line, required</param>
        /// <param name="zipCode">zip code, required</param>
        /// <param name="geofence">geofence, required</param>
        /// <param name="description">description of this address.</param>
        /// <returns>A <see cref="ResultAddress"/> indicating whether the address
        /// is added successfully.</returns>
        [OperationContract]
        public ResultAddress AddNonDeliverableAddress(string street, string zipCode, int geofence, string description)
        {
            ResultAddress ret = new ResultAddress();
            MapNdAddress address = MapAreaManager.AddNonDeliverableAddress(street, zipCode, geofence, description);
            if (address != null)
            {
                ret.IsSuccess = true;
                ret.Id = address.Id;
                ret.Street = address.Street;
                ret.ZipCode = address.ZipCode;
                ret.Geofence = address.Geofence;
                ret.Latitude = address.Latitude;
                ret.Longitude = address.Longitude;
                ret.Description = address.Description;
                double[][] locations = new double[address.Locations.Count][];
                for (int i = 0; i < address.Locations.Count; i++)
                {
                    locations[i] = new double[] { address.Locations[i].Latitude, address.Locations[i].Longitude };
                }
                ret.Locations = locations;
            }
            else
            {
                ret.IsSuccess = false;
                ret.Street = street;
                ret.ZipCode = zipCode;
                ret.Geofence = geofence;
                ret.Description = description;
            }
            return ret;
        }
        /// <summary>
        /// Add some non-deliverable address by file
        /// </summary>
        /// <param name="fileName">address file name</param>
        /// <returns></returns>
        [OperationContract]
        public IEnumerable<ResultAddress> AddNonDeliverableAddresses(string fileName)
        {
            try
            {
                GPS.Utilities.LogUtils.Info("AddNonDeliverableAddresses");
                List<ResultAddress> ret = new List<ResultAddress>();

                //ExcelStorage engine = new ExcelStorage(typeof(ExcelNdAddressRecord));
                string sPath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ndaddressfilepath"]) + fileName;
                //engine.FileName = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ndaddressfilepath"]) + fileName;
                //ExcelNdAddressRecord[] records = engine.ExtractRecords() as ExcelNdAddressRecord[];
                ScheduleTask task = new ScheduleTask()
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.Now,
                    Status = "I",
                    InUser = LoginMember.CurrentMember.Id
                };
                task.CreateAndFlush();
                string targetPath = ConfigurationManager.AppSettings["ParseNDAddressFolder"];
                File.Move(sPath, Path.Combine(targetPath, task.Id.Value.ToString()));
                var fileHandler = File.Create(Path.Combine(targetPath, task.Id.Value.ToString() + ".ok"));
                fileHandler.Close();
                #region Move the address file to background job. now only need copy file to job monitor folder
                //System.IO.StreamReader reader = new System.IO.StreamReader(sPath);
                //string sData = reader.ReadToEnd();
                //reader.Close();

                //sData = sData.Replace("\r", "");
                //string[] addressList = sData.Split('\n');

                //foreach (string sAddress in addressList)
                //{
                //    if (sAddress.Trim() == "") continue;
                //    ExcelNdAddressRecord record = new ExcelNdAddressRecord(sAddress);
                //    GPS.Utilities.LogUtils.Info(string.Format("street: {0}; zipcpde:{1}", record.Street, record.ZipCode));
                //    ret.Add(AddNonDeliverableAddress(record.Street, record.ZipCode, record.Geofence, record.Description));
                //}
                #endregion
                return ret;
            }
            catch (Exception ex)
            {
                GPS.Utilities.LogUtils.Error("WCF Unhandle Error", ex);
                return null;
            }
        }
        /// <summary>
        /// Remove an existing non-deliverable address
        /// </summary>
        /// <param name="addressId">this id of the address, required</param>
        /// <returns>A <see cref="ResultMessage"/> indicating whether the address 
        /// is removed successfully.</returns>
        [OperationContract]
        public ResultMessage RemoveNonDeliverableAddress(int addressId)
        {
            MapAreaManager.RemoveNonDeliverableAddress(addressId);
            return new ResultMessage(true, "");
        }

        /// <summary>
        /// Remove an existing custom area.
        /// </summary>
        /// <param name="name">The name of the custom area, required.</param>
        /// <returns>A <see cref="ResultMessage"/> indicating whether the custom area 
        /// is removed successfully.</returns>
        [OperationContract]
        public ResultMessage RemoveCustomArea(String name)
        {
            MapAreaManager.RemoveCustomArea(name);
            return new ResultMessage(true, "");
        }

        /// <summary>
        /// Turn a 5 digit zip area to non-deliverable or deliverable.
        /// </summary>
        /// <param name="code">The 5 digit zip code.</param>
        /// <param name="total">The total H/H of the 5 digit zip.</param>
        /// <param name="description">A description of the area.</param>
        /// <param name="enabled">A <see cref="Boolean"/> value indicating 
        /// whether the 5 digit zip area should be turned to non-deliverable.</param>
        /// <returns>A <see cref="ResultMessage"/> indicating whether the operation 
        /// is successful.</returns>
        [OperationContract]
        public ResultMessage SetFiveZipEnable(String code, Int32 total, String description, Boolean enabled)
        {
            MapAreaManager.SetFiveZipEnable(code, total, description, enabled);
            return new ResultMessage(true, "");
        }

        /// <summary>
        /// Turn a Census Tract area to non-deliverable or deliverable.
        /// </summary>
        /// <param name="stateCode">The state code of the Census Tract.</param>
        /// <param name="countyCode">The county code of the Census Tract.</param>
        /// <param name="code">The code of the Census Tract.</param>
        /// <param name="total">The total H/H of the Census Tract.</param>
        /// <param name="description">A description of the Census Tract.</param>
        /// <param name="enabled">A <see cref="Boolean"/> value indicating 
        /// whether the Census Tract should be turned to non-deliverable.</param>
        /// <returns>A <see cref="ResultMessage"/> indicating whether the operation 
        /// is successful.</returns>
        [OperationContract]
        public ResultMessage SetTractEnable(String stateCode, String countyCode, String code, Int32 total, String description, Boolean enabled)
        {
            MapAreaManager.SetTractEnable(stateCode, countyCode, code, total, description, enabled);
            return new ResultMessage(true, "");
        }

        /// <summary>
        /// Turn a Census Block Group area to non-deliverable or deliverable.
        /// </summary>
        /// <param name="stateCode">The state code of the Census Block Group.</param>
        /// <param name="countyCode">The county code of the Census Block Group.</param>
        /// <param name="tractCode">The tract code of the Census Block Group.</param>
        /// <param name="code">The code of the Census Block Group.</param>
        /// <param name="total">The total H/H of the Census Block Group.</param>
        /// <param name="description">A description of the Census Block Group.</param>
        /// <param name="enabled">A <see cref="Boolean"/> value indicating 
        /// whether the Census Block Group should be turned to non-deliverable.</param>
        /// <returns>A <see cref="ResultMessage"/> indicating whether the operation 
        /// is successful.</returns>
        [OperationContract]
        public ResultMessage SetBlockGroupEnable(String stateCode, String countyCode, String tractCode, String code, Int32 total, String description, Boolean enabled)
        {
            MapAreaManager.SetBlockGroupEnable(stateCode, countyCode, tractCode, code, total, description, enabled);
            return new ResultMessage(true, "");
        }

        [OperationContract]
        public IEnumerable<ToAreaRecord> ImportData(int campaignId, string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["importdatafilepath"] + fileName);
            Importer importer = new Importer();
            IEnumerable<AreaRecord> records = importer.ImportFile(campaignId, filePath);
            return AssemblerConfig.GetAssembler<ToAreaRecord, AreaRecord>().AssembleFrom(records);
        }

        [OperationContract]
        public void AdjustData(int campaignId, int classification, ToAdjustData[] datas)
        {
            Importer importer = new Importer();
            foreach (ToAdjustData data in datas)
            {
                importer.AdjustCount(campaignId, classification, data.Id, data.Total, data.Count, data.PartPercentage, data.IsPartModified, false);
            }
        }

        [OperationContract]
        public void AdjustDataReset(int campaignId, int classification, string zipCode)
        {
            Importer oImporter = new Importer();
            oImporter.AdjustCountReset(campaignId, classification, zipCode);
        }
    }

    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class ResultMessage
    {
        public ResultMessage(Boolean isSuccess, String message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        [DataMember]
        public Boolean IsSuccess { get; set; }

        [DataMember]
        public String Message { get; set; }

    }

    [DataContract(Namespace = "TIMM.Website.AreaServices")]
    public class ResultAddress
    {
        [DataMember]
        public Boolean IsSuccess { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public int Geofence { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double[][] Locations { get; set; }
        [DataMember]
        public string Description { get; set; }
    }

}
