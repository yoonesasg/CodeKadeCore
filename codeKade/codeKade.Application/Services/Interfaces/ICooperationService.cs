using codeKade.DataLayer.DTOs.Cooperarion;
using codeKade.DataLayer.Entities.Cooperation;

namespace codeKade.Application.Services.Interfaces;

public interface ICooperationService :IAsyncDisposable
{
    Task AddCooperation(Cooperation add);

    Task<bool> CheckCooperation(long UserId);

    Task<FilterCooperationDTO> GetAll(FilterCooperationDTO filter);

    Task<Cooperation> GetCooperation(long id);

    Task DeleteCooperation(long id);
}