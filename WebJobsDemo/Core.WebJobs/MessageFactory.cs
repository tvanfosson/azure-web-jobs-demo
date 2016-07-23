using System.Net;
using System.Net.Mail;
using WebJobDemo.Core.Configuration;
using WebJobDemo.Core.Data.Models;

namespace Core.WebJobs
{
    public class MessageFactory
    {
        public readonly IApplicationSettings _settings;

        public MessageFactory(IApplicationSettings settings)
        {
            _settings = settings;
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

        public MailMessage CreateConfirmationMessage(Subscription subscription)
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
            var emailAddress = WebUtility.UrlEncode(subscription.EmailAddress);

            var emailParam = $"emailAddress={emailAddress}";

            var keyParam = $"subscriptionKey={subscription.SubscriptionKey}";

            var parameters = string.Join("&", emailParam, keyParam);

            return parameters;
        }

        public MailMessage CreateWelcomeMessage(Subscription subscription)
        {
            var body = $"<div>Hello, {subscription.FirstName},<br/></br/></div><div>Welcome to the Demo!</div>";

            return CreateMessage(subscription.EmailAddress, "Welcome to the Demo!", body);
        }
    }
}
