using AutoMapper;
using E = KidsPrize.Models;
using R = KidsPrize.Resources;

namespace KidsPrize
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<E.Child, R.Child>();
            CreateMap<E.Score, R.Score>();
            CreateMap<E.Preference, R.Preference>();
            CreateMap<E.Redeem, R.Redeem>();
        }
    }
}