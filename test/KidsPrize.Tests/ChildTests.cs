using Xunit;
using KidsPrize.Services;
using AutoMapper;
using KidsPrize.Commands;
using System;
using KidsPrize.Bus;
using KidsPrize.Tests.Common;
using System.Security.Claims;
using KidsPrize.Extensions;

namespace KidsPrize.Tests
{
    public class ChildTests
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;
        private readonly IChildService _childService;
        private readonly IBus _bus;
        private readonly ClaimsPrincipal _user;

        public ChildTests()
        {
            _context = TestHelper.CreateContext();
            _mapper = TestHelper.CreateMapper();
            _childService = new ChildService(_context, _mapper);
            _user = TestHelper.CreateUser();
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
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            command.SetAuthorisation(_user);
            await _bus.Send(command);
            var actual = await _childService.GetChild(_user.UserId(), command.ChildId);
            Assert.Equal(command.Name, actual.Name);
            Assert.Equal(command.Gender, actual.Gender);
            Assert.Equal(0, actual.TotalScore);
        }

        [Fact]
        public async void TestUpdateChild()
        {
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            createCommand.SetAuthorisation(_user);
            await _bus.Send(createCommand);

            var updateCommand = new UpdateChild()
            {
                ChildId = createCommand.ChildId,
                Name = "New-Child-Name",
                Gender = "Female"
            };
            updateCommand.SetAuthorisation(_user);
            await _bus.Send(updateCommand);

            var actual = await _childService.GetChild(_user.UserId(), updateCommand.ChildId);
            Assert.Equal(updateCommand.Name, actual.Name);
            Assert.Equal(updateCommand.Gender, actual.Gender);
            Assert.Equal(0, actual.TotalScore);
        }

        [Fact]
        public async void TestDeleteChild()
        {
            var createCommand = new CreateChild()
            {
                ChildId = Guid.NewGuid(),
                Name = "Test-Child-Name",
                Gender = "Male"
            };
            createCommand.SetAuthorisation(_user);
            await _bus.Send(createCommand);

            var deleteCommand = new DeleteChild()
            {
                ChildId = createCommand.ChildId
            };
            deleteCommand.SetAuthorisation(_user);
            await _bus.Send(deleteCommand);

            var actual = await _childService.GetChild(_user.UserId(), deleteCommand.ChildId);
            Assert.Null(actual);
        }
    }
}
