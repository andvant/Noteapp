using System;

namespace Noteapp.Api.Infrastructure
{
    public interface IDateTimeProvider
    {
        public DateTime Now { get; }
    }
}
