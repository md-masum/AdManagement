using AdCore.Enums;
using Newtonsoft.Json;

namespace AdCore.Entity
{
    public class AdFile : BaseEntity
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "fileType")]
        public FileTypes FileType { get; set; }
    }
}
