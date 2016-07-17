using System;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IOfflineProcessingService
    {
        Task NotifySubscriber(Guid id);
        Task ConfirmationReceived(Guid id);
    }
}
