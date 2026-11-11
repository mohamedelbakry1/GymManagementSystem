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
    internal class BookingService(IUnitOfWork _unitOfWork, IMapper _mapper) : IBookingService
    {
        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsUpcomingAndOngoingAsync(CancellationToken ct = default)
        {
            var SessionRepo = _unitOfWork.SessionRepository;
            var Sessions = await SessionRepo
                .GetAllSessionsWithTrainerAndCategoryAndBookingsAsync(X => X.StartDate > DateTime.Now || X.StartDate < DateTime.Now && X.EndDate > DateTime.Now, ct);

            if (Sessions is null) return [];

            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(Sessions);
            return MappedSessions;
        }

        public async Task<IEnumerable<MemberBookingViewModel>> GetAllMembersForUpcomingSessionAsync(int SessionId, CancellationToken ct = default)
        {
            var Members = await _unitOfWork.BookingRepository.GetMembersInSessionAsync(SessionId, ct);
            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberBookingViewModel>>(Members);
        }

        public async Task<IEnumerable<MemberAttendanceViewModel>> GetAllMembersForOngoingSessionAsync(int SessionId, CancellationToken ct = default)
        {
            var Members = await _unitOfWork.BookingRepository.GetMembersInSessionAsync(SessionId, ct);
            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberAttendanceViewModel>>(Members);
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync(int Id, CancellationToken ct = default)
        {
            var Bookings = (await _unitOfWork.BookingRepository.GetAllAsync(X => X.Id == Id, ct))
                                                        .Select(X => X.MemberId)
                                                        .ToList();

            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync(X => !Bookings.Contains(X.Id), ct);

            if(Members is null) return [];

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(Members);
        }

        public async Task<bool> CreateBookingAsync(CreateBookingViewModel createBooking, CancellationToken ct = default)
        {
            var HasActiveMembership = (await _unitOfWork.MembershipRepository
                                            .GetAllAsync(x => x.MemberId == createBooking.MemberId && x.EndDate > DateTime.Now, ct)).Any();

            if(!HasActiveMembership) return false;

            var SessionRepo = _unitOfWork.SessionRepository;

            var Session = await SessionRepo.GetByIdAsync(createBooking.SessionId, X => X.StartDate > DateTime.Now, ct);
            if(Session is null) return false;

            var BookingsCount = await SessionRepo.GetCountOfBookingsAsync(createBooking.SessionId, ct);

            var AvailableSlots = Session.Capacity - BookingsCount;
            if(AvailableSlots == 0) return false;

            try
            {
                var Booking = _mapper.Map<Booking>(createBooking);
                Booking.IsAttended = false;

                _unitOfWork.BookingRepository.Add(Booking);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed To Create Booking {ex}");
                return false;
            }
        }

        public async Task<bool> MarkAttendanceAsync(int SessionId, int MemberId, CancellationToken ct = default)
        {
            try
            {
                var BookingRepo = _unitOfWork.BookingRepository;
                var Booking = (await BookingRepo
                             .GetAllAsync(X => X.SessionId == SessionId && X.MemberId == MemberId, ct))
                             .FirstOrDefault();

                if (Booking is null) return false;
                Booking.IsAttended = true;
                BookingRepo.Update(Booking);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To MarkAttendance: {ex}");
                return false;
            }
        }

        public async Task<bool> CancelBookingAsync(int SessionId, int MemberId, CancellationToken ct = default)
        {
            try
            {
                var BookingRepo = _unitOfWork.BookingRepository;
                var Booking = (await BookingRepo
                .GetAllAsync(X => X.SessionId == SessionId && X.MemberId == MemberId, ct))
                .FirstOrDefault();

                if (Booking is null) return false;

                var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId, ct: ct);

                if (Session is null || Session.StartDate <= DateTime.Now) return false;

                BookingRepo.Delete(Booking);
                return await _unitOfWork.SaveChangesAsync(ct) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To Cancel Booking: {ex}");
                return false;
            }
        }
    }
}
