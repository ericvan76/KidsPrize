using System;
using System.ComponentModel.DataAnnotations;

namespace KidsPrize.Commands
{
    public class CreateRedeemCommand
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        public int Value { get; set; }
    }
}