using System.Diagnostics;
using GymManagementBLL.Services.Interfaces;
using GymManagementPL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize]
    public class HomeController(IAnalyticsService _analyticsService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var Data = await _analyticsService.GetAnalyticsData();
            return View(Data);
        }
    }
}
