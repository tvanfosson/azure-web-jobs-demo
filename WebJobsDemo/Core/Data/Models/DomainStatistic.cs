using System;

namespace WebJobDemo.Core.Data.Models
{
    public class DomainStatistic
    {
        public string Domain { get; set; }

        public int Count { get; set; }

        public int Confirmed { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
