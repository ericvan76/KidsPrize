using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KidsPrize.Contracts.Commands;
using KidsPrize.Contracts.Models;

namespace KidsPrize.Abstractions
{
    public interface IChildService
    {
        Task<Child> GetChild(string userId, Guid childId);
        Task<IEnumerable<Child>> GetChildren(string userId);
        Task<Child> CreateChild(string userId, CreateChild command);
        Task<Child> UpdateChild(string userId, UpdateChild command);
        Task DeleteChild(string userId, Guid childId);
    }
}