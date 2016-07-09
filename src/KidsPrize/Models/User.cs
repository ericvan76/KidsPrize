using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KidsPrize.Models
{
    [Table(nameof(User))]
    public class User : Entity
    {
        private User() : base()
        { }

        public User(int id, Guid uid, string email, string givenName, string familyName, string displayName, ICollection<Identifier> identifiers, ICollection<Child> children) : base()
        {
            Id = id;
            Uid = uid;
            Email = email;
            GivenName = givenName;
            FamilyName = familyName;
            DisplayName = displayName;
            Identifiers = identifiers;
            Children = children;
        }

        [Key]
        public int Id { get; private set; }
        [Required]
        public Guid Uid { get; private set; }
        [Required]
        [MaxLength(250)]
        public string Email { get; private set; }
        [MaxLength(250)]
        public string GivenName { get; private set; }
        [MaxLength(250)]
        public string FamilyName { get; private set; }
        [MaxLength(250)]
        public string DisplayName { get; private set; }
        public ICollection<Identifier> Identifiers { get; private set; }
        public ICollection<Child> Children { get; private set; }

        public void Update(string givenName, string familyName, string displayName)
        {
            GivenName = givenName ?? GivenName;
            FamilyName = familyName ?? FamilyName;
            DisplayName = displayName ?? DisplayName;
        }

        public void AddChild(Child child)
        {
            Children.Add(child);
        }

        public void RemoveChild(Guid childUid)
        {
            var child = Children.FirstOrDefault(i => i.Uid == childUid);
            if (child != null)
            {
                Children.Remove(child);
            }
        }

        public bool TryAddIdentifier(Identifier identifier)
        {
            if (!Identifiers.Contains(identifier))
            {
                Identifiers.Add(identifier);
                return true;
            }
            return false;
        }
    }
}

