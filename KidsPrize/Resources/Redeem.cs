using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Resources
{
    public class Redeem
    {
        [DataType(DataType.DateTime)]
        public DateTimeOffset Timestamp { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
    }
}