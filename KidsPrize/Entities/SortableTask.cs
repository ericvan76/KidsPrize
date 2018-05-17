using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Entities
{
    public class SortableTask
    {
        private SortableTask() { }

        public SortableTask(string name, int order)
        {
            Name = name;
            Order = order;
        }

        [Key]
        public int Id { get; private set; }

        [MaxLength(50)]
        public string Name { get; private set; }

        [Required]
        public int Order { get; private set; }
    }
}