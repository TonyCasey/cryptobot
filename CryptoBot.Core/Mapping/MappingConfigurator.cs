using System;
using AutoMapper;
using AutoMapper.Configuration;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Account;
using CryptoBot.Model.Domain.Trading;
using CryptoBot.Model.Exchanges;

namespace CryptoBot.Core.Mapping
{
    public class MappingConfigurator : MapperConfigurationExpression
    {
        public MappingConfigurator()
        {
            Initiallize();
        }

        private void Initiallize()
        {
            CreateMap<ExchangeOrderResult, Order>()
                .ForMember(m => m.OurRef,
                    dest => dest.UseValue(Guid.NewGuid()))
                .ForMember(m => m.OrderId,
                    dest => dest.Ignore())
                .ForMember(m => m.ExchangeOrderId,
                    dest => dest.MapFrom(src => src.OrderId ))
                .ForMember(m => m.Side,
                    dest => dest.MapFrom(src => src.IsBuy ? Enumerations.OrderSideEnum.Buy:Enumerations.OrderSideEnum.Sell))
                .ForMember(m => m.TimeStamp,
                    dest => dest.MapFrom(src => src.OrderDate))
                .ForMember(m => m.Status,
                    dest => dest.MapFrom(src => src.Result))
                .ForMember(m => m.Quantity,
                    dest => dest.MapFrom(src => src.Amount))
                .ForMember(m => m.QuantityFilled,
                    dest => dest.MapFrom(src => src.AmountFilled))
                .ForMember(m => m.Status,
                    dest => dest.MapFrom(src => src.Result))
                .ForMember(m => m.Price,
                    dest => dest.MapFrom(src => src.AveragePrice))
                ;

            CreateMap<ApiSetting, ExchangeSettings>()
                .ForMember(m => m.ApiKey,
                    dest => dest.MapFrom(src => src.Key))
                .ForMember(m => m.PassPhrase,
                    dest => dest.MapFrom(src => src.Passphrase))
                .ForMember(m => m.Simulate,
                    dest => dest.MapFrom(src => src.Simulated))
                ;

        }
    }
}
