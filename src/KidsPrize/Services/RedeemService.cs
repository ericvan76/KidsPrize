using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Data;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;
using E = KidsPrize.Data.Entities;

namespace KidsPrize.Services
{
    public interface IRedeemService
    {
        Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset);
        Task<Redeem> CreateRedeem(string userId, CreateRedeemCommand command);
    }

    public class RedeemService : IRedeemService
    {
        private readonly KidsPrizeContext _context;

        public RedeemService(KidsPrizeContext context)
        {
            _context = context;
        }

        public async Task<Redeem> CreateRedeem(string userId, CreateRedeemCommand command)
        {
            // Ensure the child blongs to current user
            var child = await this._context.GetChildOrThrow(userId, command.ChildId);
            var redeem = new E.Redeem(child, DateTimeOffset.Now, command.Description, command.Value);

            child.Update(null, null, child.TotalScore - command.Value);
            this._context.Redeems.Add(redeem);
            await _context.SaveChangesAsync();

            return new Redeem
            {
                Timestamp = redeem.Timestamp,
                Description = redeem.Description,
                Value = redeem.Value
            };
        }

        public async Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset)
        {
            var query = await this._context.Redeems.Where(i => i.Child.Id == childId).OrderByDescending(i => i.Timestamp)
                .Skip(offset).Take(limit).ToListAsync();
            return query.Select(i => new Redeem
            {
                Timestamp = i.Timestamp,
                Description = i.Description,
                Value = i.Value
            });
        }
    }
}