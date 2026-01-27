using Microsoft.AspNetCore.Mvc;
using Boilerplatr.Mvc;
using CodeIn.Application;
using Boilerplatr.MagicLinks;
using Microsoft.EntityFrameworkCore;
using Boilerplatr.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Boilerplatr.Tenants;

namespace CodeIn.Web.Views.Auth;

[ApiController]
public class AuthController() : BaseController
{
    internal class LoginViewModel : BaseViewModel
    {
        public string SupportEmail { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Auth.Login)]
    public IActionResult Login
    (
        [FromQuery] string? returnUrl,
        [FromQuery] string? username,
        CancellationToken cancellationToken = default
    )
    {
        if (HttpContext.IsAuthenticated())
        {
            return Redirect(PageRoutes.Dashboard.Base);
        }

        var model = new LoginViewModel()
        {
            SupportEmail = HttpContext.GetTenant().SupportEmail
        };

        return View(model);
    }

    internal class MagicLinkViewModel : BaseViewModel
    {
        public string RedirectUrl { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Auth.MagicLink)]
    public async Task<IActionResult> MagicLinkAsync
    (
        [FromQuery] string? code,
        [FromServices] CodeInDbContext db,
        [FromServices] IAuthenticationService auth,
        CancellationToken cancellationToken = default
    )
    {
        MagicLink? link = null;
        var returnUrl = PageRoutes.Auth.Login;

        if (!string.IsNullOrWhiteSpace(code) && Ulid.TryParse(code, out var ulid))
        {
            link = await db.MagicLinks.FirstOrDefaultAsync(l => l.Id == ulid, cancellationToken);
        }

        if (link is not null && link.IsValid(SystemClock.Instance.GetCurrentInstant()))
        {
            db.MagicLinks.Remove(link);
            await db.SaveChangesAsync(cancellationToken);

            var user = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == link.UserId && u.NormalizedEmail == link.Email, cancellationToken);
                
            if (user is not null && await auth.TryLogin(HttpContext, user.Id.ToString(), user.NormalizedEmail, user.RoleId, cancellationToken))
            {
                returnUrl = !string.IsNullOrWhiteSpace(link.ReturnUrl) ? link.ReturnUrl : PageRoutes.Dashboard.Base;
            }
        }

        var model = new MagicLinkViewModel()
        {
            RedirectUrl = returnUrl
        };

        return View(model);
    }

    internal class VerifyViewModel : BaseViewModel
    {
        public string? Email { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Auth.Verify)]
    public IActionResult Verify([FromQuery] string? email)
    {
        var model = new VerifyViewModel()
        {
            Email = email
        };

        return View(model);
    }
}
