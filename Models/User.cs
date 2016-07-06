using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using IdentityModel;

namespace KidsPrize.Models
{
    [Table(nameof(User))]
    public class User : Entity
    {
        private User() : base()
        { }

        public User(int id, Guid uid, string email, string givenName, string familyName, string displayName, IList<Identifier> identifiers, IList<Child> children) : base()
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

        [NotMapped]
        public IList<Claim> Claims
        {
            get
            {
                return new List<Claim>()
                {
                    new Claim(JwtClaimTypes.Subject, Uid.ToString()),
                    new Claim(JwtClaimTypes.Email, Email),
                    new Claim(JwtClaimTypes.Name, DisplayName),
                    new Claim(JwtClaimTypes.GivenName, GivenName),
                    new Claim(JwtClaimTypes.FamilyName, FamilyName),
                    new Claim(JwtClaimTypes.IdentityProvider, "Local"),
                    new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString())
                };
            }
        }

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

        public void AddIdentifier(string issuer, string value)
        {
            if (!Identifiers.Any(i => i.Issuer == issuer && i.Value == value))
            {
                Identifiers.Add(new Identifier(0, issuer, value));
            }
        }
    }
}

