using System;

namespace ElasticLinq.IntegrationTest.Models
{
    class AccessLocation : IEquatable<AccessLocation>
    {
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool Equals(AccessLocation other)
        {
            return Equals(Zip, other.Zip) &&
                   Equals(City, other.City) &&
                   Equals(State, other.State) &&
                   Equals(Latitude, other.Latitude) &&
                   Equals(Longitude, other.Longitude);
        }

        public override bool Equals(object obj)
        {
            return obj is AccessLocation && Equals((AccessLocation) obj);
        }

        public override int GetHashCode()
        {
            return Latitude.GetHashCode();
        }
    }
}