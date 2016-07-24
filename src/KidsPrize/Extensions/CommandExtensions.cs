using System;
using System.Security.Claims;
using KidsPrize.Bus;

namespace KidsPrize.Extensions
{
    public static class CommandExtensions
    {
        public static Guid UserId(this Command command)
        {
            var user = command.GetHeader<ClaimsPrincipal>("Authorisation");
            return user?.UserId() ?? Guid.Empty;
        }

    }
}