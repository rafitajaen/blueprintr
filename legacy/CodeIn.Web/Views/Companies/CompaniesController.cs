using Microsoft.AspNetCore.Mvc;
using Boilerplatr.Mvc;
using CodeIn.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Boilerplatr.Shared;
using CodeIn.Application;
using Microsoft.AspNetCore.Authorization;
using CodeIn.Domain.Topics;
using Boilerplatr.Tenants;

namespace CodeIn.Web.Views.Companies;

internal class CompanyProfileViewModel : BaseViewModel
{
    public Company Company { get; set; }
}

[ApiController]
public class CompaniesController
(
    CodeInDbContext dbContext
) : BaseController
{
    [AllowAnonymous]
    [HttpGet(PageRoutes.Companies.Profile)]
    public async Task<IActionResult> CompanyProfileAsync([FromRoute] string? companySlug, CancellationToken cancellationToken = default)
    {
        Company? company = null;

        if (!string.IsNullOrWhiteSpace(companySlug))
        {
            var slug = new Slug(companySlug);
            company = await dbContext.Companies.AsNoTracking()
                        .Include(x => x.Jobs)
                        .ThenInclude(x => x.Topics)
                        .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        if (company is null)
        {
            return View(PageRoutes.NotFound);
        }

        var model = new CompanyProfileViewModel()
        {
            Company = company
        };

        return View(model);
    }

    public class CompaniesViewModel : BaseViewModel
    {
        public IEnumerable<Company> Featured { get; set; } = [];
        public IEnumerable<Company> Transparent { get; set; } = [];
        public IEnumerable<Company> Remote { get; set; } = [];
        public IEnumerable<Company> International { get; set; } = [];
        public IEnumerable<Company> AI { get; set; } = [];
        public IEnumerable<Company> Entertainment { get; set; } = [];
        public IEnumerable<Company> Automotive { get; set; } = [];
        public IEnumerable<Company> Aviation { get; set; } = [];
        public IEnumerable<Company> Blockchain { get; set; } = [];
        public IEnumerable<Company> Construction { get; set; } = [];
        public IEnumerable<Company> Consulting { get; set; } = [];
        public IEnumerable<Company> DataManagement { get; set; } = [];
        public IEnumerable<Company> DevTools { get; set; } = [];
        public IEnumerable<Company> Ecommerce { get; set; } = [];
        public IEnumerable<Company> Education { get; set; } = [];
        public IEnumerable<Company> Energy { get; set; } = [];
        public IEnumerable<Company> Enterprise { get; set; } = [];
        public IEnumerable<Company> Events { get; set; } = [];
        public IEnumerable<Company> Finance { get; set; } = [];
        public IEnumerable<Company> Food { get; set; } = [];
        public IEnumerable<Company> Gaming { get; set; } = [];
        public IEnumerable<Company> Healthcare { get; set; } = [];
        public IEnumerable<Company> Hospitality { get; set; } = [];
        public IEnumerable<Company> HR { get; set; } = [];
        public IEnumerable<Company> Manufacturing { get; set; } = [];
        public IEnumerable<Company> Marketing { get; set; } = [];
        public IEnumerable<Company> Marketplace { get; set; } = [];
        public IEnumerable<Company> RealState { get; set; } = [];
        public IEnumerable<Company> Robotics { get; set; } = [];
        public IEnumerable<Company> Saas { get; set; } = [];
        public IEnumerable<Company> Security { get; set; } = [];
        public IEnumerable<Company> SocialNetworking { get; set; } = [];
        public IEnumerable<Company> Translations { get; set; } = [];
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Companies.Base)]
    public async Task<IActionResult> CompaniesAsync(CancellationToken cancellationToken = default)
    {
        var tenant = HttpContext.GetTenant();
        var now = SystemClock.Instance.GetCurrentInstant();
        
        var companies = await dbContext.Companies
            .AsNoTracking()
            .Where(x => x.TenantId == tenant.Id && x.Jobs.Any(j => j.ApplyBefore != null && j.ApplyBefore >= now))
            .Include(x => x.Topics)
            .ToListAsync(cancellationToken);

        var model = new CompaniesViewModel()
        {
            Featured = companies.Where(x => x.IsFeatured || x.Topics.Any(t => t.Slug == TopicSlugs.Badges.Featured)),
            Transparent = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Badges.TransparentCompensation)),
            Remote = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Badges.FullyRemote)),
            International = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Badges.HiresInternationally)),
            AI = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.AI)),
            Entertainment = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Entertainment)),
            Automotive = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Automotive)),
            Aviation = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Automotive || t.Slug == TopicSlugs.Sectors.Aviation)),
            Blockchain = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Crypto)),
            Construction = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Construction)),
            DataManagement = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.DataManagementTool)),
            Consulting = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Consulting || t.Slug == TopicSlugs.Sectors.Services)),
            DevTools = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.DeveloperTools || t.Slug == TopicSlugs.Sectors.Api)),
            Ecommerce = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Ecommerce || t.Slug == TopicSlugs.Sectors.Retail)),
            Education = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Education)),
            Energy = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Energy)),
            Enterprise = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.EnterpriseSoftware)),
            Events = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Events || t.Slug == TopicSlugs.Sectors.Ticketing)),
            Finance = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Finance || t.Slug == TopicSlugs.Sectors.Payments)),
            Food = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.FoodDrink)),
            Gaming = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Gaming)),
            Healthcare = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Healthcare)),
            Hospitality = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Hospitality)),
            HR = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.HumanResources)),
            Manufacturing = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Manufacturing)),
            Marketing = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Marketing || t.Slug == TopicSlugs.Sectors.Sales)),
            Marketplace = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Manufacturing)),
            RealState = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.RealEstate)),
            Robotics = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Robotics)),
            Saas = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Saas)),
            Security = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Security)),
            SocialNetworking = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.SocialNetworking)),
            Translations = companies.Where(x => x.Topics.Any(t => t.Slug == TopicSlugs.Sectors.Translations)),
        };

        return View(model);
    }
}
