using Microsoft.AspNetCore.Mvc;
using AgenticContextEngine.Services;

namespace AgenticContextEngine.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            var model = _dashboardService.ObterDashboard();
            return View(model);
        }
    }

}
