using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeAccountingApp
{
    public static class SchedulerConfig
    {
        // Время ежедневного обновления (9:00)
        public static TimeSpan DailyUpdateTime { get; } = TimeSpan.FromHours(9);
    }
}