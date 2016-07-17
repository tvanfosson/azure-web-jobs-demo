using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class SubscriberLookupModel
    {
        [Display(Name = "Email Address")]
        [Required]
        public string EmailAddress { get; set; }

        [Display(Name = "Subscription Key")]
        [Required]
        public Guid? SubscriptionKey { get; set; }
    }
}