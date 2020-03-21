using System;
using KidsPrize.Data.Entities;

namespace KidsPrize
{
    public static class PreferenceExtensions
    {
        public static DateTime Today(this Preference preference)
        {
            return DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromMinutes(-1 * preference.TimeZoneOffset)).Date;
        }
    }
}