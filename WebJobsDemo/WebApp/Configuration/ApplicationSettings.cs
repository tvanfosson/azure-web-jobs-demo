using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure;

namespace WebApp.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ApplicationSettings : IApplicationSettings
    {
        private const string WebJobsDemoConnectionStringKey = "WebJobsDemoConnection";
        private const string JobMessagesConnectionStringKey = "JobMessagesConnectionString";
        private const string JobMessagesQueueKey = "JobMessagesQueue";

        private static readonly Lazy<string> _webJobsConnectionString = new Lazy<string>(() => CloudConfigurationManager.GetSetting(WebJobsDemoConnectionStringKey)); 
        private static readonly Lazy<string> _jobMessagesConnectionString = new Lazy<string>(() => CloudConfigurationManager.GetSetting(JobMessagesConnectionStringKey));
        private static readonly Lazy<string> _jobMessagesQueue = new Lazy<string>(() => CloudConfigurationManager.GetSetting(JobMessagesQueueKey));

        public string WebJobsDemoConnectionString => _webJobsConnectionString.Value;
        public string JobMessagesConnectionString => _jobMessagesConnectionString.Value;
        public string JobMessagesQueue => _jobMessagesQueue.Value;
    }
}