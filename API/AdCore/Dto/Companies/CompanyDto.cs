using AdCore.Entity;
using AdCore.MapperProfile;

namespace AdCore.Dto.Companies
{
    public class CompanyDto : BaseDto, IMapFrom<Company>
    {
        public string Name { get; set; }
        public string LogoId { get; set; }
        public string LogoUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public Dictionary<string, string> Banners { get; set; }
        public List<string> Sellers { get; set; }
        public string MasterSellerAdId { get; set; }
        public List<string> AdList { get; set; }

        public string Country { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public string Thana { get; set; }
        public string Area { get; set; }
    }
}
