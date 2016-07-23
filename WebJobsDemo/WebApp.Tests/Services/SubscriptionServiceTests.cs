using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Moq;
using WebApp.Services;
using WebJobDemo.Core.Data;
using WebJobDemo.Core.Data.Models;
using Xunit;

namespace WebApp.Tests.Services
{
    public class SubscriptionServiceTests
    {
        private readonly TestFixture _fixture;

        [Fact]
        public async Task When_the_service_is_called_it_adds_the_subscription()
        {
            var service = _fixture.GetService();

            await service.SignUp(_fixture.FirstName, _fixture.LastName, _fixture.EmailAddress);

            _fixture.MockAddCommand.Verify(c => c.Add(_fixture.FirstName, _fixture.LastName, _fixture.EmailAddress, It.IsAny<Func<Subscription, Task<Subscription>>>()));
        }

        [Fact]
        public async Task When_the_service_is_called_it_sends_the_notification()
        {
            var service = _fixture.GetService();

            await service.SignUp(_fixture.FirstName, _fixture.LastName, _fixture.EmailAddress);

            _fixture.MockOfflineProcessing.Verify(s => s.NotifySubscriber(_fixture.NewSubscription.Id));
        }

        [Fact]
        public async Task When_the_service_is_called_it_returns_the_subscription_created_by_the_add_command()
        {
            var service = _fixture.GetService();

            var subscription = await service.SignUp(_fixture.FirstName, _fixture.LastName, _fixture.EmailAddress);

            Assert.Same(_fixture.NewSubscription, subscription);
        }

        [Fact]
        public async Task When_the_service_is_called_and_the_notification_fails_no_subscription_is_returned()
        {
            var service = _fixture.GetService();

            var subscription = await service.SignUp(_fixture.FirstName, _fixture.LastName, _fixture.FailedEmailAddress);

            Assert.Null(subscription);
        }

        [Fact]
        public async Task When_the_service_is_called_it_updates_the_subscription()
        {
            var service = _fixture.GetService();

            await service.Confirm(_fixture.NewSubscription);

            _fixture.MockUpdateCommand.Verify(c => c.Update(_fixture.NewSubscription, It.IsAny<Func<Subscription, IDbConnection, IDbTransaction, Task>>()));
        }

        [Fact]
        public async Task When_the_service_is_called_it_sends_the_confirmation()
        {
            var service = _fixture.GetService();

            await service.Confirm(_fixture.NewSubscription);

            _fixture.MockOfflineProcessing.Verify(s => s.ConfirmationReceived(_fixture.NewSubscription));
        }

        public SubscriptionServiceTests()
        {
            _fixture = new TestFixture();
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class TestFixture
        {

            public readonly Subscription NewSubscription = new Subscription
            {
                Id = Guid.NewGuid(),
                FirstName = _firstName,
                LastName = _lastName,
                EmailAddress = _emailAddress,
                SubscriptionKey = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow.Date
            };

            public readonly Subscription FailedSubscription = new Subscription
            {
                Id = Guid.NewGuid(),
                FirstName = _firstName,
                LastName = _lastName,
                EmailAddress = _failedEmailAddress,
                SubscriptionKey = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow.Date
            };

            public readonly Mock<IAddSubscriptionCommand> MockAddCommand = new Mock<IAddSubscriptionCommand>();
            public readonly Mock<IUpdateSubscriptionCommand> MockUpdateCommand = new Mock<IUpdateSubscriptionCommand>();
            public readonly Mock<IOfflineProcessingService> MockOfflineProcessing = new Mock<IOfflineProcessingService>();

            private const string _firstName = "first";
            private const string _lastName = "last";
            private const string _emailAddress = "valid@example.com";
            private const string _failedEmailAddress = "invalid@example.com";

            public string FirstName => _firstName;
            public string LastName => _lastName;
            public string EmailAddress => _emailAddress;
            public string FailedEmailAddress => _failedEmailAddress;

            public SubscriptionService GetService()
            {
                MockAddCommand.Setup(m => m.Add(NewSubscription.FirstName,
                                                NewSubscription.LastName,
                                                NewSubscription.EmailAddress,
                                                It.IsAny<Func<Subscription, Task<Subscription>>>()))
                              .Callback<string, string, string, Func<Subscription, Task<Subscription>>>((f, l, e, func) => Assert.NotNull(func(NewSubscription).Result))
                              .ReturnsAsync(NewSubscription);

                MockAddCommand.Setup(m => m.Add(FailedSubscription.FirstName,
                                                FailedSubscription.LastName,
                                                FailedSubscription.EmailAddress,
                                                It.IsAny<Func<Subscription, Task<Subscription>>>()))
                              .Callback<string, string, string, Func<Subscription, Task<Subscription>>>(async (f, l, e, func) => await Assert.ThrowsAsync<MessagingException>(async () => await func(FailedSubscription)))
                              .ReturnsAsync(null);

                MockUpdateCommand.Setup(m => m.Update(It.Is<Subscription>(s => s.EmailAddress == NewSubscription.EmailAddress), It.IsAny<Func<Subscription, IDbConnection, IDbTransaction, Task>>()))
                                 .Callback<Subscription, Func<Subscription, Task>>(async (s, t) => await t(s))
                                 .Returns(Task.CompletedTask);

                MockOfflineProcessing.Setup(m => m.NotifySubscriber(It.IsNotIn(NewSubscription.Id)))
                                     .Throws(new ServerBusyException("message"));

                MockOfflineProcessing.Setup(m => m.ConfirmationReceived(It.IsNotIn(NewSubscription)))
                                     .Throws(new ServerBusyException("message"));

                return new SubscriptionService(MockAddCommand.Object, MockUpdateCommand.Object, MockOfflineProcessing.Object);
            }
        }
    }
}
