using System.Threading.Tasks;
using WebJobDemo.Core.Data;
using WebJobDemo.Core.Data.Models;

namespace WebApp.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IAddSubscriptionCommand _addSubscription;
        private readonly IUpdateSubscriptionCommand _updateSubscription;
        private readonly IOfflineProcessingService _offlineProcessingService;

        public SubscriptionService(IAddSubscriptionCommand addSubscription, IUpdateSubscriptionCommand updateSubscription, IOfflineProcessingService offlineProcessingService)
        {
            _addSubscription = addSubscription;
            _updateSubscription = updateSubscription;
            _offlineProcessingService = offlineProcessingService;
        }

        public async Task<Subscription> SignUp(string firstName, string lastName, string emailAddress)
        {
            return await _addSubscription.Add(firstName, lastName, emailAddress, async s =>
            {
                await _offlineProcessingService.NotifySubscriber(s.Id)
                                               .ConfigureAwait(false);
                return s;
            })
            .ConfigureAwait(false);
        }

        public async Task<Subscription> Confirm(Subscription subscription)
        {
            if (subscription.Confirmed)
            {
                return subscription;
            }

            subscription.Confirmed = true;

            await _updateSubscription.Update(subscription, async s =>
            {
                await _offlineProcessingService.ConfirmationReceived(subscription)
                                               .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

            return subscription;
        }
    }
}