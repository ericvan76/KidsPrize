using System;

namespace KidsPrize.Models
{
    public class Redeem
    {
        public DateTimeOffset Timestamp { get; set; }

        public string Description { get; set; }

        public int Value { get; set; }
    }
}