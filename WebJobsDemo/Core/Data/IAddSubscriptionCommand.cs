using System;
using System.Threading.Tasks;
using WebJobDemo.Core.Data.Models;

namespace WebJobDemo.Core.Data
{
    public interface IAddSubscriptionCommand
    {
        Task<Subscription> Add(string firstName, string lastName, string emailAddress, Func<Subscription, Task<Subscription>> continueWith = null);
    }
}