using KidsPrize.Data;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Tests
{
    public static class TestHelper
    {
        public static KidsPrizeContext CreateContext()
        {
            var opts = new DbContextOptionsBuilder<KidsPrizeContext>();
            opts.UseInMemoryDatabase("test");
            return new KidsPrizeContext(opts.Options);
        }
    }
}