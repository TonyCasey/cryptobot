using System;

namespace CryptoBot.Core.Utils
{
    public static class DateConversion
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UtcTimeStampToDateTime(double unixTimeStamputcMiliSeconds)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamputcMiliSeconds).ToLocalTime();
            return dtDateTime;
        }

        public static long DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            return (Int32)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }


    }
}
