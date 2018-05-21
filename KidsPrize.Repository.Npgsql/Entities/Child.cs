using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Repository.Npgsql.Entities
{
    public class Child
    {
        private Child() { }

        public Child(Guid id, string userId, string name, string gender)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Gender = gender;
            TotalScore = 0;
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        [MaxLength(250)]
        public string UserId { get; private set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; private set; }

        [MaxLength(10)]
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