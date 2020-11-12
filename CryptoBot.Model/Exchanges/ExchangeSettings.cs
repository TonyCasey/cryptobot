namespace CryptoBot.Model.Exchanges
{
    public class ExchangeSettings
    {
        public string Url { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public string PassPhrase { get; set; }
        public string QuoteCurrency  { get; set; }
        public bool Simulate { get; set; }
        public double CommissionRate { get; set; }
        public string SocketUrl { get; set; }
    }
}
