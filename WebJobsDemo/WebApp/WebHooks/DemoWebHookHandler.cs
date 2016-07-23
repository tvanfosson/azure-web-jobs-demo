using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;

namespace WebApp.WebHooks
{
    public class DemoWebHookHandler : WebHookHandler
    {
        public override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            WebHookCounter.AddToCount();
            return Task.CompletedTask;
        }
    }
}