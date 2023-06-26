using System;

namespace Refactoring.LegacyService.Services
{
    public class DateTImeProvider : IDateTimeProvider
    {
        public DateTime DateTimeNow => DateTime.Now;

    }
}
