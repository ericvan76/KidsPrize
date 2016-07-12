using System;
using System.Collections.Generic;
using System.Linq;

namespace KidsPrize.Resources
{
    public class DayScore
    {
        public DateTime Date { get; set; }
        public IEnumerable<Score> Scores { get; set; }
        public int DayTotal => Scores.Sum(s => s.Value);
    }
}