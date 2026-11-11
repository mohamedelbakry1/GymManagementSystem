using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using GymManagementSystemBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    internal class SessionService(IUnitOfWork _unitOfWork, IMapper _mapper) : ISessionService
    {
        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var Sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAndBookingsAsync(ct: ct);
            if (Sessions is null) return [];
            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(Sessions);
            return MappedSessions;
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int SessionId, CancellationToken ct = default)
        {
            var Session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAndBookingsAsync(SessionId, ct);
            if (Session is null) return null;
            var MappedSession = _mapper.Map<SessionViewModel>(Session);
            return MappedSession;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel createSession, CancellationToken ct = default)
        {
            try
            {
                if (! await IsTrainerExistsAsync(createSession.TrainerId, ct)) return false;

                if (! await IsCategoryExistsAsync(createSession.CategoryId, ct)) return false;

                if (!IsDateTimeValid(createSession.StartDate, createSession.EndDate)) return false;

                if(createSession.Capacity > 25 || createSession.Capacity < 0) return false;

                var Session = _mapper.Map<Session>(createSession);

                _unitOfWork.GetRepository<Session>().Add(Session);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Create Session Failed {ex}");
                return false;
            }
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int SessionId, CancellationToken ct = default)
        {
            var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId, ct: ct);
            if (! await IsSessionAvailableToUpdateAsync(Session!, ct)) return null!;
            return _mapper.Map<UpdateSessionViewModel>(Session);
        }

        public async Task<bool> UpdateSessionAsync(int SessionId, UpdateSessionViewModel updateSession, CancellationToken ct = default)
        {
            try
            {
                var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId, ct: ct);
                if (! await IsSessionAvailableToUpdateAsync(Session!, ct)) return false;
                if (! await IsTrainerExistsAsync(updateSession.TrainerId, ct)) return false;
                if (!IsDateTimeValid(updateSession.StartDate, updateSession.EndDate)) return false;

                _mapper.Map(updateSession, Session);
                Session!.UpdatedAt = DateTime.Now;
                _unitOfWork.SessionRepository.Update(Session);

                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Session Failed {ex}");
                return false;
            }
        }

        public async Task<bool> RemoveSessionAsync(int SessionId, CancellationToken ct = default)
        {
            try
            {
                var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId, ct: ct);
                if (Session is null) return false;

                if (! await IsSessionAvailableToRemoveAsync(Session, ct)) return false;

                _unitOfWork.SessionRepository.Delete(Session);
                return  await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Remove Session Failed {ex}");
                return false;
            }
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync(CancellationToken ct = default)
        {
            var Trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct : ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(Trainers);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDownAsync(CancellationToken ct = default)
        {
            var Categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(Categories);
        }

        #region Helper Methods
        private async Task<bool> IsTrainerExistsAsync(int TrainerId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct: ct) is not null;
        }
        private async Task<bool> IsCategoryExistsAsync(int CategoryId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Category>().GetByIdAsync(CategoryId, ct: ct) is not null;
        }
        private bool IsDateTimeValid(DateTime StartDate, DateTime EndDate)
        {
            return StartDate < EndDate && DateTime.Now < StartDate;
        }
        private async Task<bool> IsSessionAvailableToUpdateAsync(Session session, CancellationToken ct = default)
        {
            if (session is null) return false;
            if(session.EndDate < DateTime.Now) return false;
            if(session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) return false;

            var HasActiveBookings = await _unitOfWork.SessionRepository.GetCountOfBookingsAsync(session.Id, ct) > 0;
            if (HasActiveBookings) return false;

            return true;
        }
        private async Task<bool> IsSessionAvailableToRemoveAsync(Session session, CancellationToken ct = default)
        {
            if (session is null) return false;
            if (session.StartDate > DateTime.Now) return false;
            if(session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) return false;

            var HasActiveBookings = await _unitOfWork.SessionRepository.GetCountOfBookingsAsync(session.Id, ct) > 0;
            if (HasActiveBookings) return false;
            return true;
        }
        #endregion
    }
}
