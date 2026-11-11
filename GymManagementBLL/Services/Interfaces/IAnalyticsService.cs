using GymManagementBLL.ViewModels.AnalyticsViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel?> GetAnalyticsDataAsync(CancellationToken ct = default);
    }
}
