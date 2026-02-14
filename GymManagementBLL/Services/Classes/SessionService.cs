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
        public IEnumerable<SessionViewModel> GetAllSessions()
        {
            var Sessions = _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory();
            if (Sessions is null) return [];
            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(Sessions);
            foreach (var session in MappedSessions)
                session.AvailableSlots = session.Capacity - _unitOfWork.SessionRepository.GetCountOfBookings(session.Id);
            return MappedSessions;
        }

        public SessionViewModel? GetSessionById(int SessionId)
        {
            var Session = _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategory(SessionId);
            if (Session is null) return null;
            var MappedSession = _mapper.Map<SessionViewModel>(Session);
            MappedSession.AvailableSlots = MappedSession.Capacity - _unitOfWork.SessionRepository.GetCountOfBookings(MappedSession.Id);
            return MappedSession;
        }

        public bool CreateSession(CreateSessionViewModel createSession)
        {
            try
            {
                if (!IsTrainerExists(createSession.TrainerId)) return false;

                if (!IsCategoryExists(createSession.CategoryId)) return false;

                if (!IsDateTimeValid(createSession.StartDate, createSession.EndDate)) return false;

                if(createSession.Capacity > 25 || createSession.Capacity < 0) return false;

                var Session = _mapper.Map<Session>(createSession);

                _unitOfWork.GetRepository<Session>().Add(Session);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Create Session Failed {ex}");
                return false;
            }
        }

        public UpdateSessionViewModel? GetSessionToUpdate(int SessionId)
        {
            var Session = _unitOfWork.SessionRepository.GetById(SessionId);
            if (IsSessionAvailableToUpdate(Session!)) return null!;
            return _mapper.Map<UpdateSessionViewModel>(Session);
        }

        public bool UpdateSession(int SessionId, UpdateSessionViewModel updateSession)
        {
            try
            {
                var Session = _unitOfWork.SessionRepository.GetById(SessionId);
                if (IsSessionAvailableToUpdate(Session!)) return false;
                if (IsTrainerExists(updateSession.TrainerId)) return false;
                if (IsDateTimeValid(updateSession.StartDate, updateSession.EndDate)) return false;

                _mapper.Map(updateSession, Session);
                Session!.UpdatedAt = DateTime.Now;
                _unitOfWork.SessionRepository.Update(Session);

                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Update Session Failed {ex}");
                return false;
            }
        }

        public bool RemoveSession(int SessionId)
        {
            try
            {
                var Session = _unitOfWork.SessionRepository.GetById(SessionId);
                if (Session is null) return false;

                if (!IsSessionAvailableToRemove(Session)) return false;

                _unitOfWork.SessionRepository.Delete(Session);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Remove Session Failed {ex}");
                return false;
            }
        }

        #region Helper Methods
        private bool IsTrainerExists(int TrainerId)
        {
            return _unitOfWork.GetRepository<Trainer>().GetById(TrainerId) is not null;
        }
        private bool IsCategoryExists(int CategoryId)
        {
            return _unitOfWork.GetRepository<Category>().GetById(CategoryId) is not null;
        }
        private bool IsDateTimeValid(DateTime StartDate, DateTime EndDate)
        {
            return StartDate < EndDate;
        }
        private bool IsSessionAvailableToUpdate(Session session)
        {
            if (session is null) return false;
            if(session.EndDate < DateTime.Now) return false;
            if(session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) return false;

            var HasActiveBookings = _unitOfWork.SessionRepository.GetCountOfBookings(session.Id) > 0;
            if (HasActiveBookings) return false;

            return true;
        }
        private bool IsSessionAvailableToRemove(Session session)
        {
            if (session is null) return false;
            if (session.StartDate > DateTime.Now) return false;
            if(session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) return false;

            var HasActiveBookings = _unitOfWork.SessionRepository.GetCountOfBookings(session.Id) > 0;
            if (HasActiveBookings) return false;
            return true;
        }
        #endregion
    }
}
