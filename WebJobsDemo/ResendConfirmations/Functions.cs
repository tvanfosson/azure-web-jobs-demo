using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Core.WebJobs;
using Microsoft.Azure.WebJobs;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data;

namespace ResendConfirmations
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public static async Task ProcessMissingConfirmationEmails(TextWriter log)
        {
            await log.WriteLineAsync("Starting resend of missing confirmation emails");

            var settings = new ApplicationSettings("");

            var connectionFactory = new SqlConnectionFactory();

            var query = new SubscriptionQuery(settings, connectionFactory);

            var subscriptions = await query.GetSubscriptionsWithMissingConfirmations();

            var messageFactory = new MessageFactory(settings);

            var updater = new UpdateSubscriptionCommand(settings, connectionFactory);

            foreach (var subscription in subscriptions.Where(s => s.CreatedOn > DateTime.UtcNow.Date))
            {
                await log.WriteLineAsync($"Sending confirmation email to {subscription.EmailAddress}");

                subscription.ConfirmationSentOn = DateTime.UtcNow;

                await updater.Update(subscription, async (s, c, t) =>
                {
                    using (var smtpClient = new SmtpClient())
                    {
                        var updateStats = new UpdateStatisticCommands(settings, connectionFactory);

                        await updateStats.AddOrUpdateDomainCount(subscription.GetDomain(), c, t);

                        await smtpClient.SendMailAsync(messageFactory.CreateConfirmationMessage(s));
                    }
                });
            }

            await log.WriteLineAsync("Completed resend of missing confirmation emails");
        }
    }
}
