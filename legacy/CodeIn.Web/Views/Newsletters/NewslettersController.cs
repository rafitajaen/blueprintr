using Microsoft.AspNetCore.Mvc;
using Boilerplatr.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeIn.Application;
using Boilerplatr.MagicLinks;
using Microsoft.AspNetCore.Authorization;
using Boilerplatr.Shared;
using Boilerplatr.Tenants;

namespace CodeIn.Web.Views.Newsletters;

[ApiController]
public class NewslettersController
(
) : BaseController
{
    public class NewslettersViewModel : BaseViewModel
    {
        public required Social Social { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Newsletters.Base)]
    public IActionResult Newsletters()
    {
        var model = new NewslettersViewModel()
        {
            Social = HttpContext.GetTenant().Social
        };

        return View(model);
    }

    public class NewslettersVerificationViewModel : BaseViewModel
    {
        public Tenant Tenant { get; set; }
    }
    
    [AllowAnonymous]
    [HttpGet(PageRoutes.Newsletters.Verification)]
    public async Task<IActionResult> NewslettersVerificationAsync
    (
        [FromQuery] string? code,
        [FromServices] CodeInDbContext db,
        CancellationToken cancellationToken = default
    )
    {
        MagicLink? link = null;
        var now = SystemClock.Instance.GetCurrentInstant();

        if (!string.IsNullOrWhiteSpace(code) && Ulid.TryParse(code, out var ulid))
        {
            link = await db.MagicLinks.FirstOrDefaultAsync(l => l.Id == ulid, cancellationToken);
        }

        if (link is not null && link.EntityId is not null && link.IsValid(now))
        {
            db.MagicLinks.Remove(link);

            var newsletter = await db.Newsletters
                .FirstOrDefaultAsync(n => n.IdentityId == link.UserId && n.Id == link.EntityId, cancellationToken);

            if (newsletter is not null)
            {
                newsletter.VerifiedAt = now;
                newsletter.UnsuscribedAt = null;
                newsletter.PausedUntil = null;
            }

            await db.SaveChangesAsync(cancellationToken);
        }
        
        var model = new NewslettersVerificationViewModel()
        {
            Tenant = HttpContext.GetTenant()
        };

        return View(model);
    }
}
