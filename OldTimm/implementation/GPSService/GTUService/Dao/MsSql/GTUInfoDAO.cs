using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using TIMM.GTUService;
using System.Data.SqlClient;
using GTUService.TIMM;

namespace TIMM.GTUService.MsSql
{
    public class GTUInfoDAO : IGTUInfoDAO
    {
        public void InsertGTUInfo(GTU gtuInfo, int taskInfoId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                if (gtuInfo.IPAddress == null)
                {
                    gtuInfo.IPAddress = "";
                }

                conn.Open();
                string sql = "INSERT INTO gtuInfo ( dwSpeed, nHeading, dtSendTime, dtReceivedTime, sIPAddress, nAreaCode, nNetworkCode, nCellID, nGPSFix, nAccuracy, nCount, nLocationID, sVersion, dwAltitude, dwLatitude, dwLongitude, powerInfo, code, taskgtuinfoId ,Status,Distance) " +
                                         " VALUES (@dwSpeed,@nHeading,@dtSendTime,@dtReceivedTime,@sIPAddress,@nAreaCode,@nNetworkCode,@nCellID,@nGPSFix,@nAccuracy,@nCount,@nLocationID,@sVersion,@dwAltitude,@dwLatitude,@dwLongitude,@powerInfo,@code,@taskgtuinfoId,@Status,@Distance)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlParameter[] paras = {                       
                        new SqlParameter("@dwSpeed",DbType.Double),
                        new SqlParameter("@nHeading",DbType.Int32),
                        new SqlParameter("@dtSendTime",DbType.DateTime),
                        new SqlParameter("@dtReceivedTime",DbType.DateTime),
                        new SqlParameter("@sIPAddress",DbType.String), 
                        new SqlParameter("@nAreaCode",DbType.Int32), 
                        new SqlParameter("@nNetworkCode",DbType.Int32), 
                        new SqlParameter("@nCellID",DbType.Int32), 
                        new SqlParameter("@nGPSFix",DbType.Int32), 
                        new SqlParameter("@nAccuracy",DbType.Int32), 
                        new SqlParameter("@nCount",DbType.Int32), 
                        new SqlParameter("@nLocationID",DbType.Int32), 
                        new SqlParameter("@sVersion",DbType.String), 
                        new SqlParameter("@dwAltitude",DbType.Double), 
                        new SqlParameter("@dwLatitude",DbType.Double), 
                        new SqlParameter("@dwLongitude",DbType.Double), 
                        new SqlParameter("@powerInfo",DbType.Int32), 
                        new SqlParameter("@code",DbType.String), 
                        new SqlParameter("@taskgtuinfoId",DbType.Int32),  
                        new SqlParameter("@Status",DbType.Int32),
                        new SqlParameter("@Distance",DbType.Double),
                                          
                    };
                paras[0].Value = gtuInfo.Speed;
                paras[1].Value = gtuInfo.Heading;
                paras[2].Value = gtuInfo.SendTime;
                paras[3].Value = DateTime.Now;
                paras[4].Value = gtuInfo.IPAddress;
                paras[5].Value = gtuInfo.AreaCode;
                paras[6].Value = gtuInfo.NetworkCode;
                paras[7].Value = gtuInfo.CellID;
                paras[8].Value = gtuInfo.GPSFix;
                paras[9].Value = gtuInfo.Accuracy;
                paras[10].Value = gtuInfo.Count;
                paras[11].Value = gtuInfo.LocationID;
                paras[12].Value = gtuInfo.Version;
                paras[13].Value = gtuInfo.CurrentCoordinate.Altitude;
                paras[14].Value = gtuInfo.CurrentCoordinate.Latitude;
                paras[15].Value = gtuInfo.CurrentCoordinate.Longitude;
                paras[16].Value = (int)gtuInfo.PowerInfo;
                paras[17].Value = gtuInfo.Code;

                //string testSwitch = ConfigurationManager.AppSettings["TestSwitch"].ToString();
                //if (testSwitch == "0")
                //{
                //    paras[18].Value = taskInfoId;
                //}

                if (taskInfoId == 0)
                {
                    paras[18].Value = DBNull.Value;
                }
                else
                {
                    paras[18].Value = taskInfoId;
                }
                paras[19].Value = gtuInfo.Status;
                paras[20].Value = gtuInfo.Distance;
                cmd.Parameters.AddRange(paras);
                cmd.ExecuteNonQuery();

            }
        }

        public void InsertGTU(string code)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string sql = "INSERT INTO gtus ( uniqueID, model, isEnabled, userId ) " +
                                      " VALUES (@uniqueID,@model,@isEnabled,@userId )";
                SqlParameter[] paras = {              
                        new SqlParameter("@uniqueID",DbType.String),
                        new SqlParameter("@model",DbType.String),
                        new SqlParameter("@isEnabled",DbType.Byte),                    
                        new SqlParameter("@userId",DbType.Int32),              
                                          
                    };
                
                paras[0].Value = code;
                paras[1].Value = DBNull.Value;
                paras[2].Value = 1;
                paras[3].Value = DBNull.Value;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddRange(paras);
                cmd.ExecuteNonQuery();
            }
        }

        public bool IsExistGTU(string code)
        {
            bool flag = false;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string sql = "SELECT count(*) FROM gtus WHERE uniqueID = @uniqueID ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@uniqueID", code);
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public int AvailableMappingId(GTU gtuInfo)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string stopHour = ConfigurationManager.AppSettings["StopHour"].ToString();
                #region
                //string sql = "SELECT m.id,g.uniqueID FROM taskgtuinfomapping  m  " +
                //             "INNER JOIN gtus g on m.gtuId = g.id " +
                //             "INNER JOIN task t on m.taskId = t.id " +
                //             "WHERE m.taskid IN " +
                //             "(SELECT taskid FROM tasktime WHERE timetype=0  AND id IN (select max(id) FROM tasktime GROUP BY taskid)"  +
                //               //"AND DATEDIFF(time ,now()) =0 " +
                //               " AND Hour(time) < " + stopHour + 
                //               " GROUP BY taskid) " +
                //             "AND g.uniqueID  = @code ";
                #endregion

                #region
                //string sql = "SELECT m.id,g.uniqueID FROM taskgtuinfomapping  m " +
                //             "INNER JOIN gtus g on m.gtuId = g.id " +
                //             "INNER JOIN task t on m.taskId = t.id " +
                //             "WHERE g.uniqueID  = @code " +
                //             "AND m.taskid in " +
                //             "(SELECT taskid FROM tasktime " +
                //             " WHERE timetype=0" +
                //             " AND datepart(hour, time) < " + stopHour +
                //             " AND id IN (select max(id) FROM tasktime GROUP BY taskid))";
                #endregion

                string sql = "select max(id) as id,uniqueID from(" +
                             "SELECT m.id,g.uniqueID FROM taskgtuinfomapping  m " +
                             "INNER JOIN gtus g on m.gtuId = g.id " +
                             "INNER JOIN task t on m.taskId = t.id " +
                             "WHERE g.uniqueID  = @code " +
                             "AND m.taskid in " +
                             "(SELECT taskid FROM tasktime " +
                             " WHERE timetype=0" +
                             " AND datepart(hour, time) < " + stopHour +
                             " AND id IN (select max(id) FROM tasktime GROUP BY taskid))"+
                             ") myTable group by uniqueID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@code", gtuInfo.Code);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    System.Diagnostics.Trace.TraceInformation("GTU:" + gtuInfo.Code + " need be inserted!");
                    reader.Read();
                    return reader.GetInt32(0);
                }
                else
                {
                    System.Diagnostics.Trace.TraceInformation("No effective GTU !");
                    return 0;
                }
            }
        }

        public bool UpdateGTU(GTU gtuInfo)
        {
            bool bRet = false;
            try
            {
                int taskInfoId = 0;
                string testSwitch = ConfigurationManager.AppSettings["TestSwitch"].ToString();
                if (testSwitch == "0")
                {
                    taskInfoId = AvailableMappingId(gtuInfo);
                }
                else
                {
                    taskInfoId = Convert.ToInt32(testSwitch);
                }

                InsertGTUInfo(gtuInfo, taskInfoId);
                System.Diagnostics.Trace.TraceInformation("Insert GTUInfo Successfully " + gtuInfo.Code + "\n");

                //if (taskInfoId > 0)
                //{
                //    InsertGTUInfo(gtuInfo, taskInfoId);
                //    System.Diagnostics.Trace.TraceInformation("Insert GTUInfo Successfully " + gtuInfo.Code + "\n");

                //}
                bRet = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceInformation("Update GTUInfo Error " + e.Message + "\n");
                bRet = false;
            }
            return bRet;
        }

        public bool UpdateGTUList(string code)
        {
            bool bRet = false;
            try
            {
                bool isExist = true;
                string testSwitch = ConfigurationManager.AppSettings["TestSwitch"].ToString();
                if (Convert.ToInt32(testSwitch) >= 0)
                {
                    isExist = IsExistGTU(code);
                }

                if (!isExist)
                {
                    InsertGTU(code);
                    System.Diagnostics.Trace.TraceInformation("Insert GTU List Successfully " + code + "\n");
                }
                bRet = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceInformation("Update GTU List Error " + e.Message + "\n");
                bRet = false;
            }
            return bRet;
        }

        public Dictionary<int, TaskInfoDctValue> AvailableTaskId()
        {
            Dictionary<int, TaskInfoDctValue> taskDic = new Dictionary<int, TaskInfoDctValue>();
            
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                string stopHour = ConfigurationManager.AppSettings["StopHour"].ToString();
                //string sql = "SELECT id, Email FROM task WHERE id in(SELECT taskid FROM tasktime WHERE timetype=0  AND id IN (select max(id) FROM tasktime GROUP BY taskid))";
                string sql = "SELECT ta.id as id, ta.Email as Email ,dm.name as Name FROM task as ta left join distributionmaps as dm on ta.dmid=dm.id WHERE ta.id in(SELECT taskid FROM tasktime WHERE timetype=0  AND id IN (select max(id) FROM tasktime GROUP BY taskid))";

                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    taskDic.Add(reader.GetInt32(0),
                        new TaskInfoDctValue(reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            string.Empty,
                            reader.IsDBNull(2) ? string.Empty : reader.GetString(2)));

                }
            }
            return taskDic;
        }

        public List<string> getGTULstByTaskId(int tid)
        {
            List<string> result = new List<string>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string sql = "SELECT uniqueID FROM taskgtuinfomapping  m  " +
                             "INNER JOIN gtus g on m.gtuId = g.id " +
                             "WHERE m.taskid =@taskid";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@taskid", tid);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? null : reader.GetString(0));
                }
            }
            return result;
        }

        public string getMailAddress(int tid)
        {
            string result = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string sql = "SELECT Email FROM task  WHERE id = @id ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", tid);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    result = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                }
            }
            return result;
        }

        public string getDMName(int tid)
        {
            string result = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string sql = "SELECT dm.name as Name FROM task as ta LEFT JOIN distributionmaps as dm on ta.dmid=dm.id  WHERE ta.id = @id ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", tid);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    result = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                }

            }
            return result;
        }

        public Dictionary<int, List<Coordinate>> getDmCollectionByTaskIds(List<int> tidList)
        {
            Dictionary<int, List<Coordinate>> resultDic = new Dictionary<int, List<Coordinate>>();
            foreach (int tid in tidList)
            {
                if (!resultDic.ContainsKey(tid))
                    resultDic.Add(tid, getDmCollectionByTaskId(tid));
                //string sql = "SELECT latitude ,longitude FROM distributionmapcoordinates  where distributionmapid =(select dmid from task where id = @id)";
                //SqlParameter[] paras = {                       
                //        new SqlParameter("@id",DbType.Int32)                  
                //    };
                //paras[0].Value = tid;

                //DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql,paras).Tables[0];
                //for (int i = 0; i < table.Rows.Count; i++)
                //{
                //    resultDic[tid].Add(new Coordinate { Altitude = 0, Latitude = Convert.ToDouble(table.Rows[i]["latitude"]), Longitude = Convert.ToDouble(table.Rows[i]["longitude"]) });
                //}
            }
            return resultDic;
        }

        public List<Coordinate> getDmCollectionByTaskId(int tid)
        {
            List<Coordinate> result = new List<Coordinate>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string sql = "SELECT latitude ,longitude FROM distributionmapcoordinates  where distributionmapid =(select dmid from task where id = @id)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", tid);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Coordinate
                    {
                        Altitude = 0,
                        Latitude = reader.IsDBNull(0) ? 0d : reader.GetDouble(0),
                        Longitude = reader.IsDBNull(1) ? 0d : reader.GetDouble(1)
                    });
                }

            }
            return result;
        }

        public Dictionary<int, string> getNDAreaIdsByTaskId(int tid)
        {
            //Dictionary<int, string> ret = new Dictionary<int, string>();
            List<int> boxids = new List<int>();
            double maxlan, minlan, maxlon, minlon;
            maxlan = minlan = maxlon = minlon = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string sql = @" SELECT 
                                max(latitude) as maxlan ,
                                max(longitude) as maxlon,
                                min(latitude) as minlan ,
                                min(longitude) as minlon 
                            FROM distributionmapcoordinates  
                            where distributionmapid =(select dmid from task where id = @id)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", tid);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    maxlan = reader.IsDBNull(0) ? 0d: reader.GetDouble(0);
                    maxlon = reader.IsDBNull(1) ? 0d : reader.GetDouble(1);
                    minlan = reader.IsDBNull(2) ? 0d : reader.GetDouble(2);
                    minlon = reader.IsDBNull(3) ? 0d : reader.GetDouble(3);
                }


                boxids.AddRange(getBoxIds(maxlan, minlan, maxlon, minlon, 25, 40));
                if (boxids.Count > 0)
                {
                    return getNDAreaIdsByBoxes(boxids);
                }
                else
                {
                    return null;
                }
            }
        }


        public Dictionary<int, string> getNDAreaIdsByBoxes(List<int> boxids)
        {
            Dictionary<int, string> ret = new Dictionary<int, string>();
            if (boxids.Count == 0) return null;
            string paraStr = "";
            foreach (int bid in boxids)
            {
                paraStr = bid + ",";

            }
            paraStr = paraStr.TrimEnd(',');
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string caSql = "SELECT name as infostr,CustomAreaId FROM customareas as ca left join customareaboxmappings as cab on ca.id =cab.customareaid where cab.boxid in (" + paraStr + ")";
                string ndSql = "SELECT street + ' CA' + ZipCode  as infoStr ,ndaddressId FROM ndaddresses as ca left join ndaddressboxmappings as cab on ca.id =cab.ndaddressid where cab.boxid in (" + paraStr + ")";
                //string caSql = "SELECT CustomAreaId FROM customareaboxmappings where boxid in (" + paraStr  + ")";
                //string ndSql = "SELECT NdAddressId FROM ndaddressboxmappings where boxid in (" + paraStr + ")";
                SqlCommand cmd = new SqlCommand(caSql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                //read ca table
                while (reader.Read())
                {
                    ret.Add(reader.GetInt32(1), 
                        reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
                reader.Close();
                //read nd table
                cmd.CommandText = ndSql;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ret.Add(reader.GetInt32(1),
                        reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }
            return ret;
        }

        public List<int> getBoxIds(double MaxLatitude, double MinLatitude, double MaxLongitude, double MinLongitude, int mountLat, int mountLon)
        {
            List<int> ids = new List<int>();

            int minLat = Convert.ToInt32(Math.Floor(MinLatitude * 100));
            minLat = minLat - (minLat % mountLat);
            if (MinLatitude < 0)
            {
                minLat -= mountLat;
            }

            if (MinLongitude < -170 && MaxLongitude > 170)
            {
                MinLongitude = MaxLongitude;
                MaxLongitude = MinLongitude + 360;
            }

            int minLon = Convert.ToInt32(Math.Floor(MinLongitude * 100));
            minLon = minLon - (minLon % mountLon);
            if (MinLongitude < 0)
            {
                minLon -= mountLon;
            }

            while (minLat < MaxLatitude * 100)
            {
                int tempLon = minLon;
                while (tempLon < MaxLongitude * 100)
                {
                    ids.Add(minLat * 100000 + tempLon);
                    tempLon += mountLon;
                }
                minLat += mountLat;
            }

            return ids;
        }

        public string getUserForEmail(string uid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["MsSql"]))
            {
                conn.Open();
                string sql = "SELECT u.username,g.uniqueID FROM taskgtuinfomapping  m " +
                             "INNER JOIN gtus g on m.gtuId = g.id " +
                             "INNER JOIN users u on m.userid = u.id " +
                             "WHERE g.uniqueID= @code";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@code", uid);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    return reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                }

                else
                {
                    return string.Empty;
                }
            }
        }

    }
}
