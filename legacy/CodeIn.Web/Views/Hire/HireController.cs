using Microsoft.AspNetCore.Mvc;
using Boilerplatr.Mvc;
using CodeIn.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Boilerplatr.MagicLinks;
using Boilerplatr.Tenants;

namespace CodeIn.Web.Views.Hire;

[ApiController]
public class HireController
(
) : BaseController
{
    internal class HireViewModel : BaseViewModel
    {
        public string ContactEmail { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Hire.Base)]
    public IActionResult Hire()
    {
        var model = new HireViewModel()
        {
            ContactEmail = HttpContext.GetTenant().ContactEmail
        };

        return View(model);
    }

    internal class LeadSubmissionViewModel : BaseViewModel
    {
        public string SupportEmail { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Hire.LeadSubmission)]
    public IActionResult LeadSubmission()
    {
        var model = new LeadSubmissionViewModel()
        {
            SupportEmail = HttpContext.GetTenant().SupportEmail
        };

        return View(model);
    }

    internal class LeadVerificationViewModel : BaseViewModel
    {
        public string SupportEmail { get; set; }
        public bool WasVerifiedSuccessfully { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Hire.LeadVerification)]
    public async Task<IActionResult> LeadVerificationAsync
    (
        [FromQuery] string? code,
        [FromQuery] string? uid,
        [FromServices] CodeInDbContext db,
        CancellationToken cancellationToken = default
    )
    {
        MagicLink? link = null;

        var success = false;
        var now = SystemClock.Instance.GetCurrentInstant();

        if (!string.IsNullOrWhiteSpace(code) && Ulid.TryParse(code, out var ulid))
        {
            link = await db.MagicLinks.FirstOrDefaultAsync(l => l.Id == ulid, cancellationToken);
        }

        if (link is not null && link.EntityId is not null && link.IsValid(now))
        {
            db.MagicLinks.Remove(link);

            var lead = await db.Leads
                .FirstOrDefaultAsync(n => n.Id == link.EntityId, cancellationToken);

            if (lead is not null)
            {
                lead.VerifiedAt = now;
                success = true;
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        var tenant = HttpContext.GetTenant();
        
        var model = new LeadVerificationViewModel()
        {
            SupportEmail = tenant.SupportEmail,
            WasVerifiedSuccessfully = success
        };

        return View(model);
    }
}
