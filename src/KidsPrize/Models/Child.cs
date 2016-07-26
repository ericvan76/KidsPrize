using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Models
{
    public class Child
    {
        private Child()
        {
        }

        public Child(Guid id, Guid userId, string name, string gender, int totalScore)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Gender = gender;
            TotalScore = totalScore;
        }

        [Key]
        public Guid Id { get; private set; }
        [Required]
        public Guid UserId { get; private set; }
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