using System;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Services
{
    public interface IUserService
    {
        Task<User> GetUser(Guid uid);
        Task<User> CreateOrUpdateUser(User user);

    }

    public class UserService : IUserService
    {
        private readonly KidsPrizeDbContext _context;

        public UserService(KidsPrizeDbContext context)
        {
            this._context = context;
        }
        public async Task<User> GetUser(Guid uid)
        {
            return await _context.Users
                .Include(u => u.Identifiers)
                .Include(u => u.Children)
                .FirstOrDefaultAsync(u => u.Uid == uid);
        }

        public async Task<User> CreateOrUpdateUser(User user)
        {
            var identifier = user.Identifiers.First();
            var existingUser = await _context.Users.Include(u => u.Identifiers).FirstOrDefaultAsync(u => u.Identifiers.Any(i => i.Issuer == identifier.Issuer && i.Value == identifier.Value))
                ?? await _context.Users.Include(u => u.Identifiers).FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return await _context.Users.Include(u => u.Identifiers).FirstAsync(u => u.Uid == user.Uid);
            }
            else
            {
                existingUser.TryAddIdentifier(identifier);
                existingUser.Update(user.GivenName, user.FamilyName, user.DisplayName);
                await _context.SaveChangesAsync();
                return await _context.Users.Include(u => u.Identifiers).FirstAsync(u => u.Uid == existingUser.Uid);
            }

        }
    }
}
