using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.CryptoBot.Controllers;
using Api.CryptoBot.Models.DTO.Bot;
using Api.CryptoBot.Models.DTO.Chart;
using Api.CryptoBot.Models.DTO.Chart.Entities;
using Api.CryptoBot.Models.DTO.Coin;
using Api.CryptoBot.Models.DTO.Exchange;
using Api.CryptoBot.Models.DTO.Order;
using Api.CryptoBot.Models.DTO.Position;
using Api.CryptoBot.Models.DTO.Rule;
using Api.CryptoBot.Models.DTO.RuleSet;
using AutoMapper;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Domain.Trading;
using Bot = CryptoBot.Model.Domain.Bot.Bot;
using Exchange = CryptoBot.Model.Domain.Market.Exchange;

namespace Api.CryptoBot.Models.Mapping
{
    public class MappingConfigurator : Profile
    {
        public MappingConfigurator()
        {
            CreateMap<Coin, CoinResponseDto>();
            CreateMap<CoinRequestDto, Coin>()
                .ForMember(d => d.CoinId, opt => opt.Ignore());
            CreateMap<Bot, BotResponseDto>();
            CreateMap<BotRequestDto, Bot>()
                .ForMember(d => d.BotId, opt => opt.Ignore());
            CreateMap<OrderRequestDto, Order>()
                .ForMember(d => d.OrderId, opt => opt.Ignore());;
            CreateMap<Order, OrderResponseDto>();
            CreateMap<Rule, RuleResponseDto>();
            CreateMap<RuleRequestDto, Rule>()
                .ForMember(d => d.RuleId, opt => opt.Ignore());
            CreateMap<RuleSet, RuleSetResponseDto>();
            CreateMap<RuleSetRequestDto, RuleSet>()
                .ForMember(d => d.RuleSetId, opt => opt.Ignore());
            CreateMap<Symbol, SymbolsResponseDto>();
            CreateMap<Symbol, SymbolSearchResponseDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name));
            CreateMap<Exchange, ExchangeResponseDto>();
            CreateMap<ExchangeRequestDto, Exchange>()
                .ForMember(d => d.ExchangeId, opt => opt.Ignore());;
            CreateMap<Position, PositionResponseDto>();
            CreateMap<Position, PositionSearchResponseDto>();
            CreateMap<PositionRequestDto, Position>()
                .ForMember(d => d.PositionId, opt => opt.Ignore());

        }
    }
}
