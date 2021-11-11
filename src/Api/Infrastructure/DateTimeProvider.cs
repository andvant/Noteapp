using System;

namespace Noteapp.Api.Infrastructure
{
    public class DateTimeProvider : IDateTimeProvider
    {
        private readonly DateTime? _dateTime;
        public DateTime Now => _dateTime ?? DateTime.Now;

        public DateTimeProvider(DateTime? dateTime = null)
        {
            _dateTime = dateTime;
        }
    }
}
