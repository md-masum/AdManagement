using AutoMapper;

namespace AdCore.MapperProfile
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile)
        {
            profile.CreateMap(typeof(T), GetType()).ReverseMap();
        }
    }
}