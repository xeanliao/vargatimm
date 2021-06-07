using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace WHYTAlgorithmService.Geo.MySql
{
    public class DNDAreaDao : IDNDAreaDao
    {
        public Dictionary<int, List<Coordinate>> AvailableDNDArea()
        {
            Dictionary<int, List<Coordinate>> DNDAreaDct = new Dictionary<int, List<Coordinate>>();

            string sql = "(SELECT CustomAreaId as areaid, Latitude, Longitude FROM customareacoordinates) union (SELECT NdAddressId as areaid, Latitude, Longitude FROM ndaddresscoordinates)";
            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int id = Convert.ToInt32(table.Rows[i]["areaid"]);
                if (!DNDAreaDct.ContainsKey(id))
                {
                    DNDAreaDct.Add(id, new List<Coordinate>());
                }

                DNDAreaDct[id].Add(new Coordinate { Altitude = 0, Latitude = Convert.ToDouble(table.Rows[i]["latitude"]), Longitude = Convert.ToDouble(table.Rows[i]["longitude"]) });

            }
            return DNDAreaDct;
        }
    }
}
