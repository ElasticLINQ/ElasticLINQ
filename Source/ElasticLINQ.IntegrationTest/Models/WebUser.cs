using System;
using System.Linq;

namespace ElasticLINQ.IntegrationTest.Models
{
    class WebUser : IEquatable<WebUser>
    {
        public int Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public LastAccess[] LastAccess { get; set; }
        public DateTime Joined { get; set; }
        public string PasswordHash { get; set; }

        public bool Equals(WebUser other)
        {
            return Equals(Id, other.Id) &&
                   Equals(Forename, other.Forename) &&
                   Equals(Surname, other.Surname) &&
                   Equals(Phone, other.Phone) &&
                   Equals(Email, other.Email) &&
                   Equals(Username, other.Username) &&
                   Equals(Joined, other.Joined) &&
                   Equals(PasswordHash, other.PasswordHash) &&
                   (LastAccess == other.LastAccess ||
                   LastAccess.SequenceEqual(other.LastAccess));
        }

        public override bool Equals(object obj)
        {
            return obj is WebUser && Equals((WebUser) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}