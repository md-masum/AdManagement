using Newtonsoft.Json;

namespace AdCore.Entity
{
    public class User : BaseEntity
    {
        [JsonProperty(PropertyName = "adB2CId")]
        public string AdB2CId { get; set; }

        #region Seller for only
        [JsonProperty(PropertyName = "companyId")]
        public string CompanyId { get; set; }
        #endregion


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
