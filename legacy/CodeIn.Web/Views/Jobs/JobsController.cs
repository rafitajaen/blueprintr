using Microsoft.AspNetCore.Mvc;
using Boilerplatr.Mvc;
using Boilerplatr.Extensions;
using CodeIn.Domain.Jobs;
using CodeIn.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Boilerplatr.Shared;
using CodeIn.Application;
using Microsoft.AspNetCore.Authorization;
using Humanizer;
using CodeIn.Domain.Topics;
using Boilerplatr.Tenants;

namespace CodeIn.Web.Views.Jobs;

[ApiController]
public class JobsController
(
    CodeInDbContext dbContext
) : BaseController
{
    internal class JobsViewModel : BaseViewModel
    {
        public IEnumerable<Topic> Topics { get; set; } = [];
        public IEnumerable<string> Checked { get; set; } = [];
        public int FilterCount { get; set; }
        public bool ShowTitle { get; set; }
        public IEnumerable<Company> Companies { get; set; } = [];
    }
    
    [AllowAnonymous]
    [HttpGet(PageRoutes.Jobs.Base)]
    public async Task<IActionResult> JobsAsync
    (
        [FromQuery] string? Modality,
        [FromQuery] string? Seniority,
        [FromQuery] string? Salary,
        [FromQuery] string? Technology,
        [FromQuery] string? Role,
        [FromQuery] string? Benefit,
        CancellationToken cancellationToken = default
    )
    {
        var tenant = HttpContext.GetTenant();
        var now = SystemClock.Instance.GetCurrentInstant();

        IEnumerable<string> selected =
        [
            ..Modality?.Split('.', StringSplitOptions.RemoveEmptyEntries) ?? [],
            ..Seniority?.Split('.', StringSplitOptions.RemoveEmptyEntries) ?? [],
            ..Technology?.Split('.', StringSplitOptions.RemoveEmptyEntries) ?? [],
            ..Salary?.Split('.', StringSplitOptions.RemoveEmptyEntries) ?? [],
            ..Role?.Split('.', StringSplitOptions.RemoveEmptyEntries) ?? [],
            ..Benefit?.Split('.', StringSplitOptions.RemoveEmptyEntries) ?? [],
        ];

        var slugs = selected.Select(s => new Slug(s));

        var query = dbContext.Companies
            .AsNoTracking()
            .Where(x => x.TenantId == tenant.Id && x.Jobs.Any(j => j.ApplyBefore != null && j.ApplyBefore >= now))
            .Include(c => c.Jobs.Where(j => j.ApplyBefore != null && j.ApplyBefore >= now))
                .ThenInclude(j => j.Topics)
            .AsQueryable();

        if (selected.Any())
        {
            query = query.Where(company => company.Jobs.Any(job => job.Topics.Any(tag => slugs.Contains(tag.Slug))));
        }

        if (Enums.TryParseInt<SalaryRange>(Salary, ignoreCase: true, out var salary) && salary > 0)
        {
            query = query.Where(company => company.Jobs.Any
            (
                job => job.Compensation.MinQuantity.HasValue && job.Compensation.MinQuantity.Value >= salary
                    || !job.Compensation.MinQuantity.HasValue && job.Compensation.MaxQuantity.HasValue && job.Compensation.MaxQuantity.Value >= salary
            ));
        }

        var companies = await query.ToListAsync(cancellationToken);

        var topics = await dbContext.Topics
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var model = new JobsViewModel()
        {
            Topics = topics,
            FilterCount = companies.Sum(c => c.Jobs.Count),
            ShowTitle = true,
            Companies = companies,
            Checked = selected
        };

        return View(model);
    }

    public class JobEntryViewModel : BaseViewModel
    {
        public Job Job { get; set; }
        public Company Company { get; set; }
        public IEnumerable<Company> Related { get; set; } = [];
        public string SuscribersMetric { get; set; } = string.Empty;
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Jobs.Entry)]
    public async Task<IActionResult> JobEntryAsync
    (
        [FromRoute] string? companySlug,
        [FromRoute] string? jobSlug,
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrWhiteSpace(companySlug) && !string.IsNullOrWhiteSpace(jobSlug))
        {
            var tenant = HttpContext.GetTenant();
            var slugCompany = new Slug(companySlug);
            var slugJob = new Slug(jobSlug);

            var exists = await dbContext.Companies
                .AsNoTracking()
                .AnyAsync
                (
                    company => company.TenantId == tenant.Id && company.Slug == slugCompany && company.Jobs.Any(job => job.Slug == slugJob),
                    cancellationToken
                );

            Company? company = null;

            if (exists)
            {
                var now = SystemClock.Instance.GetCurrentInstant();

                company = await dbContext.Companies
                        .AsNoTracking()
                        .Include(x => x.Jobs.Where(job => job.ApplyBefore != null && job.ApplyBefore > now))
                            .ThenInclude(x => x.Topics)
                        .FirstOrDefaultAsync(x => x.TenantId == tenant.Id && x.Slug == slugCompany, cancellationToken);
            }

            if (company is not null)
            {
                var suscribers = await dbContext.Newsletters
                    .AsNoTracking()
                    .CountAsync(cancellationToken);
                
                var model = new JobEntryViewModel()
                {
                    Company = company,
                    Job = company.Jobs.First(x => string.Equals(x.Slug.Value, slugJob.Value, StringComparison.OrdinalIgnoreCase)),
                    SuscribersMetric = suscribers.ToMetric(),
                };

                return View(model);
            }
        }

        return View(PageRoutes.NotFound);
    }

    public class JobsByTopicViewModel : BaseViewModel
    {
        public Topic Topic { get; set; }
        public int FilterCount { get; set; }
        public IEnumerable<Company> Companies { get; set; } = [];
        public IEnumerable<Company> Related { get; set; } = [];
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Jobs.ByTopic)]
    public async Task<IActionResult> TopicsAsync
    (
        [FromRoute] string? topicSlug,
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrWhiteSpace(topicSlug))
        {
            var slug = new Slug
            (
                value: PageRoutes.Jobs.ByTopic.Replace("{topicSlug}", topicSlug)
            );

            var tenant = HttpContext.GetTenant();

            var topic = await dbContext.Topics
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

            var companies = await dbContext.Companies
                    .AsNoTracking()
                    .Include(x => x.Jobs.Where(j => j.Topics.Any(t => t.Slug == slug)))
                        .ThenInclude(j => j.Topics)
                    .Where(c => c.TenantId == tenant.Id && c.Jobs.Any(j => j.Topics.Any(t => t.Slug == slug)))
                    .ToListAsync(cancellationToken);

            if (topic is not null)
            {
                var model = new JobsByTopicViewModel()
                {
                    Topic = topic,
                    FilterCount = companies.Sum(c => c.Jobs.Count),
                    Companies = companies,
                };

                return View(model);
            }
        }

        return View(PageRoutes.NotFound);
    }

    public class JobApplicationViewModel : BaseViewModel
    {
        public Job Job { get; set; }
        public string SupportEmail { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Jobs.Application)]
    public async Task<IActionResult> JobApplicationAsync
    (
        [FromRoute] string? companySlug,
        [FromRoute] string? jobSlug,
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrWhiteSpace(companySlug) && !string.IsNullOrWhiteSpace(jobSlug))
        {
            var tenant = HttpContext.GetTenant();
            var job = await dbContext.Jobs
                        .AsNoTracking()
                        .Include(x => x.Topics)
                        .Include(x => x.Company)
                        .FirstOrDefaultAsync(job => job.Company.TenantId == tenant.Id && job.Slug == new Slug(jobSlug) && job.Company.Slug == new Slug(companySlug), cancellationToken);

            if (job is not null)
            {
                if (!string.IsNullOrWhiteSpace(job.ApplicationUrl))
                {
                    return Redirect(job.ApplicationUrl);
                }

                var model = new JobApplicationViewModel()
                {
                    Job = job,
                    SupportEmail = HttpContext.GetTenant().SupportEmail
                };

                return View(model);
            }
        }

        return View(PageRoutes.NotFound);
    }

    public class JobQuestionViewModel : BaseViewModel
    {
        public Job Job { get; set; }
        public string SupportEmail { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Jobs.Question)]
    public async Task<IActionResult> JobQuestionAsync
    (
        [FromRoute] string? companySlug,
        [FromRoute] string? jobSlug,
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrWhiteSpace(companySlug) && !string.IsNullOrWhiteSpace(jobSlug))
        {
            var tenant = HttpContext.GetTenant();
            var job = await dbContext.Jobs
                        .AsNoTracking()
                        .Include(x => x.Topics)
                        .Include(x => x.Company)
                        .FirstOrDefaultAsync(job => job.Company.TenantId == tenant.Id && job.Slug == new Slug(jobSlug) && job.Company.Slug == new Slug(companySlug), cancellationToken);

            if (job is not null)
            {
                var model = new JobQuestionViewModel()
                {
                    Job = job,
                    SupportEmail = HttpContext.GetTenant().SupportEmail
                };

                return View(model);
            }
        }

        return View(PageRoutes.NotFound);
    }

    public class JobQuestionSubmittedViewModel : BaseViewModel
    {
        public Job Job { get; set; }
        public string? Email { get; set; }
        public IEnumerable<Company> Related { get; set; } = [];
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Jobs.QuestionSubmitted)]
    public async Task<IActionResult> JobQuestionSubmittedAsync
    (
        [FromRoute] string? companySlug,
        [FromRoute] string? jobSlug,
        [FromQuery] string? email,
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrWhiteSpace(companySlug) && !string.IsNullOrWhiteSpace(jobSlug))
        {
            var tenant = HttpContext.GetTenant();
            var job = await dbContext.Jobs
                        .AsNoTracking()
                        .Include(x => x.Topics)
                        .Include(x => x.Company)
                        .FirstOrDefaultAsync(job => job.Company.TenantId == tenant.Id && job.Slug == new Slug(jobSlug) && job.Company.Slug == new Slug(companySlug), cancellationToken);

            if (job is not null)
            {
                var topics = job.Topics.Where(t => t.Kind == TopicKind.Technology).Select(t => t.Id).ToList();
                var relatedJobs = await dbContext.Jobs
                    .AsNoTracking()
                    .Include(j => j.Topics)
                    .Include(j => j.Company)
                    .Where(j => j.Company.TenantId == tenant.Id && j.Id != job.Id && j.Topics.Any(t => topics.Contains(t.Id)))
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var companies = new Dictionary<Guid7, Company>();

                foreach (var related in relatedJobs)
                {
                    if (!companies.TryGetValue(related.Company.Id, out var company))
                    {
                        company = related.Company;
                        company.Jobs = [];
                        companies[company.Id] = company;
                        related.Company = company;
                    }

                    company.Jobs.Add(related);
                }

                var model = new JobQuestionSubmittedViewModel()
                {
                    Job = job,
                    Email = email,
                    Related = companies.Values
                };

                return View(model);
            }
        }

        return View(PageRoutes.NotFound);
    }
}
