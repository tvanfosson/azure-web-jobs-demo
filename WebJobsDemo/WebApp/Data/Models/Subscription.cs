using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Data.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(384)]
        public string EmailAddress { get; set; }

        public Guid SubscriptionKey { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ConfirmationSentOn { get; set; }

        public bool Confirmed { get; set; }

        public byte[] Version { get; set; }
    }
}