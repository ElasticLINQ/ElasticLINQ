using System;

namespace TestConsoleApp.Models
{
    public class AccountSubscription
    {
        public string AccountAlias { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubscriptionId { get; set; }
        public string ProductCode { get; set; }
        public string LocationAlias { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndedBy { get; set; }

        public override string ToString()
        {
            return SubscriptionId + " " + Name + " " + Description + " " + CreatedBy;
        }
    }
}