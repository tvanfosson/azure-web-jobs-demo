using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class DomainStatisticsViewModel 
    {
        public string Domain { get; set; }

        public int Count { get; set; }

        public int Confirmed { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }
    }
}