using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Commands
{
    public class UpdateChildCommand
    {
        [Required]
        public Guid ChildId { get; set; }

        public string Name { get; set; }

        [RegularExpression(@"^(M|F)$")]
        public string Gender { get; set; }

        public string[] Tasks { get; set; }
    }
}