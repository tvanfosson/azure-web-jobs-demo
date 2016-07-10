using System;
using System.Threading.Tasks;
using WebApp.Data.Models;

namespace WebApp.Data
{
    public interface IAddSubscriptionCommand
    {
        Task<Subscription> Add(string firstName, string lastName, string emailAddress, Func<Subscription, Task<Subscription>> continueWith = null);
    }
}