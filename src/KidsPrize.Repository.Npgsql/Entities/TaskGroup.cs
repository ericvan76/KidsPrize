using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Repository.Npgsql.Entities
{
    public class TaskGroup
    {
        private TaskGroup() { }

        public TaskGroup(Child child, DateTime effectiveDate, string[] tasks)
        {
            Child = child;
            EffectiveDate = effectiveDate;
            Tasks = new HashSet<SortableTask>();
            for (var i = 0; i < tasks.Length; i++)
            {
                Tasks.Add(new SortableTask(tasks[i], i));
            }
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        public virtual Child Child { get; set; }

        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; private set; }

        public virtual ICollection<SortableTask> Tasks { get; private set; }

        public void Update(string[] tasks)
        {
            Tasks.Clear();
            for (var i = 0; i < tasks.Length; i++)
            {
                Tasks.Add(new SortableTask(tasks[i], i));
            }
        }

    }
}