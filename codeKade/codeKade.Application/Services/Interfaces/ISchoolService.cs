using codeKade.DataLayer.DTOs.School;
using codeKade.DataLayer.Entities.School;

namespace codeKade.Application.Services.Interfaces;

public interface ISchoolService : IAsyncDisposable
{
    Task<List<School>> GetAllSchools();

    Task<FilterSchoolDTO> GetAllFilter(FilterSchoolDTO filter);

    Task<bool> AddSchool(School school);

    Task<School> GetSchoolById(long id);

    Task<bool> EditSchool(School school);

    Task<bool> DeleteSchool(long id);
}