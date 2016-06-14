using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KidsPrize.Models
{
    [Table(nameof(Day))]
    public class Day : Entity
    {
        private Day() : base()
        { }
        public Day(int id, Child child, DateTime date,
            IList<string> taskList, ICollection<Score> scores) : base()
        {
            Id = id;
            Child = child;
            Date = date.Date;
            TaskList = taskList ?? new List<string>();
            Scores = scores ?? new HashSet<Score>();
        }

        [Key]
        public int Id { get; private set; }
        [Required]
        public Child Child { get; private set; }
        [Required]
        public DateTime Date { get; private set; }
        [NotMapped]
        public IList<string> TaskList { get; private set; }
        public string Tasks
        {
            get
            {
                return TaskList.Count == 0
                    ? null
                    : string.Join("|", TaskList);
            }
            private set
            {
                TaskList = value == null
                    ? new List<string>()
                    : new List<string>(value.Split('|'));
            }
        }
        public ICollection<Score> Scores { get; private set; }

        public void Modify(IList<string> taskList)
        {
            if (taskList != null)
            {
                var origTotal = TotalScore();
                TaskList = taskList;
                var toRemove = Scores.Where(i => !TaskList.Contains(i.Task, StringComparer.OrdinalIgnoreCase));
                foreach (var item in toRemove)
                {
                    Scores.Remove(item);
                }
                var delta = TotalScore() - origTotal;
                Child.Update(null, null, Child.TotalScore + delta);
            }
        }

        public void AddOrUpdateScore(Score score)
        {
            var origTotal = TotalScore();
            var existing = Scores.FirstOrDefault(i => i.Task.Equals(score.Task, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Update(score.Value);
            }
            else
            {
                Scores.Add(score);
            }
            var delta = TotalScore() - origTotal;
            Child.Update(null, null, Child.TotalScore + delta);
        }

        private int TotalScore()
        {
            return Scores.Sum(i => i.Value);
        }
    }
}