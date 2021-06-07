using System;
using System.Collections.Generic;

namespace UnitTest.Entities
{
	public class Campaign
	{
	    public Campaign()
		{
			this.CampaignUserMappings = new List<CampaignUserMapping>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public virtual ICollection<CampaignUserMapping> CampaignUserMappings { get; set; }
	}
}

