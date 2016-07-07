using System;
using System.Collections.Generic;
using System.Linq;
using KidsPrize.Models;
using KidsPrize.Services;
using KidsPrize.Tests.Common;
using Xunit;

namespace KidsPrize.Tests
{
    public class UserServiceTests
    {
        private readonly KidsPrizeDbContext _context;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _context = TestHelper.CreateContext();
            _userService = new UserService(_context);
        }

        [Fact]
        public async void TestCreateUser()
        {
            var user = new User(0, Guid.NewGuid(), $"{Guid.NewGuid()}@user.com", "Test", "User", "TestUser",
                new List<Identifier>() { new Identifier(0, "test-issuer", Guid.NewGuid().ToString()) },
                new List<Child>());

            await this._userService.CreateOrUpdateUser(user);

            var actual = await this._userService.GetUser(user.Uid);
            Assert.Equal(user.Uid, actual.Uid);
            Assert.Equal(user.Email, actual.Email);
            Assert.Equal(user.GivenName, actual.GivenName);
            Assert.Equal(user.FamilyName, actual.FamilyName);
            Assert.Equal(user.DisplayName, actual.DisplayName);
            Assert.Equal(1, user.Identifiers.Count);
            Assert.Equal(user.Identifiers.First(), actual.Identifiers.First());
        }

        [Fact]
        public async void ShouldCreateUserWhenIdentifierAndEmailUnmatched()
        {
            var user1 = new User(0, Guid.NewGuid(), $"{Guid.NewGuid()}@user.com", "Test", "User", "TestUser",
                new List<Identifier>() { new Identifier(0, "test-issuer", Guid.NewGuid().ToString()) },
                new List<Child>());
            await this._userService.CreateOrUpdateUser(user1);
            var user2 = new User(0, Guid.NewGuid(), $"{Guid.NewGuid()}@user.com", "Test2", "User2", "TestUser2",
                new List<Identifier>() { new Identifier(0, "test-issuer", Guid.NewGuid().ToString()) },
                new List<Child>());
            await this._userService.CreateOrUpdateUser(user2);

            var actual1 = await this._userService.GetUser(user1.Uid);
            var actual2 = await this._userService.GetUser(user2.Uid);

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotEqual(actual1, actual2);
        }

        [Fact]
        public async void ShouldUpdateUserWhenIdentifierMatched()
        {
            var user1 = new User(0, Guid.NewGuid(), $"{Guid.NewGuid()}@user.com", "Test", "User", "TestUser",
                new List<Identifier>() { new Identifier(0, "test-issuer", Guid.NewGuid().ToString()) },
                new List<Child>());
            await this._userService.CreateOrUpdateUser(user1);
            var user2 = new User(0, Guid.NewGuid(), user1.Email, "Test2", "User2", "TestUser2",
                new List<Identifier>() { new Identifier(0, "test-issuer", user1.Identifiers.First().Value) },
                new List<Child>());
            await this._userService.CreateOrUpdateUser(user2);

            var actual1 = await this._userService.GetUser(user1.Uid);
            var actual2 = await this._userService.GetUser(user2.Uid);

            Assert.NotNull(actual1);
            Assert.Null(actual2);
            Assert.Equal(user2.GivenName, actual1.GivenName);
            Assert.Equal(user2.FamilyName, actual1.FamilyName);
            Assert.Equal(user2.DisplayName, actual1.DisplayName);
            Assert.Equal(1, actual1.Identifiers.Count);
        }

        [Fact]
        public async void ShouldUpdateUserWhenEmailMatched()
        {
            var user1 = new User(0, Guid.NewGuid(), $"{Guid.NewGuid()}@user.com", "Test", "User", "TestUser",
                new List<Identifier>() { new Identifier(0, "test-issuer", Guid.NewGuid().ToString()) },
                new List<Child>());
            await this._userService.CreateOrUpdateUser(user1);
            var user2 = new User(0, Guid.NewGuid(), user1.Email, "Test2", "User2", "TestUser2",
                new List<Identifier>() { new Identifier(0, "test-issuer", Guid.NewGuid().ToString()) },
                new List<Child>());
            await this._userService.CreateOrUpdateUser(user2);

            var actual1 = await this._userService.GetUser(user1.Uid);
            var actual2 = await this._userService.GetUser(user2.Uid);

            Assert.NotNull(actual1);
            Assert.Null(actual2);
            Assert.Equal(user2.GivenName, actual1.GivenName);
            Assert.Equal(user2.FamilyName, actual1.FamilyName);
            Assert.Equal(user2.DisplayName, actual1.DisplayName);
            Assert.Equal(2, actual1.Identifiers.Count);
        }
    }
}