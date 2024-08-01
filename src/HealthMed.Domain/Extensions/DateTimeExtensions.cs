using System;

namespace HealthMed.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        #region Extension Methods

        public static bool IsDefault(this DateTime source)
            => source == default;

        public static DateTime WithoutSeconds(this DateTime source)
            => new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, 0);

        #endregion
    }
}
