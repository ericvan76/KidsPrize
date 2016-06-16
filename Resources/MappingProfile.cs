using AutoMapper;
using E = KidsPrize.Models;

namespace KidsPrize.Resources
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<E.Child, Child>();
        }
    }

}