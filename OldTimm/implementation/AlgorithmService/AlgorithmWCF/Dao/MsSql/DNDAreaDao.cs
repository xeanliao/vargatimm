using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace WHYTAlgorithmService.Geo.MsSql
{
    public class DNDAreaDao : IDNDAreaDao
    {
        public Dictionary<int, List<Coordinate>> AvailableDNDArea()
        {
            Dictionary<int, List<Coordinate>> DNDAreaDct = new Dictionary<int, List<Coordinate>>();

            string sql = "(SELECT CustomAreaId as areaid, Latitude, Longitude FROM customareacoordinates) union (SELECT NdAddressId as areaid, Latitude, Longitude FROM ndaddresscoordinates)";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    if (!DNDAreaDct.ContainsKey(id))
                    {
                        DNDAreaDct.Add(id, new List<Coordinate>());
                    }
                    DNDAreaDct[id].Add(new Coordinate
                    {
                        Altitude = 0,
                        Latitude = reader.GetDouble(1),
                        Longitude = reader.GetDouble(2)
                    });

                }
            }
            return DNDAreaDct;
        }
    }
}
