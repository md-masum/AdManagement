using Newtonsoft.Json;

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
        [JsonProperty(PropertyName = "imageUrl")]
        public List<string> ImageUrl { get; set; }
        [JsonProperty(PropertyName = "videoUrl")]
        public List<string> VideoUrl { get; set; }
        [JsonProperty(PropertyName = "sellerName")]
        public string SellerName { get; set; }
        [JsonProperty(PropertyName = "sellerId")]
        public string SellerId { get; set; }
    }
}
