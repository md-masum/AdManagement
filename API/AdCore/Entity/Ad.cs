﻿using Newtonsoft.Json;

namespace AdCore.Entity
{
    public class Ad : BaseEntity
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty(PropertyName = "endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "fileId")]
        public List<string> FileId { get; set; }

        [JsonProperty(PropertyName = "companyId")]
        public string CompanyId { get; set; }
    }
}
