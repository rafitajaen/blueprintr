using Boilerplatr.Shared;
using CodeIn.Domain.Companies;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NodaTime;

namespace CodeIn.Web.Sections.TheFeatured;

[HtmlTargetElement(SectionTargets.TheFeatured)]
public class TheFeaturedComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public IEnumerable<Company> Featured { get; set; }

    [HtmlAttributeName]
    public int PublishedJobsCount { get; set; }

    [HtmlAttributeName]
    public Instant? LastUpdateAt { get; set; }
}
