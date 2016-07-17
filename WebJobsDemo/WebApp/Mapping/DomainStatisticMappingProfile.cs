using System;
using AutoMapper;
using WebApp.Models;
using WebJobDemo.Core.Data.Models;

namespace WebApp.Mapping
{
    public class DomainStatisticMappingProfile : Profile
    {
        private readonly TimeZoneInfo _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        protected override void Configure()
        {
            CreateMap<DomainStatistic, DomainStatisticsViewModel>()
                .ForMember(d => d.LastUpdated, m => m.MapFrom(s => TimeZoneInfo.ConvertTimeFromUtc(s.LastUpdated, _localTimeZone)));
        }
    }
}