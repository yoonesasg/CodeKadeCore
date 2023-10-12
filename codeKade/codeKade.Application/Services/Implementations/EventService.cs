using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeKade.Application.PathExtentions;
using codeKade.Application.Security;
using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Event;
using codeKade.DataLayer.Entities.Blog;
using codeKade.DataLayer.Entities.Event;
using codeKade.DataLayer.Repository.Interfaces;
using LightElectric.Application.Extensions;
using Microsoft.EntityFrameworkCore;

namespace codeKade.Application.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IGenericRepository<Event> _eventRepository;
        private readonly IGenericRepository<EventBuy> _eventBuyRepository;

        public EventService(IGenericRepository<Event> eventRepository, IGenericRepository<EventBuy> eventBuyRepository)
        {
            _eventRepository = eventRepository;
            _eventBuyRepository = eventBuyRepository;
        }

        public async ValueTask DisposeAsync()
        {
            if (_eventRepository != null)
            {
                await _eventRepository.DisposeAsync();
            }
        }

        public Task<List<Event>> GetAll()
        {
            return _eventRepository.GetEntityQuery().ToListAsync();
        }

        public async Task<FilterEventDTO> GetAll(FilterEventDTO filter)
        {

            var query = _eventRepository.GetEntityQuery().AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => EF.Functions.Like(s.Title, $"%{filter.Title}%"));
            }


            filter.SkipEntity = (filter.PageID - 1) * filter.TakeEntity;
            var entitiesCount = await query.CountAsync();
            filter.PageCount = (int)Math.Ceiling(entitiesCount / (double)filter.TakeEntity);

            var products = await query.OrderBy(s => s.ID).Skip(filter.SkipEntity).Take(filter.TakeEntity)
                .ToListAsync();
            filter.StartPage = filter.PageID - 3 > 0 ? filter.PageID - 3 : 1;
            filter.EndPage = filter.PageID + 3 <= filter.PageCount ? filter.PageID + 3 : filter.PageCount;
            filter.Events = products;
            return filter;
        }

        public async Task<Event> GetActiveEvent()
        {
            var ActiveEvent = await _eventRepository.GetEntityQuery().OrderByDescending(e => e.ID)
                .FirstOrDefaultAsync(e => e.IsActive);

            return ActiveEvent;
        }

        public async Task<bool> DeleteEvent(long id)
        {
            if (id != 0)
            {
                var blog = await _eventRepository.GetByID(id);
                if (blog == null)
                {
                    return false;
                }

                blog.IsDelete = true;
                _eventRepository.EditEntity(blog);
                await _eventRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RestoreEvent(long id)
        {
            if (id != 0)
            {
                var blog = await _eventRepository.GetEntityQuery().IgnoreQueryFilters().SingleOrDefaultAsync(e=>e.ID == id);
                if (blog == null)
                {
                    return false;
                }

                blog.IsDelete = false;
                _eventRepository.EditEntity(blog);
                await _eventRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Event>> GetNewEvents()
        {
            return await _eventRepository.GetEntityQuery().Where(e=>e.IsActive).Take(3).ToListAsync();
        }

        public async Task<Event> GetSingleEvent(long id)
        {
            return await _eventRepository.GetByID(id);
        }

        public async Task<Event> GetBigEvent()
        {
            return await _eventRepository.GetEntityQuery().FirstOrDefaultAsync(e => e.IsActive);
        }

        public async Task<bool> DisActivePastEvents(DateTime today)
        {
            var activeEvents = await _eventRepository.GetEntityQuery().Where(e => e.IsActive).ToListAsync();

            foreach (var single_event in activeEvents)
            {
                if (single_event.StartDate.Date < DateTime.Now.Date)
                {
                    single_event.IsActive = false;
                    _eventRepository.EditEntity(single_event);
                }
            }

            await _eventRepository.SaveChanges();
            return true;
        }

        public async Task<bool> AddEvent(AddEventDTO add)
        {
            if (add.Image != null && add.Image.IsImage())
            {
                var ImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(add.Image.FileName);
                add.Image.AddImageToServer(ImageName, PathTools.EventImageUpload, 150, 100, PathTools.EventThumbImageUpload);
                var new_event = new Event()
                {
                    StartDate = add.StartDate,
                    IsActive = true,
                    Price = add.Price,
                    LocationName = add.LocationName,
                    Address = add.Address,
                    Description = add.Description,
                    IsDelete = false,
                    Title = add.Title,
                    ImageName = ImageName
                };
                await _eventRepository.AddEntity(new_event);
                await _eventRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<EditEventDTO> GetForEdit(long id)
        {
            var Base_Event = await _eventRepository.GetByID(id);
            var Event = new EditEventDTO()
            {
                Id = id,
                Address = Base_Event.Address,
                Description = Base_Event.Description,
                IsActive = Base_Event.IsActive,
                LocationName = Base_Event.LocationName,
                Price = Base_Event.Price,
                StartDate = Base_Event.StartDate,
                Title = Base_Event.Title,
                ImageName = Base_Event.ImageName
            };
            return Event;
        }

        public async Task<bool> EditEvent(EditEventDTO edit)
        {
            if (edit == null)
            {
                return false;
            }
            var base_event = await _eventRepository.GetByID(edit.Id);
            if (base_event == null)
            {
                return false;
            }
            if (edit.Image != null)
            {
                var ImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(edit.Image.FileName);
                edit.Image.AddImageToServer(ImageName, PathTools.EventImageUpload, 150, 100, PathTools.EventThumbImageUpload, base_event.ImageName);

                base_event.Address = edit.Address;
                base_event.Description = edit.Description;
                base_event.IsActive = edit.IsActive;
                base_event.LocationName = edit.LocationName;
                base_event.Price = edit.Price;
                base_event.StartDate = edit.StartDate;
                base_event.Title = edit.Title;
                base_event.ImageName = ImageName;


                _eventRepository.EditEntity(base_event);
                await _eventRepository.SaveChanges();
            }
            else
            {
                base_event.Address = edit.Address;
                base_event.Description = edit.Description;
                base_event.IsActive = edit.IsActive;
                base_event.LocationName = edit.LocationName;
                base_event.Price = edit.Price;
                base_event.StartDate = edit.StartDate;
                base_event.Title = edit.Title;

                _eventRepository.EditEntity(base_event);
                await _eventRepository.SaveChanges();
            }

            return true;
        }

        public async Task AddSeenToEvent(long id)
        {
            var blog = await _eventRepository.GetByID(id);
            blog.Seen += 1;
            _eventRepository.EditEntity(blog);
            await _eventRepository.SaveChanges();
        }

        public async Task<FilterEventDTO> GetDeletedBlogs(FilterEventDTO filter)
        {
            var query = _eventRepository.GetEntityQuery().AsQueryable().IgnoreQueryFilters().Where(b => b.IsDelete);

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => EF.Functions.Like(s.Title, $"%{filter.Title}%"));
            }


            filter.SkipEntity = (filter.PageID - 1) * filter.TakeEntity;
            var entitiesCount = await query.CountAsync();
            filter.PageCount = (int)Math.Ceiling(entitiesCount / (double)filter.TakeEntity);

            var products = await query.OrderBy(s => s.ID).Skip(filter.SkipEntity).Take(filter.TakeEntity)
                .ToListAsync();
            filter.StartPage = filter.PageID - 3 > 0 ? filter.PageID - 3 : 1;
            filter.EndPage = filter.PageID + 3 <= filter.PageCount ? filter.PageID + 3 : filter.PageCount;
            filter.Events = products;
            return filter;
        }

        public async Task<bool> BuyEvent(long EventId, long UserId)
        {
            if (EventId == 0 || UserId == 0)
            {
                return false;
            }
            var EventBuy = new EventBuy()
            {
                EventId = EventId,
                UserId = UserId,
                IsDelete = false
            };

            await _eventBuyRepository.AddEntity(EventBuy);
            await _eventBuyRepository.SaveChanges();
            return true;
        }

        public async Task<bool> IsUserHaveEvent(long UserId, long EventId)
        {
            return await _eventBuyRepository.GetEntityQuery()
                .AnyAsync(eb => eb.EventId == EventId && eb.UserId == UserId);
        }

        public async Task<FilterEventBuyDTO> GetEventBuys(long? EventId,FilterEventBuyDTO filter)
        {
            var query = _eventBuyRepository.GetEntityQuery().Where(er=>er.EventId == EventId).Include(eb=>eb.User).AsQueryable().IgnoreQueryFilters().Where(b => !b.IsDelete);

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(s => EF.Functions.Like(s.User.FirstName + " " +s.User.LastName, $"%{filter.FullName}%"));
            }
            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(s => EF.Functions.Like(s.User.Email, $"%{filter.Email}%"));
            }
            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                query = query.Where(s => EF.Functions.Like(s.User.Mobile, $"%{filter.PhoneNumber}%"));
            }

            filter.SkipEntity = (filter.PageID - 1) * filter.TakeEntity;
            var entitiesCount = await query.CountAsync();
            filter.PageCount = (int)Math.Ceiling(entitiesCount / (double)filter.TakeEntity);

            var products = await query.OrderBy(s => s.ID).Skip(filter.SkipEntity).Take(filter.TakeEntity)
                .ToListAsync();
            filter.StartPage = filter.PageID - 3 > 0 ? filter.PageID - 3 : 1;
            filter.EndPage = filter.PageID + 3 <= filter.PageCount ? filter.PageID + 3 : filter.PageCount;
            filter.EventBuys = products;
            return filter;
        }

        public async Task<long> DeleteEventBuy(long id)
        {
            var eventBuy = await _eventBuyRepository.GetByID(id);

            if (eventBuy == null)
            {
                return 0;
            }

            eventBuy.IsDelete = true;
            _eventBuyRepository.EditEntity(eventBuy);
            await _eventBuyRepository.SaveChanges();
            return id;
        }
    }
}
