using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementSystemBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default);
        Task<SessionViewModel?> GetSessionByIdAsync(int SessionId, CancellationToken ct = default);
        Task<bool> CreateSessionAsync(CreateSessionViewModel createSession, CancellationToken ct = default);
        Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int SessionId, CancellationToken ct = default);
        Task<bool> UpdateSessionAsync(int SessionId, UpdateSessionViewModel updateSession, CancellationToken ct = default);
        Task<bool> RemoveSessionAsync(int SessionId, CancellationToken ct = default);
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDownAsync(CancellationToken ct = default);
    }
}
