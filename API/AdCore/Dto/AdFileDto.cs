using AdCore.Entity;
using AdCore.Enums;
using AdCore.MapperProfile;

namespace AdCore.Dto
{
    public class AdFileDto : BaseDto, IMapFrom<AdFile>
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public FileTypes FileType { get; set; }
    }
}
