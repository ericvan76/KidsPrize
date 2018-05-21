using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KidsPrize.Contracts.Commands;
using KidsPrize.Contracts.Models;

namespace KidsPrize.Abstractions
{
    public interface IRedeemService
    {
        Task<IEnumerable<Redeem>> GetRedeems(string userId, Guid childId, int limit, int offset);
        Task<Redeem> CreateRedeem(string userId, CreateRedeem command);
    }
}