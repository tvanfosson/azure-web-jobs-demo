using System;
using System.Threading.Tasks;
using WebJobDemo.Core.Data.Models;

namespace WebApp.Services
{
    public interface IOfflineProcessingService
    {
        Task NotifySubscriber(Guid id);
        Task ConfirmationReceived(Subscription subscription);
    }
}
