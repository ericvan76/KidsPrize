using System;
using System.Linq;
using AutoMapper;
using E = KidsPrize.Models;

namespace KidsPrize.Resources
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<E.Child, Child>();
            CreateMap<E.Day, DayScore>()
                .ForMember(d => d.Date, opt => opt.MapFrom(s => s.Date))
                .ForMember(d => d.Scores, opt => opt.ResolveUsing(s =>
                    s.Scores.OrderBy(i=>i.Position).ToDictionary(i => i.Task, i => i.Value, StringComparer.OrdinalIgnoreCase)));
        }
    }
}