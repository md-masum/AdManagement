using AdCore.Entity;
using AdCore.MapperProfile;

namespace AdCore.Dto
{
    public class AdDto : BaseDto, IMapFrom<Ad>
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<string> ImageUrl { get; set; }
        public List<string> VideoUrl { get; set; }
        public string SellerName { get; set; }
        public string SellerId { get; set; }
    }
}
