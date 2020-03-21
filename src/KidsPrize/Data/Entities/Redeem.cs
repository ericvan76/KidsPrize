using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Data.Entities
{
    public class Redeem
    {
        private Redeem() { }

        public Redeem(Child child, DateTimeOffset timestamp, string description, int value)
        {
            Child = child;
            Timestamp = timestamp;
            Description = description;
            Value = value;
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        public virtual Child Child { get; private set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Timestamp { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; private set; }

        [Required]
        public int Value { get; private set; }

    }
}