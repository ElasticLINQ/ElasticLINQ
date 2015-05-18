using System;

namespace ElasticLINQ.IntegrationTest.Models
{
    class LastAccess : IEquatable<LastAccess>
    {
        public DateTime When { get; set; }
        public string Agent { get; set; }
        public AccessLocation Location { get; set; }
        public string Ip { get; set; }

        public bool Equals(LastAccess other)
        {
            return Equals(When, other.When) &&
                   Equals(Agent, other.Agent) &&
                   Equals(Location, other.Location) &&
                   Equals(Ip, other.Ip);
        }

        public override bool Equals(object obj)
        {
            return obj is LastAccess && Equals((LastAccess) obj);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * When.GetHashCode();
            if (!String.IsNullOrEmpty(Ip))
                hash = hash & Ip.GetHashCode();
            return hash;
        }
    }
}