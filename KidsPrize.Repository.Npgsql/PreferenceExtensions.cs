using System;
using KidsPrize.Repository.Npgsql.Entities;

namespace KidsPrize.Repository.Npgsql
{
    public static class PreferenceExtensions
    {
        public static DateTime Today(this Preference preference)
        {
            return DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromMinutes(-1 * preference.TimeZoneOffset)).Date;
        }
    }
}