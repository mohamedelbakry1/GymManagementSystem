using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AnalyticsViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    internal class AnalyticsService(IUnitOfWork _unitOfWork) : IAnalyticsService
    {
        public async Task<AnalyticsViewModel?> GetAnalyticsDataAsync(CancellationToken ct = default)
        {
            var sessionRepo = _unitOfWork.GetRepository<Session>();
            var upcomingSessions = await sessionRepo.CountAsync(x => x.StartDate > DateTime.Now, ct);
            var ongoingSessions = await sessionRepo.CountAsync(x => x.StartDate <= DateTime.Now && x.EndDate > DateTime.Now, ct);
            var completedSessions = await sessionRepo.CountAsync(x => x.EndDate < DateTime.Now, ct);

            var Members = await _unitOfWork.GetRepository<Member>().CountAsync(ct: ct);
            var Trainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var Memberships = await _unitOfWork.GetRepository<Membership>().CountAsync(X => X.EndDate > DateTime.Now, ct: ct);

            return new AnalyticsViewModel()
            {
                TotalMembers = Members,
                ActiveMembers = Memberships,
                TotalTrainers = Trainers,
                UpcomingSessions = upcomingSessions,
                OngoingSessions = ongoingSessions,
                CompletedSessions = completedSessions
            };
        }
    }
}
