using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KidsPrize.Resources
{
    public class DayScore
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public IDictionary<string, int> Scores { get; set; }
        public int DayTotal => Scores.Sum(s => s.Value);
    }
}