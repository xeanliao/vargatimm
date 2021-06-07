using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using GTUService.TIMM;

namespace TIMM.GTUService.MySql
{
    public class GTUInfoDAO : IGTUInfoDAO
    {
        public void InsertGTUInfo(GTU gtuInfo, int taskInfoId)
        {
            using (MySqlConnection conn = new MySqlConnection(DbHelper.ConnectionString))
            {
                conn.Open();
                string sql = "INSERT INTO gtuInfo ( dwSpeed, nHeading, dtSendTime, dtReceivedTime, sIPAddress, nAreaCode, nNetworkCode, nCellID, nGPSFix, nAccuracy, nCount, nLocationID, sVersion, dwAltitude, dwLatitude, dwLongitude, powerInfo, code, taskgtuinfoId ,Status,Distance) " +
                                         " VALUES (?dwSpeed,?nHeading,?dtSendTime,?dtReceivedTime,?sIPAddress,?nAreaCode,?nNetworkCode,?nCellID,?nGPSFix,?nAccuracy,?nCount,?nLocationID,?sVersion,?dwAltitude,?dwLatitude,?dwLongitude,?powerInfo,?code,?taskgtuinfoId,?Status,?Distance)";
                MySqlParameter[] paras = {                       
                        new MySqlParameter("?dwSpeed",MySqlDbType.Double),
                        new MySqlParameter("?nHeading",MySqlDbType.Int32),
                        new MySqlParameter("?dtSendTime",MySqlDbType.DateTime),
                        new MySqlParameter("?dtReceivedTime",MySqlDbType.DateTime),
                        new MySqlParameter("?sIPAddress",MySqlDbType.VarChar), 
                        new MySqlParameter("?nAreaCode",MySqlDbType.Int32), 
                        new MySqlParameter("?nNetworkCode",MySqlDbType.Int32), 
                        new MySqlParameter("?nCellID",MySqlDbType.Int32), 
                        new MySqlParameter("?nGPSFix",MySqlDbType.Int32), 
                        new MySqlParameter("?nAccuracy",MySqlDbType.Int32), 
                        new MySqlParameter("?nCount",MySqlDbType.Int32), 
                        new MySqlParameter("?nLocationID",MySqlDbType.Int32), 
                        new MySqlParameter("?sVersion",MySqlDbType.VarChar), 
                        new MySqlParameter("?dwAltitude",MySqlDbType.Double), 
                        new MySqlParameter("?dwLatitude",MySqlDbType.Double), 
                        new MySqlParameter("?dwLongitude",MySqlDbType.Double), 
                        new MySqlParameter("?powerInfo",MySqlDbType.Int32), 
                        new MySqlParameter("?code",MySqlDbType.VarChar), 
                        new MySqlParameter("?taskgtuinfoId",MySqlDbType.Int32),  
                        new MySqlParameter("?Status",MySqlDbType.Int32),
                        new MySqlParameter("?Distance",MySqlDbType.Double),
                                          
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

                string testSwitch = ConfigurationManager.AppSettings["TestSwitch"].ToString();
                if (testSwitch == "0")
                {
                    paras[18].Value = taskInfoId;
                }
                paras[19].Value = gtuInfo.Status;
                paras[20].Value = gtuInfo.Distance;

                DbHelper.ExecuteNonQuery(conn, CommandType.Text, sql, paras);

            }
        }

        public void InsertGTU(string code)
        {
            using (MySqlConnection conn = new MySqlConnection(DbHelper.ConnectionString))
            {
                conn.Open();
                string sql = "INSERT INTO gtus (  uniqueID, model, isEnabled, userId ) " +
                                      " VALUES (?uniqueID,?model,?isEnabled,?userId )";
                MySqlParameter[] paras = {           
                        new MySqlParameter("?uniqueID",MySqlDbType.String),
                        new MySqlParameter("?model",MySqlDbType.String),
                        new MySqlParameter("?isEnabled",MySqlDbType.Bit),                    
                        new MySqlParameter("?userId",MySqlDbType.Int32),              
                                          
                    };
                
                paras[0].Value = code;
                paras[1].Value = DBNull.Value;
                paras[2].Value = 1;
                paras[3].Value = DBNull.Value;


                DbHelper.ExecuteNonQuery(conn, CommandType.Text, sql, paras);
            }
        }

        public bool IsExistGTU(string code)
        {
            bool flag = false;
            string sql = "SELECT id FROM gtus WHERE uniqueID = ?uniqueID ";
            MySqlParameter[] paras = {                       
                        new MySqlParameter("?uniqueID",MySqlDbType.String),
            };
            paras[0].Value = code;
            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql, paras).Tables[0];
            if (table.Rows.Count > 0)
            {
                flag = true;
            }
            return flag;
        }

        public int AvailableMappingId(GTU gtuInfo)
        {
            string stopHour = ConfigurationManager.AppSettings["StopHour"].ToString();
            //string sql = "SELECT m.id,g.uniqueID FROM taskgtuinfomapping  m  " +
            //             "INNER JOIN gtus g on m.gtuId = g.id " +
            //             "INNER JOIN task t on m.taskId = t.id " +
            //             "WHERE m.taskid IN " +
            //             "(SELECT taskid FROM tasktime WHERE timetype=0  AND id IN (select max(id) FROM tasktime GROUP BY taskid)"  +
            //               //"AND DATEDIFF(time ,now()) =0 " +
            //               " AND Hour(time) < " + stopHour + 
            //               " GROUP BY taskid) " +
            //             "AND g.uniqueID  = ?code ";
            #region
            //string sql = "SELECT m.id,g.uniqueID FROM taskgtuinfomapping  m " +
            //             "INNER JOIN gtus g on m.gtuId = g.id " +
            //             "INNER JOIN task t on m.taskId = t.id " +
            //             "WHERE g.uniqueID  = ?code " +
            //             "AND m.taskid in " +
            //             "(SELECT taskid FROM tasktime " +
            //             " WHERE timetype=0" +
            //             " AND Hour(time) < " + stopHour +
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
                             " AND id IN (select max(id) FROM tasktime GROUP BY taskid))" +
                             ") myTable group by uniqueID";

            MySqlParameter[] paras = {                       
                        new MySqlParameter("?code",MySqlDbType.String),
            };
            paras[0].Value = gtuInfo.Code;

            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql, paras).Tables[0];
            if (table.Rows.Count > 0)
            {
                System.Diagnostics.Trace.TraceInformation("GTU:" + gtuInfo.Code + " need be inserted!");
                return Convert.ToInt32(table.Rows[0]["id"]);

            }
            else
            {
                System.Diagnostics.Trace.TraceInformation("No effective GTU !");
                return 0;
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

                if (taskInfoId > 0)
                {
                    InsertGTUInfo(gtuInfo, taskInfoId);
                    System.Diagnostics.Trace.TraceInformation("Insert GTUInfo Successfully " + gtuInfo.Code + "\n");

                }
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
            string stopHour = ConfigurationManager.AppSettings["StopHour"].ToString();
            //string sql = "SELECT id, Email FROM task WHERE id in(SELECT taskid FROM tasktime WHERE timetype=0  AND id IN (select max(id) FROM tasktime GROUP BY taskid))";
            string sql = "SELECT ta.id as id, ta.Email as Email ,dm.name as Name FROM task as ta left join distributionmaps as dm on ta.dmid=dm.id WHERE ta.id in(SELECT taskid FROM tasktime WHERE timetype=0  AND id IN (select max(id) FROM tasktime GROUP BY taskid))";

            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                taskDic.Add(Convert.ToInt32(table.Rows[i]["id"]), new TaskInfoDctValue(table.Rows[i]["Name"].ToString(), string.Empty, table.Rows[i]["Email"].ToString()));

            }
            return taskDic;
        }

        public List<string> getGTULstByTaskId(int tid)
        {
            List<string> result = new List<string>();
            string sql = "SELECT uniqueID FROM taskgtuinfomapping  m  " +
                         "INNER JOIN gtus g on m.gtuId = g.id " +
                         "WHERE m.taskid =?taskid";

            MySqlParameter[] paras = {                       
                        new MySqlParameter("?taskid",MySqlDbType.String),
            };
            paras[0].Value = tid;
            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql, paras).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                result.Add(table.Rows[i]["uniqueID"].ToString());

            }
            return result;
        }

        public string getMailAddress(int tid)
        {
            string result = string.Empty;
            string sql = "SELECT Email FROM task  WHERE id = ?id ";
            MySqlParameter[] paras = {                       
                        new MySqlParameter("?id",MySqlDbType.String),
            };
            paras[0].Value = tid;
            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql, paras).Tables[0];
            if (table.Rows.Count > 0)
            {
                return table.Rows[0]["Email"].ToString();
            }
            else
                return result;
        }

        public string getDMName(int tid)
        {
            string result = string.Empty;
            string sql = "SELECT dm.name as Name FROM task as ta LEFT JOIN distributionmaps as dm on ta.dmid=dm.id  WHERE ta.id = ?id ";
            MySqlParameter[] paras = {                       
                        new MySqlParameter("?id",MySqlDbType.String),
            };
            paras[0].Value = tid;
            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql, paras).Tables[0];
            if (table.Rows.Count > 0)
            {
                return table.Rows[0]["Name"].ToString();
            }
            else
                return result;
        }

        public Dictionary<int, List<Coordinate>> getDmCollectionByTaskIds(List<int> tidList)
        {
            Dictionary<int, List<Coordinate>> resultDic = new Dictionary<int, List<Coordinate>>();
            foreach (int tid in tidList)
            {
                if (!resultDic.ContainsKey(tid))
                    resultDic.Add(tid, getDmCollectionByTaskId(tid));
                //string sql = "SELECT latitude ,longitude FROM distributionmapcoordinates  where distributionmapid =(select dmid from task where id = ?id)";
                //MySqlParameter[] paras = {                       
                //        new MySqlParameter("?id",MySqlDbType.Int32)                  
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
            string sql = "SELECT latitude ,longitude FROM distributionmapcoordinates  where distributionmapid =(select dmid from task where id = ?id)";
            MySqlParameter[] paras = {                       
                        new MySqlParameter("?id",MySqlDbType.Int32)                  
                    };
            paras[0].Value = tid;

            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql, paras).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                result.Add(new Coordinate { Altitude = 0, Latitude = Convert.ToDouble(table.Rows[i]["latitude"]), Longitude = Convert.ToDouble(table.Rows[i]["longitude"]) });
            }
            return result;
        }

        public Dictionary<int, string> getNDAreaIdsByTaskId(int tid)
        {
            //Dictionary<int, string> ret = new Dictionary<int, string>();
            List<int> boxids = new List<int>();
            double maxlan, minlan, maxlon, minlon;
            maxlan = minlan = maxlon = minlon = 0;
            string sql = "SELECT max(latitude) as maxlan ,max(longitude) as maxlon,min(latitude) as minlan ,min(longitude) as minlon FROM distributionmapcoordinates  where distributionmapid =(select dmid from task where id = ?id)";
            MySqlParameter[] paras = {                       
                        new MySqlParameter("?id",MySqlDbType.Int32)                  
                    };
            paras[0].Value = tid;

            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql, paras).Tables[0];
            if (table.Rows.Count > 0)
            {
                maxlan = Convert.ToDouble(table.Rows[0]["maxlan"]);
                maxlon = Convert.ToDouble(table.Rows[0]["maxlon"]);
                minlan = Convert.ToDouble(table.Rows[0]["minlan"]);
                minlon = Convert.ToDouble(table.Rows[0]["minlon"]);
            }
            boxids.AddRange(getBoxIds(maxlan, minlan, maxlon, minlon, 25, 40));
            if (boxids.Count > 0)
                return getNDAreaIdsByBoxes(boxids);
            else
                return null;
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
            string caSql = "SELECT name as infostr,CustomAreaId FROM customareas as ca left join customareaboxmappings as cab on ca.id =cab.customareaid where cab.boxid in (" + paraStr + ")";
            string ndSql = "SELECT concat(street , ' CA' , ZipCode)  as infoStr ,ndaddressId FROM ndaddresses as ca left join ndaddressboxmappings as cab on ca.id =cab.ndaddressid where cab.boxid in (" + paraStr + ")";
            //string caSql = "SELECT CustomAreaId FROM customareaboxmappings where boxid in (" + paraStr  + ")";
            //string ndSql = "SELECT NdAddressId FROM ndaddressboxmappings where boxid in (" + paraStr + ")";

            DataTable caTable = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, caSql).Tables[0];
            DataTable ndTable = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, ndSql).Tables[0];
            for (int i = 0; i < caTable.Rows.Count; i++)
            {
                ret.Add(Convert.ToInt32(caTable.Rows[i]["CustomAreaId"]), (caTable.Rows[i]["infoStr"]).ToString());
            }
            for (int i = 0; i < ndTable.Rows.Count; i++)
            {
                ret.Add(Convert.ToInt32(ndTable.Rows[i]["NdAddressId"]), (ndTable.Rows[i]["infoStr"]).ToString());
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
            string sql = "SELECT u.username,g.uniqueID FROM taskgtuinfomapping  m " +
                         "INNER JOIN gtus g on m.gtuId = g.id " +
                         "INNER JOIN users u on m.userid = u.id " +
                         "WHERE g.uniqueID= ?code";
            MySqlParameter[] paras = {                       
                        new MySqlParameter("?code",MySqlDbType.String),
            };
            paras[0].Value = uid;

            DataTable table = DbHelper.ExecuteDataset(DbHelper.ConnectionString, CommandType.Text, sql, paras).Tables[0];
            if (table.Rows.Count > 0)
            {
                return table.Rows[0]["username"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
