using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidsPrize.Models
{
    [Table(nameof(Identifier))]
    public class Identifier : Entity, IEquatable<Identifier>
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
        [MaxLength(50)]
        public string Issuer { get; private set; }
        [Required]
        [MaxLength(50)]
        public string Value { get; private set; }

        public bool Equals(Identifier other)
        {
            if (other == null)
            {
                return false;
            }
            return Issuer.Equals(other.Issuer, StringComparison.OrdinalIgnoreCase)
                && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object other)
        {
            if (other is Identifier)
            {
                return this.Equals((Identifier)other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Issuer.ToLower().GetHashCode();
                hash = hash * 23 + Value.ToLower().GetHashCode();
                return hash;
            }
        }
    }
}