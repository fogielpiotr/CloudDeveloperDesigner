using Application.Common.Interfaces;

namespace Application.Common.Services
{
    public class Clock : IClock
    {
        public DateTimeOffset CurrentDate() => DateTimeOffset.UtcNow;
    }
}
