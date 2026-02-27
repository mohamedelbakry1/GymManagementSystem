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
    public class SessionService(IUnitOfWork _unitOfWork, IMapper _mapper) : ISessionService
    {
        public async Task<IEnumerable<SessionViewModel>> GetAllSessions()
        {
            var Sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync();
            if (Sessions is null) return [];
            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(Sessions);
            foreach (var session in MappedSessions)
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookingsAsync(session.Id);
            return MappedSessions;
        }

        public async Task<SessionViewModel?> GetSessionById(int SessionId)
        {
            var Session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAsync(SessionId);
            if (Session is null) return null;
            var MappedSession = _mapper.Map<SessionViewModel>(Session);
            MappedSession.AvailableSlots = MappedSession.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookingsAsync(MappedSession.Id);
            return MappedSession;
        }

        public async Task<bool> CreateSession(CreateSessionViewModel createSession)
        {
            try
            {
                if (! await IsTrainerExists(createSession.TrainerId)) return false;

                if (! await IsCategoryExists(createSession.CategoryId)) return false;

                if (!IsDateTimeValid(createSession.StartDate, createSession.EndDate)) return false;

                if(createSession.Capacity > 25 || createSession.Capacity < 0) return false;

                var Session = _mapper.Map<Session>(createSession);

                await _unitOfWork.GetRepository<Session>().AddAsync(Session);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Create Session Failed {ex}");
                return false;
            }
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdate(int SessionId)
        {
            var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId);
            if (! await IsSessionAvailableToUpdate(Session!)) return null!;
            return _mapper.Map<UpdateSessionViewModel>(Session);
        }

        public async Task<bool> UpdateSession(int SessionId, UpdateSessionViewModel updateSession)
        {
            try
            {
                var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId);
                if (! await IsSessionAvailableToUpdate(Session!)) return false;
                if (! await IsTrainerExists(updateSession.TrainerId)) return false;
                if (!IsDateTimeValid(updateSession.StartDate, updateSession.EndDate)) return false;

                _mapper.Map(updateSession, Session);
                Session!.UpdatedAt = DateTime.Now;
                _unitOfWork.SessionRepository.Update(Session);

                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Session Failed {ex}");
                return false;
            }
        }

        public async Task<bool> RemoveSession(int SessionId)
        {
            try
            {
                var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId);
                if (Session is null) return false;

                if (! await IsSessionAvailableToRemove(Session)) return false;

                _unitOfWork.SessionRepository.Delete(Session);
                return  await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Remove Session Failed {ex}");
                return false;
            }
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown()
        {
            var Trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(Trainers);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDown()
        {
            var Categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(Categories);
        }

        #region Helper Methods
        private async Task<bool> IsTrainerExists(int TrainerId)
        {
            return await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId) is not null;
        }
        private async Task<bool> IsCategoryExists(int CategoryId)
        {
            return await _unitOfWork.GetRepository<Category>().GetByIdAsync(CategoryId) is not null;
        }
        private bool IsDateTimeValid(DateTime StartDate, DateTime EndDate)
        {
            return StartDate < EndDate && DateTime.Now < StartDate;
        }
        private async Task<bool> IsSessionAvailableToUpdate(Session session)
        {
            if (session is null) return false;
            if(session.EndDate < DateTime.Now) return false;
            if(session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) return false;

            var HasActiveBookings = await _unitOfWork.SessionRepository.GetCountOfBookingsAsync(session.Id) > 0;
            if (HasActiveBookings) return false;

            return true;
        }
        private async Task<bool> IsSessionAvailableToRemove(Session session)
        {
            if (session is null) return false;
            if (session.StartDate > DateTime.Now) return false;
            if(session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) return false;

            var HasActiveBookings = await _unitOfWork.SessionRepository.GetCountOfBookingsAsync(session.Id) > 0;
            if (HasActiveBookings) return false;
            return true;
        }
        #endregion
    }
}
