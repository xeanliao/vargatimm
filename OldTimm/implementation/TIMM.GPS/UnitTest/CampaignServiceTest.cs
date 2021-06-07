using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using TIMM.GPS.Model;
using System.Net.Http.Formatting;

namespace UnitTest
{
    public class CampaignServiceTest
    {
        public void QueryTest()
        {
            using (HttpClient client = new HttpClient())
            {
                CampaignCriteria criteria = new CampaignCriteria { PageIndex = 0, PageSize = 10 };
                var result = client.PostAsync("http://localhost:5887/service/campaign", 
                    new ObjectContent<CampaignCriteria>(criteria, JsonMediaTypeFormatter.DefaultMediaType));
                result.Wait();
            }
        }
    }
}
