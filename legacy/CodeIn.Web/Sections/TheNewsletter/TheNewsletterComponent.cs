using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Sections.TheNewsletter;

[HtmlTargetElement(SectionTargets.TheNewsletter)]
public class TheNewsletterComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string Metric { get; set; }

    [HtmlAttributeName]
    public bool? ShowMetrics { get; set; }
}
