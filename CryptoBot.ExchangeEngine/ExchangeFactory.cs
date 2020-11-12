using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Common;
using CryptoBot.Model.Exchanges;

namespace CryptoBot.ExchangeEngine
{
    public static class ExchangeFactory
    {
        public static IExchangeApi GetExchangeApi(Enumerations.ExchangesEnum exchange, ExchangeSettings exchangeSettings)
        {
            switch (exchange)
            {
                case Enumerations.ExchangesEnum.Gdax:
                    return new ExchangeGdaxAPI(exchangeSettings);
                case Enumerations.ExchangesEnum.Bittrex:
                    return new ExchangeBittrexAPI(exchangeSettings);
                default:
                    return null;
            }
        }
    }
}
