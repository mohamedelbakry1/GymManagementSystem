using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    public class BookingService(IUnitOfWork _unitOfWork, IMapper _mapper) : IBookingService
    {
        public IEnumerable<SessionViewModel> GetAllSessionsUpcomingAndOngoing()
        {
            var SessionRepo = _unitOfWork.SessionRepository;
            var Sessions = SessionRepo
                .GetAllSessionsWithTrainerAndCategory(X => X.StartDate > DateTime.Now || X.StartDate < DateTime.Now && X.EndDate > DateTime.Now);

            if (Sessions is null) return [];

            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(Sessions);

            foreach(var Session in MappedSessions)
                Session.AvailableSlots = Session.Capacity - SessionRepo.GetCountOfBookings(Session.Id);

            return MappedSessions;
        }

        public IEnumerable<MemberBookingViewModel> GetAllMembersForUpcomingSession(int SessionId)
        {
            var Members = _unitOfWork.BookingRepository.GetMembersInSession(SessionId);
            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberBookingViewModel>>(Members);
        }

        public IEnumerable<MemberAttendanceViewModel> GetAllMembersForOngoingSession(int SessionId)
        {
            var Members = _unitOfWork.BookingRepository.GetMembersInSession(SessionId);
            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberAttendanceViewModel>>(Members);
        }

        public IEnumerable<MemberSelectViewModel> GetMembersForDropDown(int Id)
        {
            var Bookings = _unitOfWork.BookingRepository.GetAll(X => X.Id == Id)
                                                        .Select(X => X.MemberId)
                                                        .ToList();

            var Members = _unitOfWork.GetRepository<Member>().GetAll(X => !Bookings.Contains(X.Id));

            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(Members);
        }

        public bool CreateBooking(CreateBookingViewModel createBooking)
        {
            var HasActiveMembership = _unitOfWork.MembershipRepository.GetById(createBooking.MemberId, X => X.Status == "Active");
            if(HasActiveMembership is null) return false;

            var SessionRepo = _unitOfWork.SessionRepository;

            var Session = SessionRepo.GetById(createBooking.SessionId, X => X.StartDate > DateTime.Now);
            if(Session is null) return false;

            var BookingsCount = SessionRepo.GetCountOfBookings(createBooking.SessionId);

            var AvailableSlots = Session.Capacity - BookingsCount;
            if(AvailableSlots == 0) return false;

            try
            {
                var Booking = _mapper.Map<Booking>(createBooking);
                Booking.IsAttended = false;

                _unitOfWork.BookingRepository.Add(Booking);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed To Create Booking {ex}");
                return false;
            }
        }

        public bool MarkAttendance(int SessionId, int MemberId)
        {
            try
            {
                var BookingRepo = _unitOfWork.BookingRepository;
                var Booking = BookingRepo
                .GetAll(X => X.SessionId == SessionId && X.MemberId == MemberId)
                .FirstOrDefault();
                if (Booking is null) return false;
                Booking.IsAttended = true;
                BookingRepo.Update(Booking);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To MarkAttendance: {ex}");
                return false;
            }
        }

        public bool CancelBooking(int SessionId, int MemberId)
        {
            try
            {
                var BookingRepo = _unitOfWork.BookingRepository;
                var Booking = BookingRepo
                .GetAll(X => X.SessionId == SessionId && X.MemberId == MemberId)
                .FirstOrDefault();

                if (Booking is null) return false;

                var Session = _unitOfWork.SessionRepository.GetById(SessionId);

                if (Session is null || Session.StartDate <= DateTime.Now) return false;

                BookingRepo.Delete(Booking);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To Cancel Booking: {ex}");
                return false;
            }
        }
    }
}
