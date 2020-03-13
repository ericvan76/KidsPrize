using System;

namespace KidsPrize.Models
{
    public class Child
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public int TotalScore { get; set; }
    }
}