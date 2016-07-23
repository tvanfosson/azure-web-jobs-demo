namespace WebJobDemo.Core.Configuration
{
    public interface IApplicationSettings
    {
        string WebJobsDemoConnectionString { get; }
        string JobMessagesConnectionString { get; }
        string WebJobsFromAddress { get; }
        string WebHookUri { get; }
        string WebHookKey { get;  }
    }
}
