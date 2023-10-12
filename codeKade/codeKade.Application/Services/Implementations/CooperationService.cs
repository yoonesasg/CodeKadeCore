using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Cooperarion;
using codeKade.DataLayer.Entities.Cooperation;
using codeKade.DataLayer.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace codeKade.Application.Services.Implementations
{
    public class CooperationService : ICooperationService
    {
        private readonly IGenericRepository<Cooperation> _cooperationRepository;

        public CooperationService(IGenericRepository<Cooperation> cooperationRepository)
        {
            _cooperationRepository = cooperationRepository;
        }

        public async ValueTask DisposeAsync()
        {
            if (_cooperationRepository != null)
            {
                await _cooperationRepository.DisposeAsync();
            }
        }

        public async Task AddCooperation(Cooperation add)
        {
            if (add != null)
            {
                await _cooperationRepository.AddEntity(add);
                await _cooperationRepository.SaveChanges();
            }
        }

        public async Task<bool> CheckCooperation(long UserId)
        {
            return await _cooperationRepository.GetEntityQuery().AnyAsync(c => c.UserId == UserId && !c.IsDelete);
        }

        public async Task<FilterCooperationDTO> GetAll(FilterCooperationDTO filter)
        {
            var query = _cooperationRepository.GetEntityQuery().Include(c => c.User).AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(s => EF.Functions.Like(s.User.FirstName + " " + s.User.LastName, $"%{filter.Name}%"));
            }
            filter.SkipEntity = (filter.PageID - 1) * filter.TakeEntity;
            var entitiesCount = await query.CountAsync();
            filter.PageCount = (int)Math.Ceiling(entitiesCount / (double)filter.TakeEntity);

            var products = await query.OrderBy(s=>!s.Seen).Skip(filter.SkipEntity).Take(filter.TakeEntity)
                .ToListAsync();
            filter.StartPage = filter.PageID - 3 > 0 ? filter.PageID - 3 : 1;
            filter.EndPage = filter.PageID + 3 <= filter.PageCount ? filter.PageID + 3 : filter.PageCount;
            filter.Cooperations = products;
            return filter;
        }

        public async Task<Cooperation> GetCooperation(long id)
        {
            var model = await _cooperationRepository.GetByID(id);
            model.Seen = true;
            _cooperationRepository.EditEntity(model);
            await _cooperationRepository.SaveChanges();
            return model;
        }

        public async Task DeleteCooperation(long id)
        {
            await _cooperationRepository.DeleteEntity(id);
            await _cooperationRepository.SaveChanges();
        }
    }
}
