using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data.Models;

namespace SendWebHooksWebJob
{
    public class Functions
    {
        private readonly IWebHookManager _webHookManager;


        public Functions(IWebHookManager webHookManager)
        {
            _webHookManager = webHookManager;

        }

        public  async Task ProcessAsync(
            [ServiceBusTrigger(ApplicationSettings.ConfirmationTopic, ApplicationSettings.WebHookSubscription)] BrokeredMessage message,
            TextWriter log)
        {
            var subscription = message.GetBody<Subscription>();

            await log.WriteLineAsync($"Sending web hook for {subscription.EmailAddress}");

            await _webHookManager.NotifyAllAsync(
                                    ApplicationSettings.WebHookSubscription,
                                    $"Welcome, {subscription.FirstName} {subscription.LastName}"
                                 );

            await log.WriteLineAsync("Sent WebHook");
        }
    }
}
