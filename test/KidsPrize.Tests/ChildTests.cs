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
    public class ChildTests
    {
        private readonly KidsPrizeDbContext _context;
        private readonly IMapper _mapper;
        private readonly IChildService _childService;
        private readonly UserInfo _user;
        private readonly IBus _bus;

        public ChildTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _childService = new ChildService(_context, _mapper);
            _user = TestHelper.CreateUser(_context);
            _bus = new TestBus(_user,
                new CreateChildHandler(_context),
                new UpdateChildHandler(_context),
                new DeleteChildHandler(_context)
            );
        }

        [Fact]
        public async void TestCreateChild()
        {
            var command = new CreateChild()
            {
                ChildUid = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            await _bus.Send(command);
            var actual = await _childService.GetChild(_user.Uid, command.ChildUid);
            Assert.Equal(command.Name, actual.Name);
            Assert.Equal(command.Gender, actual.Gender);
            Assert.Equal(0, actual.TotalScore);
        }

        [Fact]
        public async void TestUpdateChild()
        {
            var createCommand = new CreateChild()
            {
                ChildUid = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            await _bus.Send(createCommand);

            var updateCommand = new UpdateChild()
            {
                ChildUid = createCommand.ChildUid,
                Name = "New-Child-Name",
                Gender = "Female"
            };
            await _bus.Send(updateCommand);

            var actual = await _childService.GetChild(_user.Uid, updateCommand.ChildUid);
            Assert.Equal(updateCommand.Name, actual.Name);
            Assert.Equal(updateCommand.Gender, actual.Gender);
            Assert.Equal(0, actual.TotalScore);
        }

        [Fact]
        public async void TestDeleteChild()
        {
            var createCommand = new CreateChild()
            {
                ChildUid = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            await _bus.Send(createCommand);

            var deleteCommand = new DeleteChild()
            {
                ChildUid = createCommand.ChildUid
            };
            await _bus.Send(deleteCommand);

            var actual = await _childService.GetChild(_user.Uid, deleteCommand.ChildUid);
            Assert.Null(actual);
        }
    }
}
