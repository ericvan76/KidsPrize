using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KidsPrize.Models
{
    [Table(nameof(Day))]
    public class Day
    {
        private Day()
        {
        }

        public Day(int id, Child child, DateTime date, string[] tasks)
        {
            Id = id;
            Child = child;
            Date = date.Date;
            int pos = 0;
            Scores = new HashSet<Score>(tasks.Select(t => new Score(0, t, pos++, 0)));
        }

        [Key]
        public int Id { get; private set; }
        [Required]
        public Child Child { get; private set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; private set; }
        public ICollection<Score> Scores { get; private set; }
        public int DayTotal => Scores.Sum(i => i.Value);
        public IEnumerable<string> TaskList => Scores.OrderBy(s => s.Position).Select(s => s.Task);

        public void SetTasks(string[] tasks)
        {
            var origTotal = DayTotal;
            Scores.Where(s => !tasks.Contains(s.Task, StringComparer.OrdinalIgnoreCase))
                .ToList().ForEach(i => Scores.Remove(i));
            int pos = 0;
            foreach (var task in tasks)
            {
                var score = Scores.FirstOrDefault(s => s.Task.Equals(task, StringComparison.OrdinalIgnoreCase));
                if (score != null)
                {
                    score.SetPosition(pos++);
                }
                else
                {
                    score = new Score(0, task, pos++, 0);
                    Scores.Add(score);
                }
            }
            var delta = DayTotal - origTotal;
            Child.Update(null, null, Child.TotalScore + delta);
        }

        public void SetScore(string task, int value)
        {
            var origTotal = DayTotal;
            var score = Scores.FirstOrDefault(i => i.Task.Equals(task, StringComparison.OrdinalIgnoreCase));
            if (score == null)
            {
                throw new ArgumentException($"Task '{task}' not found.");
            }
            score.Update(value);
            var delta = DayTotal - origTotal;
            Child.Update(null, null, Child.TotalScore + delta);
        }
    }
}