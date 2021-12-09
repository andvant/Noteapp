using Noteapp.Core.Interfaces;
using System;

namespace Noteapp.Infrastructure
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
