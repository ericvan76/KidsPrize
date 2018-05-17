using AutoMapper;
using E = KidsPrize.Entities;
using M = KidsPrize.Models;

namespace KidsPrize
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<E.Child, M.Child>();
            CreateMap<E.Score, M.Score>();
            CreateMap<E.Preference, M.Preference>();
            CreateMap<E.Redeem, M.Redeem>();
        }
    }
}