using System;
using KidsPrize.Models;

namespace KidsPrize.Extensions
{
    public static class PreferenceExtensions
    {
        public static DateTime Today(this Preference preference)
        {
            return DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromMinutes(preference.TimeZoneOffset)).Date;
        }
    }
}