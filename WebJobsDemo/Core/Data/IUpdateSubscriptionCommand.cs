using System;
using System.Data;
using System.Threading.Tasks;
using WebJobDemo.Core.Data.Models;

namespace WebJobDemo.Core.Data
{
    public interface IUpdateSubscriptionCommand
    {
        Task Update(Subscription subscription, Func<Subscription, IDbConnection, IDbTransaction, Task> continueWith = null);
    }
}
