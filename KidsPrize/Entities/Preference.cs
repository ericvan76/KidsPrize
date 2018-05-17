using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Entities
{
    public class Preference
    {
        private Preference() { }

        public Preference(string userId, int timeZoneOffset)
        {
            UserId = userId;
            TimeZoneOffset = timeZoneOffset;
        }

        [Key]
        [MaxLength(250)]
        public string UserId { get; private set; }

        [Required]
        public int TimeZoneOffset { get; private set; }

        public void Update(int? timeZoneOffset)
        {
            TimeZoneOffset = timeZoneOffset ?? TimeZoneOffset;
        }

    }
}