using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Abstractions;
using KidsPrize.Contracts.Commands;
using KidsPrize.Contracts.Models;
using KidsPrize.Http.Services;
using KidsPrize.Repository.Npgsql;
using Xunit;

namespace KidsPrize.Tests
{
    public class RedeemTests
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;
        private readonly IChildService _childService;
        private readonly IRedeemService _redeemService;
        private readonly IScoreService _scoreService;
        private readonly string _userId;

        public RedeemTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _childService = new ChildService(_context, _mapper);
            _scoreService = new ScoreService(_context, _mapper);
            _redeemService = new RedeemService(_context, _mapper);
            _userId = Guid.NewGuid().ToString();
        }

        [Fact]
        public async Task TestCreateRedeem()
        {
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _childService.CreateChild(_userId, createCommand);

            for (int i = 0; i < 100; i++)
            {
                var setScoreCommand = new SetScore()
                {
                    ChildId = createCommand.ChildId,
                    Date = DateTime.Today.AddDays(i),
                    Task = "Task A",
                    Value = 1
                };
                await _scoreService.SetScore(_userId, setScoreCommand);
            }

            var actual = await _childService.GetChild(_userId, createCommand.ChildId);

            Assert.Equal(100, actual.TotalScore);

            var expectRedeems = new List<Redeem>();
            for (int i = 1; i <= 20; i++)
            {
                var createRedeem = new CreateRedeem()
                {
                    ChildId = createCommand.ChildId,
                    Description = $"Icecream-{i}",
                    Value = 2
                };
                var result = await _redeemService.CreateRedeem(_userId, createRedeem);
                expectRedeems.Add(result);

                actual = await _childService.GetChild(_userId, createCommand.ChildId);

                Assert.Equal(100 - i * 2, actual.TotalScore);
            }

            var actualRedeems = new List<Redeem>();
            for (int i = 0; i < 99; i++)
            {
                var redeems = await _redeemService.GetRedeems(_userId, createCommand.ChildId, 5, i * 5);
                if (redeems.Count() == 0)
                {
                    break;
                }
                actualRedeems.AddRange(redeems);
            }

            Assert.Equal(expectRedeems, actualRedeems.OrderBy(i => i.Timestamp).ToList(), new RedeemComparer());
        }
    }

    class RedeemComparer : IEqualityComparer<Redeem>
    {
        public bool Equals(Redeem x, Redeem y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            if (x.Timestamp == y.Timestamp && x.Description == y.Description && x.Value == y.Value)
                return true;
            return false;
        }

        public int GetHashCode(Redeem obj)
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + obj.Timestamp.GetHashCode();
                hash = hash * 23 + obj.Description.GetHashCode();
                hash = hash * 23 + obj.Value.GetHashCode();
                return hash;
            }
        }
    }
}