using CryptoBot.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CryptoBot.Model.Domain.Market
{
    [Table("Trends", Schema = "Market")]
    public class Trend : BaseEntity
    {
        public Trend()
        {
            //Ticks = new List<Tick>();
        }

        public long TrendId { get; set; }

        public int BarCount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Enumerations.TrendDirectionEnum Direction { get; set; }

        //public virtual List<Tick> Ticks { get; set; }

        /// <summary>
        /// Calculate the entire % incresae from the start of the trend
        /// </summary>
        public double PercentageCount
        {
            get
            {
                //if (Ticks.Any())
                //{
                //    // get the opening figure
                //    //double opening = Ticks.First().Ohlc.Open;
                //    //double closing = Ticks.Last().Ohlc.Close;

                //    //return closing > opening ? 
                //    //    Calculations.Percentage(closing - opening, opening)
                //    //    :Calculations.Percentage(opening-closing, opening);
                //}
                return 0;
                    
            }
            set { throw new NotImplementedException(); }
        }

        public double SimpleMovingAverage(int size)
        {
            //Ticks.Reverse();
            //double result = Ticks.Take(size).Sum(x => x.Last) / size;
            //Ticks.Reverse();
            return 0;
        }
        
    }
}
