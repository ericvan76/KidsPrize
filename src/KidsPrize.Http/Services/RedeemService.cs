using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Http.Commands;
using KidsPrize.Http.Models;
using KidsPrize.Repository.Npgsql;
using Microsoft.EntityFrameworkCore;
using E = KidsPrize.Repository.Npgsql.Entities;

namespace KidsPrize.Http.Services
{
    public interface IRedeemService
    {
        Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset);
        Task<Redeem> CreateRedeem(string userId, CreateRedeemCommand command);
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

        public async Task<Redeem> CreateRedeem(string userId, CreateRedeemCommand command)
        {
            // Ensure the child blongs to current user
            var child = await this._context.GetChildOrThrow(userId, command.ChildId);
            var redeem = new E.Redeem(child, DateTimeOffset.Now, command.Description, command.Value);

            child.Update(null, null, child.TotalScore - command.Value);
            this._context.Redeems.Add(redeem);
            await _context.SaveChangesAsync();

            return _mapper.Map<Redeem>(redeem);
        }

        public async Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset)
        {
            var query = await this._context.Redeems.Where(i => i.Child.Id == childId).OrderByDescending(i => i.Timestamp)
                .Skip(offset).Take(limit).ToListAsync();
            return query.Select(i => _mapper.Map<Redeem>(i));
        }
    }
}