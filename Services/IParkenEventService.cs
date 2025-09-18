using ParkenEvent.Models;

namespace ParkenEvent.Services
{
    public interface IParkenEventService
    {
        Task<EventResponse> GetTodaysEventAsync();
    }
}