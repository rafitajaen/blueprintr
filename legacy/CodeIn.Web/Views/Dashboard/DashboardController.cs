using Microsoft.AspNetCore.Mvc;
using Boilerplatr.Mvc;
using CodeIn.Domain.Users;
using CodeIn.Application.Users;

namespace CodeIn.Web.Views.Dashboard;

[ApiController]
public class DashboardController() : BaseController
{
    [HttpGet(PageRoutes.Dashboard.Base)]
    public IActionResult Overview()
    {
        return View(new BaseViewModel());
    }

    [HttpGet(PageRoutes.Dashboard.Jobs)]
    public IActionResult Jobs()
    {
        return View(new BaseViewModel());
    }

    internal class ProfileViewModel : BaseViewModel
    {
        public User User { get; set; }
    }

    [HttpGet(PageRoutes.Dashboard.Profile)]
    public IActionResult Profile
    (
        CancellationToken cancellationToken = default
    )
    {
        var model = new ProfileViewModel()
        {
            User = HttpContext.GetRequiredUser()
        };

        return View(model);
    }
}
