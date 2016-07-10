using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Data.Models;

namespace WebApp.Data
{
    public interface ISubscriptionQuery
    {
        Task<ICollection<Subscription>> GetSubscriptions();
        Task<Subscription> GetSubscriptionById(Guid id);
        Task<Subscription> GetSubscriptionByEmailAddressAndSubscriptionKey(string emailAddress, Guid subscriptionKey);
    }
}