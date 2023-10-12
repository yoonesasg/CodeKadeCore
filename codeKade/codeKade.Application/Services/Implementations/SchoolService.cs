using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.School;
using codeKade.DataLayer.Entities.School;
using codeKade.DataLayer.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace codeKade.Application.Services.Implementations
{
    public class SchoolService : ISchoolService
    {
        private readonly IGenericRepository<School> _schoolRepository;
        private readonly IUserService _userService;

        public SchoolService(IGenericRepository<School> schoolRepository, IUserService userService)
        {
            _schoolRepository = schoolRepository;
            _userService = userService;
        }

        public async ValueTask DisposeAsync()
        {
            if (_schoolRepository != null)
            {
                await _schoolRepository.DisposeAsync();
            }
        }

        public async Task<List<School>> GetAllSchools()
        {
            return await _schoolRepository.GetEntityQuery().ToListAsync();
        }

        public async Task<FilterSchoolDTO> GetAllFilter(FilterSchoolDTO filter)
        {
            var query = _schoolRepository.GetEntityQuery().AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(s => EF.Functions.Like(s.Name, $"%{filter.Name}%"));
            }


            filter.SkipEntity = (filter.PageID - 1) * filter.TakeEntity;
            var entitiesCount = await query.CountAsync();
            filter.PageCount = (int)Math.Ceiling(entitiesCount / (double)filter.TakeEntity);

            var products = await query.OrderBy(s => s.ID).Skip(filter.SkipEntity).Take(filter.TakeEntity)
                .ToListAsync();
            filter.StartPage = filter.PageID - 3 > 0 ? filter.PageID - 3 : 1;
            filter.EndPage = filter.PageID + 3 <= filter.PageCount ? filter.PageID + 3 : filter.PageCount;
            filter.Schools= products;
            return filter;
        }

        public async Task<bool> AddSchool(School school)
        {
            await _schoolRepository.AddEntity(school);
            await _schoolRepository.SaveChanges();
            return true;
        }

        public async Task<School> GetSchoolById(long id)
        {
            return await _schoolRepository.GetByID(id);
        }

        public async Task<bool> EditSchool(School school)
        {
            _schoolRepository.EditEntity(school);
            await _schoolRepository.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteSchool(long id)
        {
            var school = await _schoolRepository.GetByID(id);
            if (school == null)
            {
                return false;
            }

            await _schoolRepository.DeleteEntity(id);
            await _schoolRepository.SaveChanges();
            return true;
        }

    }
}
