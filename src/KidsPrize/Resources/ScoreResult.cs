using System.Collections.Generic;

namespace KidsPrize.Resources
{
    public class ScoreResult
    {
        public Child Child { get; set; }
        public IEnumerable<Score> Scores { get; set; }
        public IEnumerable<TaskGroup> TaskGroups { get; set; }
    }
}