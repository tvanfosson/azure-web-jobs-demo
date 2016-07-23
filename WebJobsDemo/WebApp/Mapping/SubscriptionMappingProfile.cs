using AutoMapper;
using WebApp.Models;
using WebJobDemo.Core.Data.Models;

namespace WebApp.Mapping
{
    public class SubscriptionMappingProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Subscription, SubscriptionViewModel>()
                .ForMember(d => d.PerformedLookup, m => m.Ignore());
        }
    }
}