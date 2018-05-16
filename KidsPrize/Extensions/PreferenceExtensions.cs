using System;
using KidsPrize.Entities;

namespace KidsPrize.Extensions
{
    public static class PreferenceExtensions
    {
        public static DateTime Today(this Preference preference)
        {
            return DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromMinutes(-1 * preference.TimeZoneOffset)).Date;
        }
    }
}