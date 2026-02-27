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
        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsUpcomingAndOngoing()
        {
            var SessionRepo = _unitOfWork.SessionRepository;
            var Sessions = await SessionRepo
                .GetAllSessionsWithTrainerAndCategoryAsync(X => X.StartDate > DateTime.Now || X.StartDate < DateTime.Now && X.EndDate > DateTime.Now);

            if (Sessions is null) return [];

            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(Sessions);

            foreach(var Session in MappedSessions)
                Session.AvailableSlots = Session.Capacity - await SessionRepo.GetCountOfBookingsAsync(Session.Id);

            return MappedSessions;
        }

        public async Task<IEnumerable<MemberBookingViewModel>> GetAllMembersForUpcomingSession(int SessionId)
        {
            var Members = await _unitOfWork.BookingRepository.GetMembersInSessionAsync(SessionId);
            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberBookingViewModel>>(Members);
        }

        public async Task<IEnumerable<MemberAttendanceViewModel>> GetAllMembersForOngoingSession(int SessionId)
        {
            var Members = await _unitOfWork.BookingRepository.GetMembersInSessionAsync(SessionId);
            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberAttendanceViewModel>>(Members);
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDown(int Id)
        {
            var Bookings = (await _unitOfWork.BookingRepository.GetAllAsync(X => X.Id == Id))
                                                        .Select(X => X.MemberId)
                                                        .ToList();

            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync(X => !Bookings.Contains(X.Id));

            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(Members);
        }

        public async Task<bool> CreateBooking(CreateBookingViewModel createBooking)
        {
            var HasActiveMembership = await _unitOfWork.MembershipRepository
                                            .GetByIdAsync(createBooking.MemberId, X => X.EndDate > DateTime.Now);

            if(HasActiveMembership is null) return false;

            var SessionRepo = _unitOfWork.SessionRepository;

            var Session = await SessionRepo.GetByIdAsync(createBooking.SessionId, X => X.StartDate > DateTime.Now);
            if(Session is null) return false;

            var BookingsCount = await SessionRepo.GetCountOfBookingsAsync(createBooking.SessionId);

            var AvailableSlots = Session.Capacity - BookingsCount;
            if(AvailableSlots == 0) return false;

            try
            {
                var Booking = _mapper.Map<Booking>(createBooking);
                Booking.IsAttended = false;

                await _unitOfWork.BookingRepository.AddAsync(Booking);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed To Create Booking {ex}");
                return false;
            }
        }

        public async Task<bool> MarkAttendance(int SessionId, int MemberId)
        {
            try
            {
                var BookingRepo = _unitOfWork.BookingRepository;
                var Booking = (await BookingRepo
                             .GetAllAsync(X => X.SessionId == SessionId && X.MemberId == MemberId))
                             .FirstOrDefault();

                if (Booking is null) return false;
                Booking.IsAttended = true;
                BookingRepo.Update(Booking);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To MarkAttendance: {ex}");
                return false;
            }
        }

        public async Task<bool> CancelBooking(int SessionId, int MemberId)
        {
            try
            {
                var BookingRepo = _unitOfWork.BookingRepository;
                var Booking = (await BookingRepo
                .GetAllAsync(X => X.SessionId == SessionId && X.MemberId == MemberId))
                .FirstOrDefault();

                if (Booking is null) return false;

                var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId);

                if (Session is null || Session.StartDate <= DateTime.Now) return false;

                BookingRepo.Delete(Booking);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To Cancel Booking: {ex}");
                return false;
            }
        }
    }
}
