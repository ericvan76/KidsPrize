using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Models
{
    public class Preference
    {
        private Preference() {}

        public Preference(Guid userId, int timeZoneOffset)
        {
            UserId = userId;
            TimeZoneOffset = timeZoneOffset;
        }

        [Key]
        public Guid UserId { get; private set; }

        [Required]
        public int TimeZoneOffset{ get; private set; }

        public void Update(int? timeZoneOffset)
        {
            TimeZoneOffset = timeZoneOffset ?? TimeZoneOffset;
        }

    }
}