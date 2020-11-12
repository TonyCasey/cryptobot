using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CryptoBot.Core.Mapping;
using CryptoBot.Database;
using CryptoBot.IndicatrorEngine;
using Moq;
using NLog;

namespace CryptoBot.Tests
{
    public class BaseTest
    {
        public Logger _logger;
        public IMapper _mapper;
        public Mock<CryptoBotDbContext> _dbContext;
        public Mock<IIndicatorFactoryWrapper> _iIndicatorFactoryWrapper;

        public BaseTest()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _dbContext = new Mock<CryptoBotDbContext>();
            _iIndicatorFactoryWrapper = new Mock<IIndicatorFactoryWrapper>();

            SetupMapping();
            
        }

        private void SetupMapping()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfigurator>();
            });

            _mapper = new Mapper(mapperConfig);

        }

    }
}
