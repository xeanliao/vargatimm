using System;
using System.Collections.Generic;

namespace UnitTest.Entities
{
	public class CampaignUserMapping
	{
		public int Id { get; set; }
		public Nullable<int> CampaignId { get; set; }
		public Nullable<int> UserId { get; set; }
		public Nullable<int> Status { get; set; }
		public virtual Campaign Campaign { get; set; }
		public virtual CampaignUserMapping CampaignUserMapping1 { get; set; }
		public virtual CampaignUserMapping CampaignUserMapping2 { get; set; }
		public virtual User User { get; set; }
	}
}

