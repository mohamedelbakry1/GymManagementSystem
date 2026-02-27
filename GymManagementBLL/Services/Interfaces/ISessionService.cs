using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementSystemBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessions();
        Task<SessionViewModel?> GetSessionById(int SessionId);
        Task<bool> CreateSession(CreateSessionViewModel createSession);
        Task<UpdateSessionViewModel?> GetSessionToUpdate(int SessionId);
        Task<bool> UpdateSession(int SessionId, UpdateSessionViewModel updateSession);
        Task<bool> RemoveSession(int SessionId);
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown();
        Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDown();
    }
}
