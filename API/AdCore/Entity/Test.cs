using Newtonsoft.Json;

namespace AdCore.Entity
{
    public class Test : BaseEntity
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
