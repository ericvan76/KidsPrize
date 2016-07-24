using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Resources;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Services
{
    public interface IChildService
    {
        Task<Child> GetChild(Guid userId, Guid childId);
        Task<IEnumerable<Child>> GetChildren(Guid userId);
    }

    public class ChildService : IChildService
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;
        public ChildService(KidsPrizeContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<Child> GetChild(Guid userId, Guid childId)
        {
            var child = await _context.Children.FirstOrDefaultAsync(i => i.UserId == userId && i.Id == childId);
            if (child != null)
            {
                return _mapper.Map<Child>(child);
            }
            return null;
        }

        public async Task<IEnumerable<Child>> GetChildren(Guid userId)
        {
            var result = new List<Child>();
            var children = await _context.Children.Where(i => i.UserId == userId).ToListAsync();
            children.ForEach(i => result.Add(_mapper.Map<Child>(i)));
            return result;
        }
    }
}