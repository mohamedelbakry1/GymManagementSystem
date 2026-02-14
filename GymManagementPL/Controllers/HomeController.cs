using System.Diagnostics;
using GymManagementBLL.Services.Interfaces;
using GymManagementPL.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class HomeController(IAnalyticsService _analyticsService) : Controller
    {
        public IActionResult Index()
        {
            var Data = _analyticsService.GetAnalyticsData();
            return View(Data);
        }
    }
}
