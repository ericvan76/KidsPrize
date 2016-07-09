using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidsPrize.Models
{
    [Table(nameof(Score))]
    public class Score : Entity
    {
        private Score() : base()
        { }
        public Score(int id, string task, int position, int value) : base()
        {
            Id = id;
            Task = task;
            Position = position;
            Value = value;
        }

        [Key]
        public int Id { get; private set; }
        [Required]
        [MaxLength(50)]
        public string Task { get; private set; }
        [Required]
        public int Position { get; private set; }
        [Required]
        public int Value { get; private set; }

        public void Update(int? value)
        {
            Value = value ?? Value;
        }

        public void SetPosition(int pos)
        {
            Position = pos;
        }
    }
}