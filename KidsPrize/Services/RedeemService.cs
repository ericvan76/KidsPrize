using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Resources;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Services
{
    public interface IRedeemService
    {
        Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset);
    }

    public class RedeemService : IRedeemService
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;

        public RedeemService(KidsPrizeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset)
        {
            var query = await this._context.Redeems.Where(i => i.Child.Id == childId).OrderByDescending(i => i.Timestamp)
                .Skip(offset).Take(limit).ToListAsync();
            return query.Select(i => _mapper.Map<Redeem>(i));
        }
    }
}