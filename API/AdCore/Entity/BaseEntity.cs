using Newtonsoft.Json;

namespace AdCore.Entity
{
    public class BaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updateBy")]
        public string UpdateBy { get; set; }

        [JsonProperty(PropertyName = "updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty(PropertyName = "type")] 
        public string Type { get; set; }
    }
}
