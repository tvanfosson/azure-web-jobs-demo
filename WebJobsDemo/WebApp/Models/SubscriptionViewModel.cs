using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class SubscriptionViewModel : SubscriberLookupModel
    {
        public Guid Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Display(Name = "Confirmation Sent On")]
        public DateTime? ConfirmationSentOn { get; set; }

        public bool Confirmed { get; set; }

        public bool PerformedLookup { get; set; }

        public static SubscriptionViewModel Convert(SubscriberLookupModel model)
        {
            return new SubscriptionViewModel
            {
                EmailAddress = model.EmailAddress,
                SubscriptionKey = model.SubscriptionKey
            };
        }
    }
}