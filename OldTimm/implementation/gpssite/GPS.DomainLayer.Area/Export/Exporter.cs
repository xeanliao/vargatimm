using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GPS.DomainLayer.Enum;
using System.Data;
using GPS.DataLayer;
using GPS.DomainLayer.Entities;
using GPS.DataLayer.ValueObjects;
using GPS.DomainLayer.QuerySpecifications;
using System.Data.SqlClient;
using log4net;

namespace GPS.DomainLayer.Area.Export
{
    public class Exporter
    {
        private static ILog m_Logger = LogManager.GetLogger(typeof(Exporter));
        public static Campaign campaignForExport;

        public Exporter(int campaignId)
        {
            CampaignRepository camRep = new CampaignRepository();
            campaignForExport = camRep.GetEntity(campaignId);
        }

        //ExportSourceSpecification<Five
        public DataTable GetExportDataTable(Classifications classification, Dictionary<Classifications, List<int>> areaIds, Dictionary<Classifications, List<int>> nonareaIds)
        {
            DataTable dt = null;
            switch (classification)
            {
                case Classifications.Z5:
                    dt = GetFiveZips(areaIds, nonareaIds);
                    break;
                case Classifications.TRK:
                    dt = GetTracts(areaIds, nonareaIds);
                    break;
                case Classifications.BG:
                    dt = GetBlockGroups(areaIds, nonareaIds);
                    break;
                case Classifications.PremiumCRoute:
                    dt = GetCRoutes(areaIds, nonareaIds);
                    break;
            }
            return dt.DefaultView.ToTable(true);
        }

        private DataTable GetFiveZips(Dictionary<Classifications, List<int>> areaIds, Dictionary<Classifications, List<int>> nonareaIds)
        {
            ExportSourceSpecification<FiveZipArea> specification = new ExportSourceSpecification<FiveZipArea>();
            if (areaIds.ContainsKey(Classifications.Z3)) specification.SetSelectedThreeZipIds(areaIds[Classifications.Z3]);
            if (areaIds.ContainsKey(Classifications.Z5)) specification.SetSelectedFiveZipIds(areaIds[Classifications.Z5]);
            if (nonareaIds.ContainsKey(Classifications.Z5)) specification.SetDeselectedFiveZipIds(nonareaIds[Classifications.Z5]);

            IEnumerable<FiveZipArea> items = specification.GetExportSource(new FiveZipRepository());
            DataTable dt = new DataTable();
            dt.Columns.Add("ZIP");
            dt.Columns.Add("TOTAL");
            dt.Columns.Add("PENETRATION");
            if (campaignForExport != null)
            {
                foreach (var item in items)
                {
                    DataRow row = dt.NewRow();
                    row[0] = item.Code;
                    var data = campaignForExport.CampaignFiveZipImporteds.Where(t => t.FiveZipArea.Id == item.Id).SingleOrDefault();
                    if (data != null)
                    {
                        //var per = data.Total > 0 ? (double)data.Penetration / (double)data.Total : 0;
                        var per = data.IsPartModified ? (double)data.Penetration * (double)data.PartPercentage : (double)data.Penetration;
                        row[2] = (int)Math.Round(per);
                        double total = data.IsPartModified ? (double)data.Total * (double)data.PartPercentage : (double)data.Total;
                        row[1] = (int)Math.Round(total);
                    }
                    else
                    {
                        row[2] = "";
                        row[1] = item.APT_COUNT + item.HOME_COUNT;
                    }
                    dt.Rows.Add(row);
                }
            }
            //foreach (var item in items)
            //{
            //    DataRow row = dt.NewRow();
            //    row[0] = item.Code;
            //    row[1] = item.OTotal;
            //    row[2] = item.Penetration;
            //    dt.Rows.Add(row);
            //}
            return dt;
        }

        private DataTable GetTracts(Dictionary<Classifications, List<int>> areaIds, Dictionary<Classifications, List<int>> nonareaIds)
        {
            ExportSourceSpecification<Tract> specification = new ExportSourceSpecification<Tract>();
            if (areaIds.ContainsKey(Classifications.Z3)) specification.SetSelectedThreeZipIds(areaIds[Classifications.Z3]);
            if (areaIds.ContainsKey(Classifications.Z5)) specification.SetSelectedFiveZipIds(areaIds[Classifications.Z5]);
            if (nonareaIds.ContainsKey(Classifications.Z5)) specification.SetDeselectedFiveZipIds(nonareaIds[Classifications.Z5]);
            if (areaIds.ContainsKey(Classifications.TRK)) specification.SetSelectedTractIds(areaIds[Classifications.TRK]);
            if (nonareaIds.ContainsKey(Classifications.TRK)) specification.SetDeselectedTractIds(nonareaIds[Classifications.TRK]);
            IEnumerable<Tract> items = specification.GetExportSource(new TractRepository());
            DataTable dt = new DataTable();
            dt.Columns.Add("CNSTT TRACKS");
            dt.Columns.Add("TOTAL");
            dt.Columns.Add("PENETRATION");
            if (campaignForExport != null)
            {
                foreach (var item in items)
                {
                    DataRow row = dt.NewRow();
                    row[0] = item.Code;
                    var data = campaignForExport.CampaignTractImporteds.Where(t => t.Tract.Id == item.Id).SingleOrDefault();
                    if (data != null)
                    {
                        row[2] = data.Penetration;
                        row[1] = data.Total;
                    }
                    else
                    {
                        row[1] = "";
                        row[2] = "";
                    }
                    dt.Rows.Add(row);
                }
            }
            //foreach (var item in items)
            //{
            //    DataRow row = dt.NewRow();
            //    //row[0] = string.Format("{0}{1}{2}", item.StateCode.PadLeft(2, '0'), item.CountyCode.PadLeft(3, '0'), item.Code.PadRight(6, '0'));
            //    row[0] = item.ArbitraryUniqueCode;
            //    row[1] = item.OTotal;
            //    row[2] = item.Penetration;
            //    dt.Rows.Add(row);
            //}
            return dt;
        }
        private DataTable GetBlockGroups(Dictionary<Classifications, List<int>> areaIds, Dictionary<Classifications, List<int>> nonareaIds)
        {
            ExportSourceSpecification<BlockGroup> specification = new ExportSourceSpecification<BlockGroup>();
            if (areaIds.ContainsKey(Classifications.Z3)) specification.SetSelectedThreeZipIds(areaIds[Classifications.Z3]);
            if (areaIds.ContainsKey(Classifications.Z5)) specification.SetSelectedFiveZipIds(areaIds[Classifications.Z5]);
            if (nonareaIds.ContainsKey(Classifications.Z5)) specification.SetDeselectedFiveZipIds(nonareaIds[Classifications.Z5]);
            if (areaIds.ContainsKey(Classifications.TRK)) specification.SetSelectedTractIds(areaIds[Classifications.TRK]);
            if (nonareaIds.ContainsKey(Classifications.TRK)) specification.SetDeselectedTractIds(nonareaIds[Classifications.TRK]);
            if (areaIds.ContainsKey(Classifications.BG)) specification.SetSelectedBlockGroupIds(areaIds[Classifications.BG]);
            if (nonareaIds.ContainsKey(Classifications.BG)) specification.SetDeselectedBlockGroupIds(nonareaIds[Classifications.BG]);
            IEnumerable<BlockGroup> items = specification.GetExportSource(new BlockGroupRepository());
            DataTable dt = new DataTable();
            dt.Columns.Add("CTBKG BLOCK GROUPS");
            dt.Columns.Add("TOTAL");
            dt.Columns.Add("PENETRATION");
            if (campaignForExport != null)
            {
                foreach (var item in items)
                {
                    DataRow row = dt.NewRow();
                    row[0] = item.Code;
                    var data = campaignForExport.CampaignBlockGroupImporteds.Where(t => t.BlockGroup.Id == item.Id).SingleOrDefault();
                    if (data != null)
                    {
                        row[2] = data.Penetration;
                        row[1] = data.Total;
                    }
                    else
                    {
                        row[1] = "";
                        row[2] = "";
                    }
                    dt.Rows.Add(row);
                }
            }
            //foreach (var item in items)
            //{
            //    DataRow row = dt.NewRow();
            //    //row[0] = string.Format("{0}{1}{2}{3}", item.StateCode.PadLeft(2, '0'), item.CountyCode.PadLeft(3, '0'), item.TractCode.PadRight(6, '0'), item.Code);
            //    row[0] = item.ArbitraryUniqueCode;
            //    row[1] = item.OTotal;
            //    row[2] = item.Penetration;
            //    dt.Rows.Add(row);
            //}
            return dt;
        }

        private DataTable GetCRoutes(Dictionary<Classifications, List<int>> areaIds, Dictionary<Classifications, List<int>> nonareaIds)
        {
            m_Logger.DebugFormat("begin GetCRoutes, compaign id : {0}", campaignForExport.Id);
            ExportSourceSpecification<PremiumCRoute> specification = new ExportSourceSpecification<PremiumCRoute>();
            if (areaIds.ContainsKey(Classifications.Z3)) specification.SetSelectedThreeZipIds(areaIds[Classifications.Z3]);
            if (areaIds.ContainsKey(Classifications.Z5)) specification.SetSelectedFiveZipIds(areaIds[Classifications.Z5]);
            if (nonareaIds.ContainsKey(Classifications.Z5)) specification.SetDeselectedFiveZipIds(nonareaIds[Classifications.Z5]);
            if (areaIds.ContainsKey(Classifications.PremiumCRoute)) specification.SetSelectedCRouteIds(areaIds[Classifications.PremiumCRoute]);
            if (nonareaIds.ContainsKey(Classifications.PremiumCRoute)) specification.SetDeselectedCRouteIds(nonareaIds[Classifications.PremiumCRoute]);
            IEnumerable<PremiumCRoute> items = specification.GetExportSource(new PremiumCRouteRepository());
            DataTable dt = new DataTable();
            dt.Columns.Add("PREMIUMCROUTE");
            dt.Columns.Add("APT");
            dt.Columns.Add("HOME");
            dt.Columns.Add("TOTAL");
            dt.Columns.Add("PENETRATION");

            if (campaignForExport != null)
            {
                string condition = string.Empty;
                foreach (var item in items)
                {
                    condition += string.Format(@" final_geocode = '{0}' OR", item.Code);
                }

                if (!string.IsNullOrEmpty(condition))
                {
                    condition = condition.Substring(0, condition.Length - 3); // remove 'AND'
                    condition = " WHERE " + condition;
                }

                var connString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["TIMMContext"];
                // get the origin croute
                using (var conn = new SqlConnection(connString.ConnectionString))
                {
                    string query = string.Format(
                        @"SELECT part_geocode, SUM(part_apt), SUM(part_home) 
                            FROM premiumcrouteparts 
                            WHERE part_geocode in (SELECT DISTINCT (part_geocode) FROM premiumcrouteparts  {0})
                            GROUP BY part_geocode", condition);
                    m_Logger.DebugFormat("Execute query : {0}", query);
                    var cmd = new SqlCommand(query, conn);
                    cmd.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();

                        for (int i = 0; i < 3; i++)
                        {
                            row[i] = reader[i];
                        }

                        int typedTotal;
                        switch (campaignForExport.AreaDescription)
                        {
                            case "APT ONLY":
                                typedTotal = System.Convert.ToInt32(row[1]);
                                break;
                            case "HOME ONLY":
                                typedTotal = System.Convert.ToInt32(row[2]);
                                break;
                            case "APT + HOME":
                            default:
                                typedTotal = System.Convert.ToInt32(row[1]) + System.Convert.ToInt32(row[2]);
                                //result.Count += 0;
                                break;
                        }

                        row[3] = typedTotal;

                        dt.Rows.Add(row);
                    }
                }

                //foreach (var item in items)
                //{
                //    DataRow row = dt.NewRow();
                //    row[0] = item.Code;
                //    var data = campaignForExport.CampaignCRouteImporteds.Where(t => t.PremiumCRoute.Id == item.Id).SingleOrDefault();
                //    if (data != null)
                //    {
                //        //var per = data.Total > 0 ? (double)data.Penetration / (double)data.Total : 0;
                //        var per = data.IsPartModified ? (double)data.Penetration * (double)data.PartPercentage : (double)data.Penetration;
                //        row[2] = (int)Math.Round(per);
                //        double total = data.IsPartModified ? (double)data.Total * (double)data.PartPercentage : (double)data.Total;
                //        row[1] = (int)Math.Round(total);
                //    }
                //    else
                //    {
                //        row[2] = "";
                //        row[1] = item.APT_COUNT + item.HOME_COUNT;
                //    }
                //    dt.Rows.Add(row);
                //}
            }

            //foreach (var item in items)
            //{
            //    DataRow row = dt.NewRow();
            //    //row[0] = item.ZIP + item.CROUTE;
            //    row[0] = item.Code;
            //    row[1] = item.OTotal;
            //    row[2] = "";
            //    dt.Rows.Add(row);
            //}
            return dt;
        }
    }
}
