using Newtonsoft.Json;

namespace AdCore.Entity
{
    public class Company :BaseEntity
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "logoId")]
        public string LogoId { get; set; }

        [JsonProperty(PropertyName = "logoUrl")]
        public string LogoUrl { get; set; }

        [JsonProperty(PropertyName = "shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty(PropertyName = "longDescription")]
        public string LongDescription { get; set; }

        [JsonProperty(PropertyName = "banners")]
        public Dictionary<string, string> Banners { get; set; }

        [JsonProperty(PropertyName = "sellers")]
        public List<string> Sellers { get; set; }

        [JsonProperty(PropertyName = "masterSellerAdId")]
        public string MasterSellerAdId { get; set; }



        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "division")]
        public string Division { get; set; }

        [JsonProperty(PropertyName = "district")]
        public string District { get; set; }

        [JsonProperty(PropertyName = "thana")]
        public string Thana { get; set; }

        [JsonProperty(PropertyName = "area")]
        public string Area { get; set; }
    }
}
