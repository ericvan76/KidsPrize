using System.Threading.Tasks;
using KidsPrize.Extensions;
using KidsPrize.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KidsPrize.Commands
{
    public class SetPreference : Command, IAsyncRequest
    {
        public int? TimeZoneOffset { get; set; }
    }

    public class SetPreferenceHandler : AsyncRequestHandler<SetPreference>
    {
        private readonly KidsPrizeContext _context;

        public SetPreferenceHandler(KidsPrizeContext context)
        {
            this._context = context;
        }

        protected override async Task HandleCore(SetPreference message)
        {
            var preference = await this._context.Preferences.FirstOrDefaultAsync(p => p.UserId == message.UserId());
            if (preference == null)
            {
                preference = new Preference(message.UserId(), 0);
                this._context.Preferences.Add(preference);
            }
            preference.Update(message.TimeZoneOffset);
            await this._context.SaveChangesAsync();
        }
    }
}