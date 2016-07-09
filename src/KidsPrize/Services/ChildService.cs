using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Resources;
using Microsoft.EntityFrameworkCore;
using E = KidsPrize.Models;

namespace KidsPrize.Services
{
    public interface IChildService
    {
        Task<Child> GetChild(Guid userUid, Guid childUid);
        Task<IEnumerable<Child>> GetChildren(Guid userUid);
    }

    public class ChildService : IChildService
    {
        private readonly E.KidsPrizeDbContext _context;
        private readonly IMapper _mapper;
        public ChildService(E.KidsPrizeDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<Child> GetChild(Guid userUid, Guid childUid)
        {
            var q = await _context.Users.Include(i => i.Children).FirstOrDefaultAsync(i => i.Uid == userUid);
            var child = q?.Children.FirstOrDefault(i => i.Uid == childUid);
            if (child != null)
            {
                return _mapper.Map<Child>(child);
            }
            return null;
        }

        public async Task<IEnumerable<Child>> GetChildren(Guid userUid)
        {
            var q = await _context.Users.Include(i => i.Children).FirstOrDefaultAsync(i => i.Uid == userUid);
            var result = new List<Child>();
            if (q != null)
            {
                foreach (var item in q.Children)
                {
                    result.Add(_mapper.Map<Child>(item));
                }
            }
            return result;
        }
    }
}