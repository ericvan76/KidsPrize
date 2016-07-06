using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidsPrize.Models
{
    [Table(nameof(Identifier))]
    public class Identifier : Entity
    {
        private Identifier() : base()
        { }

        public Identifier(int id, string issuer, string value) : base()
        {
            Id = id;
            Issuer = issuer;
            Value = value;
        }

        [Key]
        public int Id { get; private set; }
        [Required]
        public string Issuer { get; private set; }
        [Required]
        public string Value { get; private set; }

    }
}