using AdCore.Entity;
using AdCore.MapperProfile;

namespace AdCore.Dto
{
    public class TestDto : BaseDto, IMapFrom<Test>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
