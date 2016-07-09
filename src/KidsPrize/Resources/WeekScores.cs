using System;
using System.Collections.Generic;
using System.Linq;

namespace KidsPrize.Resources
{
    public class WeekScores
    {
        public Guid ChildUid { get; set; }
        public int ChildTotal { get; set; }
        public IEnumerable<DayScore> DayScores { get; set; }
        public IEnumerable<string> Tasks => DayScores.SelectMany(ds => ds.Scores.Keys).Distinct(StringComparer.OrdinalIgnoreCase);
        public int WeekTotal => DayScores.Sum(s => s.DayTotal);
    }
}