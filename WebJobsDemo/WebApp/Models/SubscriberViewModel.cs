using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class SubscriberViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(384)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }
}