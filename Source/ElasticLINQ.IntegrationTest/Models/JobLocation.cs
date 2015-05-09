using System;

namespace ElasticLINQ.IntegrationTest.Models
{
    class JobLocation : IEquatable<JobLocation>
    {
        public string Country { get; set; }
        public string City { get; set; }

        public bool Equals(JobLocation other)
        {
            return Equals(Country, other.Country) &&
                   Equals(City, other.City);
        }

        public override bool Equals(object obj)
        {
            return obj is JobLocation && Equals((JobLocation)obj);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            if (City != null)
                hash = hash * 23 + City.GetHashCode();
            return hash;
        }
    }
}