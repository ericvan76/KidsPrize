using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Entities
{
    public class Score
    {
        private Score() { }

        public Score(Child child, DateTime date, string task, int value)
        {
            Child = child;
            Date = date;
            Task = task;
            Value = value;
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        public virtual Child Child { get; private set; }

        [RequiredAttribute]
        [DataType(DataType.Date)]
        public DateTime Date { get; private set; }

        [Required]
        [MaxLength(50)]
        public string Task { get; private set; }

        [Required]
        public int Value { get; private set; }

        public void Update(int value)
        {
            var delta = value - Value;
            this.Value = value;
            this.Child.Update(null, null, this.Child.TotalScore + delta);
        }
    }
}