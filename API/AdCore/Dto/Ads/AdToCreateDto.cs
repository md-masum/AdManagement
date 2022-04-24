using AdCore.Entity;
using AdCore.MapperProfile;

namespace AdCore.Dto.Ads
{
    public class AdToCreateDto : BaseDto, IMapFrom<Ad>
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
