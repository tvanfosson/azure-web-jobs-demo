using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using Core.WebJobs;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data;
using WebJobDemo.Core.Data.Models;

namespace SendNotificationsWebJob
{
    public class Functions
    {
        private readonly IApplicationSettings _settings;
        private readonly IConnectionFactory _connectionFactory;
        private readonly MessageFactory _messageFactory;

        public Functions()
            : this(new ApplicationSettings(""), new SqlConnectionFactory())
        {
        }

        public Functions(IApplicationSettings settings, IConnectionFactory connectionFactory)
        {
            _settings = settings;
            _connectionFactory = connectionFactory;
            _messageFactory = new MessageFactory(_settings);
        }

        public async Task SendNotification([ServiceBusTrigger(ApplicationSettings.JobMessagesQueue)] Guid id, TextWriter log)
        {
            try
            {
                await log.WriteLineAsync($"Sending notifications for {id}");

                var query = new SubscriptionQuery(_settings, _connectionFactory);

                var subscription = await query.GetSubscriptionById(id);

                if (subscription == null || subscription.ConfirmationSentOn.HasValue)
                {
                    return;
                }

                var mailMessage = _messageFactory.CreateConfirmationMessage(subscription);

                subscription.ConfirmationSentOn = DateTime.UtcNow;

                var update = new UpdateSubscriptionCommand(_settings, _connectionFactory);

                await update.Update(subscription, async (s,c,t) =>
                {    
                    var updateStats = new UpdateStatisticCommands(_settings, _connectionFactory);

                    await updateStats.AddOrUpdateDomainCount(subscription.GetDomain(), c, t);
                           
                    using (var smtpClient = new SmtpClient())
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                });

                await log.WriteLineAsync($"Sent email to {subscription.EmailAddress}");
            }
            catch (Exception e)
            {
                log.Write($"GUID: {id}, Error: {e.Message}");
                throw;
            }
        }

        public async Task SendWelcome([ServiceBusTrigger(ApplicationSettings.ConfirmationTopic, ApplicationSettings.WelcomeSubscription)] BrokeredMessage message, TextWriter log)
        {

            var subscription = message.GetBody<Subscription>();

            await log.WriteLineAsync($"Sending welcome notification for {subscription.EmailAddress}");

            var mailMessage = _messageFactory.CreateWelcomeMessage(subscription);

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(mailMessage);
            }

            await log.WriteLineAsync($"Sent welcome notification for {subscription.EmailAddress}");
        }
    }
}
