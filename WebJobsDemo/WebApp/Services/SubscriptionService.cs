using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Data.Models;

namespace WebApp.Services
{
    public class SubscriptionService
    {
        private readonly IAddSubscriptionCommand _addSubscription;
        private readonly IOfflineProcessingService _offlineProcessingService;

        public SubscriptionService(IAddSubscriptionCommand addSubscription, IOfflineProcessingService offlineProcessingService)
        {
            _addSubscription = addSubscription;
            _offlineProcessingService = offlineProcessingService;
        }

        public async Task<Subscription> SignUp(string firstName, string lastName, string emailAddress)
        {
            return await _addSubscription.Add(firstName, lastName, emailAddress, async s =>
            {
                await _offlineProcessingService.NotifySubscriber(s.Id)
                                               .ConfigureAwait(false);
                return s;
            });
        }
    }
}