using AdCore.Entity;
using AdCore.MapperProfile;

namespace AdCore.Dto.Ads
{
    public class AdDto : BaseDto, IMapFrom<Ad>
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<AdFileDto> AdFiles { get; set; }
        public string SellerName { get; set; }
        public string SellerId { get; set; }
    }
}
