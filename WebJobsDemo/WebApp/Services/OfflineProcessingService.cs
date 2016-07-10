using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using WebApp.Configuration;

namespace WebApp.Services
{
    public class OfflineProcessingService : IOfflineProcessingService
    {
        private readonly QueueClient _queueClient;

        public OfflineProcessingService(IApplicationSettings settings)
        {
            var messagingFactory = MessagingFactory.CreateFromConnectionString(settings.JobMessagesConnectionString);
            _queueClient = messagingFactory.CreateQueueClient(settings.JobMessagesQueue);
        }
        public async Task NotifySubscriber(Guid id)
        {
            await _queueClient.SendAsync(new BrokeredMessage(id));
        }
    }
}