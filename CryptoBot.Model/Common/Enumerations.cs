namespace CryptoBot.Model.Common
{
    public static class Enumerations
    {
        
        public enum OrderSideEnum
        {
            Buy = 1,
            Sell
        }

        public enum OrderTypeEnum
        {
            Limit = 1, // DO NOT CHANGE THE TOP 3 HERE, their names are used in the Exchange API
            Stop,
            Market
        }

        public enum OrderStatusEnum
        {
            /// <summary>
            /// Order status is unknown
            /// </summary>
            Unknown,

            /// <summary>
            /// Order has been filled completely
            /// </summary>
            Filled,

            /// <summary>
            /// Order partially filled
            /// </summary>
            FilledPartially,

            /// <summary>
            /// Order is pending or open but no amount has been filled yet
            /// </summary>
            Pending,

            /// <summary>
            /// Error
            /// </summary>
            Error,

            /// <summary>
            /// Order was cancelled
            /// </summary>
            Canceled
        }

        public enum OrderRejectionReasonEnum
        {
            UnknownSymbol = 1,
            ExchangeClosed,
            OrderExceedsLimit,
            UnknownOrder,
            DuplicateOrder,
            UnsupportedOrder,
            UnknownAccount,
            Other
        }

        public enum TimeInForceEnum
        {
            GoodTilCanceled = 1,
            ImmediateOrCancel,
            FillOrKill,
            Day
        }

        public enum TrendDirectionEnum
        {
            None,
            Up,
            Down
        }

        public enum BidAskTradeEnum
        {
            Ask = 1,
            Bid,
            Trade
        }

        public enum CandleSizeEnum
        {
            OneMinute = 1,
            ThreeMinute = 3,
            FiveMinute = 5,
            TenMinute = 10,
            FifteenMinute = 15,
            ThirtyMinute = 30,
            SixtyMinute = 60,
            FourHours = 240,
            SixHours = 360,
            TwelveHours = 720,
            TwentyFourHours = 1440
        }

        public enum ExchangesEnum
        {
            Bittrex = 1,
            HitBTC,
            Gdax
        }

        public enum TradeTypeEnum
        {
            Buy = 1,
            Sell
        }

        public enum IndicatorSignalEnum
        {
            Sell,
            Buy,
            None
        }

        public enum SafetyPositionEnum
        {
            Broken,
            Ok
        }

        public enum BoughtSoldPositionEnum
        {
            Sold,
            Bought
        }

        public enum BotSettingsEnum
        {
            TargetPrice,
            BarSize,
        }

        public enum IndicatorTypeEnum
        {
            Macd = 1,
            Volume,
            CloseOpenCrossing,
            Stocastic
        }

        public enum SafetyTypeEnum
        {
            StopLoss = 1
        }

        public enum PositionStatusEnum
        {
            Bought,
            Sold
        }

        public enum RuleOperatorEnum
        {
            Equal,
            NotEqual,
            GreaterThan,
            LessThan
        }

        public enum SchedulerType
        {
            ScheduledEventOccursEveryValueInSeconds,            
            ScheduledEventOccursEveryValueInSecondsPastTheHour,
            ScheduledEventOccursOnceDailyValueInSecondsPastMidnight,
        }

        public enum MessagingAppEnum
        {
            Telegram = 1,
            Slack
        }

    }
}
