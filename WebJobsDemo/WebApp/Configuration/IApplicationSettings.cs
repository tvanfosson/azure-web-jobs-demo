namespace WebApp.Configuration
{
    public interface IApplicationSettings
    {
        string WebJobsDemoConnectionString { get; }
        string JobMessagesConnectionString { get; }
        string JobMessagesQueue { get; }
    }
}
