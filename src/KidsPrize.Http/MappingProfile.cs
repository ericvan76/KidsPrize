using AutoMapper;
using E = KidsPrize.Repository.Npgsql.Entities;
using M = KidsPrize.Http.Models;

namespace KidsPrize.Http
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