using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data.Models;

namespace WebApp.Services
{
    public class OfflineProcessingService : IOfflineProcessingService
    {
        private readonly QueueClient _queueClient;
        private readonly TopicClient _topicClient;

        public OfflineProcessingService(IApplicationSettings settings)
        {
            var messagingFactory = MessagingFactory.CreateFromConnectionString(settings.JobMessagesConnectionString);
            _queueClient = messagingFactory.CreateQueueClient(ApplicationSettings.JobMessagesQueue);
            _topicClient = messagingFactory.CreateTopicClient(ApplicationSettings.ConfirmationTopic);
        }

        public async Task NotifySubscriber(Guid id)
        {
            await _queueClient.SendAsync(new BrokeredMessage(id));
        }

        public async Task ConfirmationReceived(Subscription subscription)
        {
            await _topicClient.SendAsync(new BrokeredMessage(subscription));
        }
    }
}