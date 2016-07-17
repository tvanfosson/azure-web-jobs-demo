using System;
using System.Threading.Tasks;
using WebJobDemo.Core.Data.Models;

namespace WebJobDemo.Core.Data
{
    public interface IUpdateSubscriptionCommand
    {
        Task Update(Subscription subscription, Func<Subscription, Task> continueWith = null);
    }
}
