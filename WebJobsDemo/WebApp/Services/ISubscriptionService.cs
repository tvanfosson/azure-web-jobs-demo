using System.Threading.Tasks;
using WebJobDemo.Core.Data.Models;

namespace WebApp.Services
{
    public interface ISubscriptionService
    {
        Task<Subscription> SignUp(string firstName, string lastName, string emailAddress);
        Task<Subscription> Confirm(Subscription subscription);
    }
}