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
        Task<Child> GetChild(Guid childUid);
        Task<IEnumerable<Child>> GetChildren();
    }

    public class ChildService : IChildService
    {
        private readonly E.KidsPrizeDbContext dbContext;
        private readonly IMapper mapper;
        public ChildService(E.KidsPrizeDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<Child> GetChild(Guid childUid)
        {
            var q = await dbContext.Users.Include(i => i.Children).FirstOrDefaultAsync(i => i.Uid == Guid.Empty);
            var child = q?.Children.FirstOrDefault(i => i.Uid == childUid);
            if (child != null)
            {
                return mapper.Map<Child>(child);
            }
            return null;
        }

        public async Task<IEnumerable<Child>> GetChildren()
        {
            var q = await dbContext.Users.Include(i => i.Children).FirstOrDefaultAsync(i => i.Uid == Guid.Empty);
            var result = new List<Child>();
            if (q != null)
            {
                foreach (var item in q.Children)
                {
                    result.Add(mapper.Map<Child>(item));
                }
            }
            return result;
        }
    }
}