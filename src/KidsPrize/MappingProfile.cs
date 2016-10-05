using System.Linq;
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
            CreateMap<E.TaskGroup, R.TaskGroup>()
                .ForMember(d => d.Tasks, opt => opt.ResolveUsing(s => s.Tasks.OrderBy(t => t.Order).Select(t => t.Name)));
            CreateMap<E.Preference, R.Preference>();
        }
    }
}