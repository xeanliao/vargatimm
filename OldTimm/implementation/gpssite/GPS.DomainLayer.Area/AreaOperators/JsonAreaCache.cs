using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using GPS.DomainLayer.Enum;
using GPS.DataLayer;
using System.Web;
using GPS.DomainLayer.Entities;

namespace GPS.DomainLayer.Area
{
    public class JsonAreaCache
    {
        private static readonly string KEY_AREA = "AreaCacheManager";
        private static readonly string KEY_CAMPAIGN_AREA = "CampaignAreaCacheManager";

        #region Contains

        public static bool Contains(Classifications classification, int boxId)
        {
            string key = string.Format("{0}:{1}", classification, boxId);
            ICacheManager cacheManager = GetCache(KEY_AREA);
            return cacheManager.Contains(key);
        }

        public static bool Contains(Classifications classification, int boxId, int campaignId)
        {
            string campaignKey = string.Format("{0}", campaignId);
            string key = string.Format("{0}:{1}:{2}", classification, boxId, campaignId);
            ICacheManager cacheManager = GetCache(KEY_CAMPAIGN_AREA);
            if (!cacheManager.Contains(campaignKey))
            {
                object obj = new object();
                lock (obj)
                {
                    if (!cacheManager.Contains(campaignKey))
                    {
                        AddCampaingnBoxIds(cacheManager, campaignId);
                        cacheManager.Add(campaignKey, true);
                    }
                }
            }
            return cacheManager.Contains(key);
        }

        #region AddCampaingnBoxIds

        private static void AddCampaingnBoxIds(ICacheManager cacheManager, int campaignId)
        {
            using ( IWorkSpace ws = WorkSpaceManager.Instance.NewWorkSpace())
            {
                Campaign campaign = ws.Repositories.CampaignRepository.GetEntity(campaignId);
                if (campaign != null)
                {
                    foreach (var c in campaign.CampaignFiveZipImporteds)
                    {
                        IEnumerable<int> boxIds = c.FiveZipArea.FiveZipBoxMappings.Select(t => t.BoxId);
                        foreach (int boxId in boxIds)
                        {
                            string key = string.Format("{0}:{1}:{2}", Classifications.Z5, boxId, campaignId);
                            cacheManager.Add(key, null);
                        }
                    }

                    /*
                    int oldcount = 0;
                    DateTime oldstart = DateTime.MinValue;
                    DateTime oldend = DateTime.MinValue;

                    oldstart = DateTime.Now;
                    foreach (var c in campaign.CampaignCRouteImporteds)
                    {
                        IEnumerable<int> boxIds = c.PremiumCRoute.PremiumCRouteBoxMappings.Select(t => t.BoxId);
                        foreach (int boxId in boxIds)
                        {
                            string key = string.Format("{0}:{1}:{2}", Classifications.PremiumCRoute, boxId, campaignId);
                            //cacheManager.Add(key, null);
                            oldcount++;
                        }
                    }
                    oldend = DateTime.Now;
                     * */

                    // Jimmy added the following
                    //int newcount = 0;
                    //DateTime newstart = DateTime.MinValue;
                    //DateTime newend = DateTime.MinValue;
                    //newstart = DateTime.Now;
                    CampaignRepository camp = new CampaignRepository();
                    IList<int> boxIdList = camp.GetBoxIDs(campaignId);
                    foreach (int boxId in boxIdList)
                    {
                        string key = string.Format("{0}:{1}:{2}", Classifications.PremiumCRoute, boxId, campaignId);
                        cacheManager.Add(key, null);
                        //newcount++;
                    }
                    //newend = DateTime.Now;

                    foreach (var c in campaign.CampaignTractImporteds)
                    {
                        IEnumerable<int> boxIds = c.Tract.TractBoxMappings.Select(t => t.BoxId);
                        foreach (int boxId in boxIds)
                        {
                            string key = string.Format("{0}:{1}:{2}", Classifications.TRK, boxId, campaignId);
                            cacheManager.Add(key, null);
                        }
                    }

                    foreach (var c in campaign.CampaignBlockGroupImporteds)
                    {
                        IEnumerable<int> boxIds = c.BlockGroup.BlockGroupBoxMappings.Select(t => t.BoxId);
                        foreach (int boxId in boxIds)
                        {
                            string key = string.Format("{0}:{1}:{2}", Classifications.BG, boxId, campaignId);
                            cacheManager.Add(key, null);
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Add

        public static void Add(Classifications classification, int boxId, string value)
        {
            string key = string.Format("{0}:{1}", classification, boxId);
            ICacheManager cacheManager = GetCache(KEY_AREA);
            cacheManager.Add(key, value);
        }

        public static void Add(Classifications classification, int boxId, int campaignId, string value)
        {
            string key = string.Format("{0}:{1}:{2}", classification, boxId, campaignId);
            ICacheManager cacheManager = GetCache(KEY_CAMPAIGN_AREA);
            cacheManager.Add(key, value);
        }

        #endregion

        #region Get

        public static string Get(Classifications classification, int boxId)
        {
            string key = string.Format("{0}:{1}", classification, boxId);
            ICacheManager cacheManager = GetCache(KEY_AREA);
            if (cacheManager.GetData(key) == null)
            {
                return null;
            }
            else
            {
                return cacheManager.GetData(key).ToString();
            }
        }

        public static string Get(Classifications classification, int boxId, int campaignId)
        {
            string key = string.Format("{0}:{1}:{2}", classification, boxId, campaignId);
            ICacheManager cacheManager = GetCache(KEY_CAMPAIGN_AREA);
            if (cacheManager.GetData(key) == null)
            {
                return null;
            }
            else
            {
                return cacheManager.GetData(key).ToString();
            }
        }

        #endregion

        #region Clear

        public static void ClearAreas()
        {
            ICacheManager cacheManager = GetCache(KEY_AREA);
            cacheManager.Flush();
        }

        public static void ClearCampaignAreas()
        {
            ICacheManager cacheManager = GetCache(KEY_CAMPAIGN_AREA);
            cacheManager.Flush();
        }

        public static void ClearCampaignAreas(int campaignId)
        {
            ICacheManager cacheManager = GetCache(KEY_CAMPAIGN_AREA);
            cacheManager.Flush();
        }

        #endregion

        private static ICacheManager GetCache(string key)
        {
            return CacheFactory.GetCacheManager(key);
        }

        private static ICacheManager GetCampaignCache(int campaignId)
        {
            return GetCache(KEY_CAMPAIGN_AREA + campaignId.ToString());
        }
    }

    public class JsonAreaTempCache
    {
        private static readonly string KEY_TEMP_AREA = "TempArea";

        #region Contains

        public static bool Contains(Classifications classification, int boxId)
        {
            string key = string.Format("{0}:{1}", classification, boxId);
            Dictionary<string, string> cache = GetTempCache();
            return cache.ContainsKey(key);
        }

        public static bool Contains(Classifications classification, int boxId, int campaignId)
        {
            string key = string.Format("{0}:{1}:{2}", classification, boxId, campaignId);
            Dictionary<string, string> cache = GetTempCache();
            return cache.ContainsKey(key);
        }

        #endregion

        #region Add

        public static void Add(Classifications classification, int boxId, string value)
        {
            string key = string.Format("{0}:{1}", classification, boxId);
            Dictionary<string, string> cache = GetTempCache();
            if (cache.ContainsKey(key))
            {
                cache[key] = value;
            }
            else
            {
                cache.Add(key, value);
            }
        }

        public static void Add(Classifications classification, int boxId, int campaignId, string value)
        {
            string key = string.Format("{0}:{1}:{2}", classification, boxId, campaignId);
            Dictionary<string, string> cache = GetTempCache();
            if (cache.ContainsKey(key))
            {
                cache[key] = value;
            }
            else
            {
                cache.Add(key, value);
            }
        }

        #endregion

        #region AddKey

        public static void AddKey(Classifications classification, int boxId)
        {
            string key = string.Format("{0}:{1}", classification, boxId);
            Dictionary<string, string> cache = GetTempCache();
            if (cache.ContainsKey(key))
            {
                cache[key] = null;
            }
            else
            {
                cache.Add(key, null);
            }
        }

        public static void AddKeys(Classifications classification, IEnumerable<Int32> boxIds)
        {
            foreach (Int32 boxId in boxIds)
            {
                AddKey(classification, boxId);
            }
        }

        public static void AddKey(Classifications classification, int boxId, int campaignId)
        {
            string key = string.Format("{0}:{1}:{2}", classification, boxId, campaignId);
            Dictionary<string, string> cache = GetTempCache();
            if (cache.ContainsKey(key))
            {
                cache[key] = null;
            }
            else
            {
                cache.Add(key, null);
            }
        }

        #endregion

        #region Get

        public static string Get(Classifications classification, int boxId)
        {
            string key = string.Format("{0}:{1}", classification, boxId);
            Dictionary<string, string> cache = GetTempCache();
            return cache[key];
        }

        public static string Get(Classifications classification, int boxId, int campaignId)
        {
            string key = string.Format("{0}:{1}:{2}", classification, boxId, campaignId);
            Dictionary<string, string> cache = GetTempCache();
            return cache[key];
        }

        #endregion

        public static void Clear()
        {
            HttpContext.Current.Session.Remove(KEY_TEMP_AREA);
        }

        private static Dictionary<string, string> GetTempCache()
        {
            if (HttpContext.Current.Session[KEY_TEMP_AREA] == null)
            {
                HttpContext.Current.Session[KEY_TEMP_AREA] = new Dictionary<string, string>();
            }
            return HttpContext.Current.Session[KEY_TEMP_AREA] as Dictionary<string, string>;
        }
    }
}
