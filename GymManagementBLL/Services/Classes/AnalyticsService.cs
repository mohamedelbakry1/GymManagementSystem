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
        public AnalyticsViewModel? GetAnalyticsData()
        {
            var Sessions = _unitOfWork.SessionRepository.GetAll();
            return new AnalyticsViewModel()
            {
                TotalMembers = _unitOfWork.GetRepository<Member>().GetAll().Count(),
                ActiveMembers = _unitOfWork.GetRepository<Membership>().GetAll(X => X.Status == "Active").Count(),
                TotalTrainers = _unitOfWork.GetRepository<Trainer>().GetAll().Count(),
                UpcomingSessions = Sessions.Count(X => X.StartDate > DateTime.Now),
                OngoingSessions = Sessions.Count(X => X.StartDate <= DateTime.Now && X.EndDate > DateTime.Now),
                CompletedSessions = Sessions.Count(X => X.EndDate < DateTime.Now)
            };
        }
    }
}
