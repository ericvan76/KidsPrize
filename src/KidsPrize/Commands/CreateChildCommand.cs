using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Commands
{
    public class CreateChildCommand
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^(M|F)$")]
        public string Gender { get; set; }

        [Required]
        [MinLength(1)]
        public string[] Tasks { get; set; }

    }
}