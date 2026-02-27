using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AnalyticsViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Classes
{
    public class AnalyticsService(IUnitOfWork _unitOfWork) : IAnalyticsService
    {
        public async Task<AnalyticsViewModel?> GetAnalyticsData()
        {
            var Sessions = await _unitOfWork.SessionRepository.GetAllAsync();

            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
            var Trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();
            var Memberships = await _unitOfWork.GetRepository<Membership>().GetAllAsync(X => X.EndDate > DateTime.Now);

            return new AnalyticsViewModel()
            {
                TotalMembers = Members.Count(),
                ActiveMembers = Memberships.Count(),
                TotalTrainers = Trainers.Count(),
                UpcomingSessions = Sessions.Count(X => X.StartDate > DateTime.Now),
                OngoingSessions = Sessions.Count(X => X.StartDate <= DateTime.Now && X.EndDate > DateTime.Now),
                CompletedSessions = Sessions.Count(X => X.EndDate < DateTime.Now)
            };
        }
    }
}
