using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.Web;
using TIMM.GPS.Model;
using TIMM.GPS.RESTService.Helper;
using System.Configuration;
using System.Data.Entity;
using System.Data;
using System.Text.RegularExpressions;

namespace TIMM.GPS.RESTService
{
    [ServiceContract]
    public class NdAddressService
    {
        [WebGet(UriTemplate = "query")]
        public List<NdAddress> GetAllNdAddress()
        {
            using (var context = new TIMMContext())
            {
                return context.NdAddresses.ToList();
            }
        }

        [WebGet(UriTemplate = "/all/locations")]
        public List<List<Location>> GetAllNdAddressLocations()
        {
            string sqlCustomerArea = @"
                SELECT [CustomAreaId]
                      ,[Latitude]
                      ,[Longitude]
                  FROM [dbo].[customareacoordinates]
              ORDER BY [CustomAreaId],[Id]
            ";
            string sqlNdAddress = @"
                SELECT [NdAddressId]
                      ,[Latitude]
                      ,[Longitude]
                  FROM [dbo].[ndaddresscoordinates]
              ORDER BY [NdAddressId],[Id]
            ";
            List<List<Location>> result = new List<List<Location>>();
            List<string> ndAddress = new List<string>();
            using (var context = new TIMMContext())
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = sqlCustomerArea;
                var reader = cmd.ExecuteReader();
                int? currentId = null;
                int id = 0;
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                    if(currentId == null || currentId != id)
                    {
                        result.Add(new List<Location>());
                        currentId = id;
                    }
                    
                    result[result.Count - 1].Add(new Location 
                    {
                        Latitude = reader.GetDouble(1),
                        Longitude = reader.GetDouble(2)
                    });
                }
                reader.Close();

                cmd.CommandText = sqlNdAddress;
                reader = cmd.ExecuteReader();
                currentId = null;
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                    if (currentId == null || currentId != id)
                    {
                        result.Add(new List<Location>());
                        currentId = id;
                    }
                    
                    result[result.Count - 1].Add(new Location
                    {
                        Latitude = reader.GetDouble(1),
                        Longitude = reader.GetDouble(2)
                    });
                }
            }
            return result;
        }

        internal static List<NdAddress> GetAllNdAddressAndCustomArea()
        {
            string sql = @"
                SELECT  *
                FROM    ( SELECT    0 AS [TableId] ,
                                    [N].[Id] ,
                                    [L].[Id] AS [LId] ,
                                    [N].[Street] ,
                                    [N].[ZipCode] ,
                                    [N].[Description] ,
                                    [L].[Latitude] ,
                                    [L].[Longitude]
                          FROM      [dbo].[ndaddresses] AS N
                                    INNER JOIN [dbo].[ndaddresscoordinates] AS L ON [N].[Id] = [L].[NdAddressId]
                          UNION ALL
                          SELECT    1 AS [TableId] ,
                                    [C].[Id] AS [LId] ,
                                    [L].[Id] ,
                                    [C].[Name] AS [Street] ,
                                    '00000' AS [ZipCode] ,
                                    [C].[Description] ,
                                    [L].[Latitude] ,
                                    [L].[Longitude]
                          FROM      [dbo].[customareas] AS C
                                    INNER JOIN [dbo].[customareacoordinates] AS L ON [C].[Id] = [L].[CustomAreaId]
                        ) AS T
                ORDER BY [T].[TableId] ,[T].[Id] ,[T].[LId]
            ";
            List<NdAddress> result = new List<NdAddress>();
            using (var context = new TIMMContext())
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                int? currentTableId = null;
                int? currentNdAddressId = null;
                NdAddress currentNdAddress = null;
                while(reader.Read())
                {
                    int tableId = reader.GetInt32(0);
                    int ndAddressId = reader.GetInt32(1);
                    if (currentTableId.HasValue
                        && currentTableId.Value != tableId
                        && currentNdAddressId.HasValue
                        && currentNdAddressId.Value != ndAddressId)
                    {
                        currentTableId = tableId;
                        currentNdAddressId = ndAddressId;
                        currentNdAddress = new NdAddress
                        {
                            Id = ndAddressId,
                            Street = reader.GetString(3),
                            ZipCode = reader.GetString(4),
                            Description = reader.GetString(5),
                            Locations = new List<Location>()
                        };
                        result.Add(currentNdAddress);
                    }
                    currentNdAddress.Locations.Add(new Location { 
                        Latitude = reader.GetFloat(6),
                        Longitude = reader.GetFloat(7)
                    });
                }
            }
            return result;
        }
    }
}
