using codeKade.DataLayer.DTOs.Event;
using codeKade.DataLayer.Entities.Event;

namespace codeKade.Application.Services.Interfaces;

public interface IEventService : IAsyncDisposable
{
    Task<List<Event>> GetAll();

    Task<FilterEventDTO> GetAll(FilterEventDTO filter);

    Task<Event> GetActiveEvent();

    Task<bool> DeleteEvent(long id);

    Task<bool> RestoreEvent(long id);

    Task<List<Event>> GetNewEvents();

    Task<Event> GetSingleEvent(long id);

    Task<Event> GetBigEvent();

    Task<bool> DisActivePastEvents(DateTime today);

    Task<bool> AddEvent(AddEventDTO add);

    Task<EditEventDTO> GetForEdit(long id);

    Task<bool> EditEvent(EditEventDTO edit);

    Task AddSeenToEvent(long id);

    Task<FilterEventDTO> GetDeletedBlogs(FilterEventDTO filter);

    Task<bool> BuyEvent(long EventId, long UserId);

    Task<bool> IsUserHaveEvent(long UserId,long EventId);

    Task<FilterEventBuyDTO> GetEventBuys(long? EventId,FilterEventBuyDTO filter);

    Task<long> DeleteEventBuy(long id);


}