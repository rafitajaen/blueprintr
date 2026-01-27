using Microsoft.AspNetCore.Mvc;
using Boilerplatr.Mvc;
using CodeIn.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using CodeIn.Application;
using Microsoft.AspNetCore.Authorization;
using Humanizer;
using System.Diagnostics;
using CodeIn.Domain.Topics;
using CodeIn.Domain.Users;
using Boilerplatr.Shared;
using Boilerplatr.Identities;
using Boilerplatr.Tenants;

namespace CodeIn.Web.Views.Pages;

[ApiController]
public class PagesController
(
    CodeInDbContext dbContext
) : BaseController
{
    internal class HomeViewModel : BaseViewModel
    {
        public Instant? LastUpdateAt { get; set; }
        public int LatestJobCount { get; set; }
        public int PublishedJobsCount { get; set; }
        public string SuscribersMetric { get; set; }
        public string NewsletterEmail { get; set; }
        public IEnumerable<Speciality> Specialities { get; set; }
        public IEnumerable<Company> FeaturedCompanies { get; set; } = [];
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Home)]
    public async Task<IActionResult> HomeAsync(CancellationToken cancellationToken = default)
    {
        var tenant = HttpContext.GetTenant();

        var suscribers = await dbContext.Newsletters
            .AsNoTracking()
            .Where(x => x.TenantId == tenant.Id)
            .CountAsync(cancellationToken);

        var specialities = await dbContext.Topics
            .AsNoTracking()
            .Where(t => t.Kind == TopicKind.Technology && t.DeletedAt == null)
            .Select(t => new
            {
                t.Slug,
                t.Name,
                t.Jobs.Count
            })
            .ToListAsync(cancellationToken);

        var now = SystemClock.Instance.GetCurrentInstant();
        var week =  now - Duration.FromDays(7);

        var model = new HomeViewModel()
        {
            LastUpdateAt = await dbContext.Jobs
                .AsNoTracking()
                .Where(x => x.Company.TenantId == tenant.Id && x.PublishedAt != null)
                .Select(x => x.PublishedAt)
                .MaxAsync(cancellationToken),

            LatestJobCount = await dbContext.Jobs
                .AsNoTracking()
                .Where(x => x.Company.TenantId == tenant.Id && x.PublishedAt >= week)
                .CountAsync(cancellationToken),

            PublishedJobsCount = await dbContext.Jobs
                .AsNoTracking()
                .Where(x => x.ApplyBefore > now)
                .CountAsync(cancellationToken),

            FeaturedCompanies = await dbContext.Companies
                .AsNoTracking()
                .Where(x => x.TenantId == tenant.Id)
                .Include(x => x.Jobs)
                .ThenInclude(x => x.Topics)
                .OrderByDescending(x => x.UpdatedAt)
                .Take(5)
                .ToListAsync(cancellationToken),

            SuscribersMetric = suscribers.ToMetric(),
            NewsletterEmail = HttpContext.GetTenant().NewsletterEmail,
            Specialities = specialities.Select(x => new Speciality(x.Slug.Value, x.Name, x.Count)).OrderByDescending(x => x.Count),
        };

        return View(model);
    }

    internal class AboutViewModel : BaseViewModel
    {
        public User? Author { get; set; }
        public string SuscribersMetric { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.About)]
    public async Task<IActionResult> AboutAsync(CancellationToken cancellationToken = default)
    {
        var suscribers = await dbContext.Newsletters.CountAsync(cancellationToken);

        var model = new AboutViewModel()
        {
            Author = await dbContext.Users.FirstAsync(u => u.RoleId == Role.SuperAdmin.Name, cancellationToken),
            SuscribersMetric = suscribers.ToMetric(),
        };

        return View(model);
    }

    internal class ContactViewModel : BaseViewModel
    {
        public string ContactEmail { get; set; }
        public Social Social { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Contact)]
    public IActionResult Contact()
    {
        var model = new ContactViewModel()
        {
            ContactEmail = HttpContext.GetTenant().ContactEmail,
            Social = HttpContext.GetTenant().Social
        };

        return View(model);
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Privacy)]
    public IActionResult PrivacyAsync(CancellationToken cancellationToken = default)
    {
        return View(new BaseViewModel());
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Terms)]
    public IActionResult TermsAsync(CancellationToken cancellationToken = default)
    {
        return View(new BaseViewModel());
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Cookies)]
    public IActionResult CookiesAsync(CancellationToken cancellationToken = default)
    {
        return View(new BaseViewModel());
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    [HttpGet(PageRoutes.Error)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
