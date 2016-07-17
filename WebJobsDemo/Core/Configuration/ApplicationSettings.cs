using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure;

namespace WebJobDemo.Core.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ApplicationSettings : IApplicationSettings
    {
        public const string JobMessagesQueue = "job-messages";
        public const string ConfirmationTopic = "confirmations";
        public const string StatsSubscription = "updatestats";
        public const string WelcomeSubscription = "sendwelcome";

        private static class Mail
        {
            public const string FromAddressKey = "Mail.FromAddress";
            public const string AdminAddressKey = "Mail.AdminAddress";
        }

        private const string WebJobsDemoConnectionStringKey = "WebJobsDemoConnection";
        private const string JobMessagesConnectionStringKey = "JobMessagesConnectionString";

        private static readonly Lazy<string> _webJobsConnectionString = new Lazy<string>(() => GetSetting(WebJobsDemoConnectionStringKey)); 
        public string WebJobsDemoConnectionString => _webJobsConnectionString.Value;

        private static readonly Lazy<string> _jobMessagesConnectionString = new Lazy<string>(() => GetSetting(JobMessagesConnectionStringKey));
        public string JobMessagesConnectionString => _jobMessagesConnectionString.Value;

        private static readonly Lazy<string> _webJobsFromAddress = new Lazy<string>(() => GetSetting(Mail.FromAddressKey));
        public string WebJobsFromAddress => _webJobsFromAddress.Value;

        private static readonly Lazy<string> _webJobsAdminAddress = new Lazy<string>(() => GetSetting(Mail.AdminAddressKey));
        public string WebJobsAdminAddress => _webJobsAdminAddress.Value;

        private static string GetSetting(string key)
        {
            return CloudConfigurationManager.GetSetting(key);
        }
    }
}