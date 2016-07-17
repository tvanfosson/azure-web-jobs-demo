using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebJobDemo.Core.Data.Models;

namespace WebJobDemo.Core.Data
{
    public interface ISubscriptionQuery
    {
        Task<ICollection<Subscription>> GetSubscriptions();
        Task<Subscription> GetSubscriptionById(Guid id);
        Task<Subscription> GetSubscriptionByEmailAddressAndSubscriptionKey(string emailAddress, Guid subscriptionKey);
        Task<ICollection<DomainStatistic>> GetStatistics();
    }
}