using System.Collections.Generic;
using AutoMapper;

namespace WebApp.App_Start
{
    public static class MapperConfig
    {
        public static IMapper Register(IEnumerable<Profile> profiles, bool verifyMaps = true)
        {
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            if (verifyMaps)
            {
                config.AssertConfigurationIsValid();
            }

            var mapper = config.CreateMapper();

            return mapper;
        }
    }
}