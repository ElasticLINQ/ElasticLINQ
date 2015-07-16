using System;

namespace ElasticLinq.IntegrationTest.Models
{
    class JobOpening : IEquatable<JobOpening>
    {
        public long Id { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public DateTime Posted { get; set; }
        public string Description { get; set; }
        public JobLocation Location { get; set; }
        public Uri MoreInformation { get; set; }

        public bool Equals(JobOpening other)
        {
            return Equals(Id, other.Id) &&
                   Equals(Company, other.Company) &&
                   Equals(JobTitle, other.JobTitle) &&
                   Equals(Posted, other.Posted) &&
                   Equals(Description, other.Description) &&
                   Equals(Location, other.Location) &&
                   Equals(MoreInformation, other.MoreInformation);
        }

        public override bool Equals(object obj)
        {
            return obj is JobOpening && Equals((JobOpening)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}