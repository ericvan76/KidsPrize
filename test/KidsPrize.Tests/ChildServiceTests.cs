using Xunit;
using KidsPrize.Models;
using KidsPrize.Services;
using AutoMapper;
using KidsPrize.Commands;
using System;
using KidsPrize.Bus;
using KidsPrize.Tests.Common;

namespace KidsPrize.Tests
{
    public class ChildServiceTests
    {
        private readonly KidsPrizeDbContext _context;
        private readonly IMapper _mapper;
        private readonly IChildService _childService;
        private readonly UserInfo _user;

        public ChildServiceTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _childService = new ChildService(_context, _mapper);
            _user = TestHelper.CreateUser(_context);
        }

        [Fact]
        public async void TestCreateChild()
        {

            var bus = new TestBus(_user, new CreateChildHandler(_context));

            var command = new CreateChild() {
                ChildUid = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            await bus.Send(command);
            var actual = await _childService.GetChild(_user.Uid, command.ChildUid);
            Assert.Equal(command.Name, actual.Name);
            Assert.Equal(command.Gender, actual.Gender);
            Assert.Equal(0, actual.TotalScore);
        }
    }
}
