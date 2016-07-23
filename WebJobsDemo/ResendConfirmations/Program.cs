using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;

namespace ResendConfirmations
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var host = new JobHost();

            foreach (var method in typeof (Functions).GetMethods().Where(m => m.GetCustomAttribute<NoAutomaticTriggerAttribute>() != null))
            {
                host.Call(method);
            }
        }
    }
}
