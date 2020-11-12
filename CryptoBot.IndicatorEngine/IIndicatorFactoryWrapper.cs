using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using NLog;

namespace CryptoBot.IndicatrorEngine
{
    public interface IIndicatorFactoryWrapper
    {
        IIndicatorEngine GetIndicator(Enumerations.IndicatorTypeEnum indicatorType, Logger logger);
    }
}
