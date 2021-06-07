using System;
using System.Collections.Generic;
using System.Text;
using GPS.DomainLayer.Enum;
using Newtonsoft.Json;
using GPS.DomainLayer.Interfaces;

namespace GPS.DomainLayer.Entities
{
    public class CampaignRecord : ICampaignRecords
    {
        private const string FORMAT = "{0}$06${1}";

        public CampaignRecord()
        {
        }
        public virtual int AreaId
        {
            get;
            set;
        }
        public virtual int Classification
        {
            get;
            set;
        }
        public virtual int Id
        {
            get;
            set;
        }
        public virtual bool Value
        {
            get;
            set;
        }

        #region parent

        public virtual int CampaignId
        {
            get{return Campaign.Id;}
            set { Campaign.Id = value; }
        }
        public virtual Campaign Campaign{get;set;}

        #endregion

        #region ICampaignRecords Members

        private List<List<string>> _relation;

        public virtual string ClientAreaId
        {
            get
            {
                return String.Format(FORMAT, Classification.ToString(), AreaId);
            }
            set { }
        }

        public virtual List<List<string>> Relation
        {
            get
            {
                _relation = new List<List<string>>();
                if (this.threeZipIds != null && this.threeZipIds.Count > 0)
                    _relation.Add(CalculateIds(this.threeZipIds, Classifications.Z3));
                if (this.fiveZipIds != null && this.fiveZipIds.Count > 0)
                    _relation.Add(CalculateIds(this.fiveZipIds, Classifications.Z5));
                if (this.tractId != 0)
                    _relation.Add(CalculateIds(new List<int> { this.tractId }, Classifications.Z5));

                return _relation;
            }
            set { }
        }

        #endregion

        [JsonIgnore]
        public virtual int tractId { get; set; }

        /// <summary>
        /// the parent id
        /// </summary>
        [JsonIgnore]
        public virtual List<int> fiveZipIds { get; set; }

        /// <summary>
        /// the parent id
        /// </summary>
        [JsonIgnore]
        public virtual List<int> threeZipIds { get; set; }

        public static List<string> CalculateIds(List<int> ids, Classifications classification)
        {
            List<string> areaIds = new List<string>();
            foreach (int id in ids)
            {
                areaIds.Add(String.Format(FORMAT, ((int)classification).ToString(), id));
            }

            return areaIds;
        }
    }
}
