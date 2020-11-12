using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;
using CryptoBot.Model.Domain.Market;
using CryptoBot.Model.Exchanges;

namespace CryptoBot.Core.Trading
{
    public interface ITrader
    {
        
        Task<ExchangeOrderResult> Buy(Bot bot, decimal price);
        Task<ExchangeOrderResult> Sell(Bot bot, decimal price);
        Task<ExchangeOrderResult> GetOrderDetails(string orderId);
        void CancelOrder(string orderId);
        Ticker GetLatestTicker(Coin baseCoin, Coin coin);
    }
}
