using AdCore.Entity;
using AdCore.MapperProfile;

namespace AdCore.Dto.Companies
{
    public class CompanyCreateOrUpdateDto : IMapFrom<CompanyDto>
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Country { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public string Thana { get; set; }
        public string Area { get; set; }
    }
}
