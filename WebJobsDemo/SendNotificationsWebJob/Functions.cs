using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
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

        public Functions()
            : this(new ApplicationSettings(""), new SqlConnectionFactory())
        {
        }

        public Functions(IApplicationSettings settings, IConnectionFactory connectionFactory)
        {
            _settings = settings;
            _connectionFactory = connectionFactory;
        }

        public async Task SendNotification([ServiceBusTrigger(ApplicationSettings.JobMessagesQueue)] Guid id, TextWriter log)
        {
            try
            {
                await log.WriteAsync($"Sending notifications for {id}");

                var query = new SubscriptionQuery(_settings, _connectionFactory);

                var subscription = await query.GetSubscriptionById(id);

                if (subscription == null || subscription.ConfirmationSentOn.HasValue)
                {
                    return;
                }

                var mailMessage = CreateConfirmationMessage(subscription);

                subscription.ConfirmationSentOn = DateTime.UtcNow;

                var update = new UpdateSubscriptionCommand(_settings, _connectionFactory);

                await update.Update(subscription, async s =>
                {             
                    using (var smtpClient = new SmtpClient())
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                });

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

            var mailMessage = CreateWelcomeMessage(subscription);

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        private MailMessage CreateMessage(string to, string subject, string body)
        {
            var message = new MailMessage(_settings.WebJobsFromAddress, to)
            {
                IsBodyHtml = true,
                Body = body,
                Subject = subject
            };

            return message;
        }

        private MailMessage CreateConfirmationMessage(Subscription subscription)
        {
            var confirm = CreateConfirmationLink(subscription);

            var body = $"<div>Hello {subscription.FirstName},<br/><br/></div><div>We're glad you signed up. Please {confirm} your subscription.</div>";

            return CreateMessage(subscription.EmailAddress, "Confirm your address", body);
        }


        private static string CreateConfirmationLink(Subscription subscription)
        {
            var parameters = CreateConfirmationParameters(subscription);

            var link = $"<a href=\"http://webjobsdemo.azurewebsites.net/home/confirm?{parameters}\">confirm</a>";

            return link;
        }

        private static string CreateConfirmationParameters(Subscription subscription)
        {
            // TODO: a one-time token for the subscription confirmation rather than the email address and key
            var emailParam = $"emailAddress={subscription.EmailAddress}";

            var keyParam = $"subscriptionKey={subscription.SubscriptionKey}";

            var parameters = string.Join("&", emailParam, keyParam);

            return parameters;
        }

        private MailMessage CreateWelcomeMessage(Subscription subscription)
        {
            var body = $"<div>Hello, {subscription.FirstName},<br/></br/></div><div>Welcome to the Demo!</div>";

            return CreateMessage(subscription.EmailAddress, "Welcome to the Demo!", body);
        }
    }
}
