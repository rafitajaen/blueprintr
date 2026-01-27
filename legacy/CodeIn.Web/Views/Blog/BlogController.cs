using Microsoft.AspNetCore.Mvc;
using Boilerplatr.Mvc;
using Microsoft.EntityFrameworkCore;
using Boilerplatr.Shared;
using CodeIn.Application;
using Microsoft.AspNetCore.Authorization;
using CodeIn.Domain.Articles;
using Boilerplatr.Utils;
using Humanizer;

namespace CodeIn.Web.Views.Blog;

[ApiController]
public class BlogController
(
    CodeInDbContext dbContext
) : BaseController
{
    internal class ArticlesViewModel : BaseViewModel
    {
        public Instant Now { get; set; }
        public Article Article { get; set; }
        public string SuscribersMetric { get; set; }
    }

    [AllowAnonymous]
    [HttpGet(PageRoutes.Blog.Article)]
    public async Task<IActionResult> ArticlesAsync
    (
        [FromRoute] string? categorySlug,
        [FromRoute] string? articleSlug,
        CancellationToken cancellationToken = default
    )
    {
        Article? article = null;

        if (!string.IsNullOrWhiteSpace(categorySlug) && !string.IsNullOrWhiteSpace(articleSlug))
        {
            var categoryKey = new Slug($"/{categorySlug}".NormalizeUrl());
            var articleKey = new Slug($"/{articleSlug}".NormalizeUrl());

            article = await dbContext.Articles
                        .AsNoTracking()
                        .Include(x => x.Author)
                        .Include(x => x.Category)
                        .FirstOrDefaultAsync(x => x.Category.Slug == categoryKey && x.Slug == articleKey, cancellationToken);
        }

        if (article is null)
        {
            return View(PageRoutes.NotFound);
        }

        var suscribers = await dbContext.Newsletters.CountAsync(cancellationToken);

        var model = new ArticlesViewModel()
        {
            Article = article,
            SuscribersMetric = suscribers.ToMetric(),
            Now = SystemClock.Instance.GetCurrentInstant(),
        };

        return View(model);
    }
}
