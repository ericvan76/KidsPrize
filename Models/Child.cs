using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidsPrize.Models
{
    [Table(nameof(Child))]
    public class Child : Entity
    {
        private Child() : base()
        { }
        public Child(int id, Guid uid, string name, string gender, int totalScore) : base()
        {
            Id = id;
            Uid = uid;
            Name = name;
            Gender = gender;
            TotalScore = totalScore;
        }

        [Key]
        public int Id { get; private set; }
        [Required]
        public Guid Uid { get; private set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; private set; }
        [MaxLength(50)]
        public string Gender { get; private set; }
        [Required]
        public int TotalScore { get; private set; }

        public void Update(string name, string gender, int? totalScore)
        {
            Name = name ?? Name;
            Gender = gender ?? Gender;
            TotalScore = totalScore ?? TotalScore;
        }
    }

}