using System;

namespace Noteapp.Core.Interfaces
{
    public interface IDateTimeProvider
    {
        public DateTime Now { get; }
    }
}
