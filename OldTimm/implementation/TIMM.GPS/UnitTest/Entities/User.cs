using System;
using System.Collections.Generic;

namespace UnitTest.Entities
{
	public class User
	{
	    public User()
		{
			this.CampaignUserMappings = new List<CampaignUserMapping>();
		}

		public int Id { get; set; }
		public string Code { get; set; }
		public virtual ICollection<CampaignUserMapping> CampaignUserMappings { get; set; }
	}
}

