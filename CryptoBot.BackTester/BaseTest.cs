using AutoMapper;
using CryptoBot.Core.Mapping;
using CryptoBot.IndicatrorEngine;

namespace CryptoBot.BackTester
{
    public class BaseTest
    {
        public IMapper _mapper;
        public IIndicatorFactoryWrapper _IndicatorFactoryWrapper;
        public BaseTest()
        {
            SetupMapping();
        }

        private void SetupMapping()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfigurator>();
            });

            _mapper = new Mapper(mapperConfig);

            _IndicatorFactoryWrapper = new IndicatorFactoryWrapper();
        }

    }
}
