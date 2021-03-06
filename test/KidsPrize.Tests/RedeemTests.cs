using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KidsPrize.Commands;
using KidsPrize.Models;
using KidsPrize.Services;
using KidsPrize.Data;
using Xunit;

namespace KidsPrize.Tests
{
    public class RedeemTests
    {
        private readonly KidsPrizeContext _context;
        private readonly IChildService _service;
        private readonly string _userId;

        public RedeemTests()
        {
            _context = TestHelper.CreateContext();
            _service = new ChildService(_context);
            _userId = Guid.NewGuid().ToString();
        }

        [Fact]
        public async Task TestCreateRedeem()
        {
            var createCommand = new CreateChildCommand()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male",
                Tasks = new[] { "Task A", "Task B", "Task C" }
            };

            await _service.CreateChild(_userId, createCommand, DateTime.Today);

            for (int i = 0; i < 100; i++)
            {
                var setScoreCommand = new SetScoreCommand()
                {
                    ChildId = createCommand.ChildId,
                    Date = DateTime.Today.AddDays(i),
                    Task = "Task A",
                    Value = 1
                };
                await _service.SetScore(_userId, setScoreCommand);
            }

            var actual = await _service.GetChild(_userId, createCommand.ChildId);

            Assert.Equal(100, actual.TotalScore);

            var expectRedeems = new List<Redeem>();
            for (int i = 1; i <= 20; i++)
            {
                var createRedeem = new CreateRedeemCommand()
                {
                    ChildId = createCommand.ChildId,
                    Description = $"Icecream-{i}",
                    Value = 2
                };
                var result = await _service.CreateRedeem(_userId, createRedeem);
                expectRedeems.Add(result);

                actual = await _service.GetChild(_userId, createCommand.ChildId);

                Assert.Equal(100 - i * 2, actual.TotalScore);
            }

            var actualRedeems = new List<Redeem>();
            for (int i = 0; i < 99; i++)
            {
                var redeems = await _service.GetRedeems(_userId, createCommand.ChildId, 5, i * 5);
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