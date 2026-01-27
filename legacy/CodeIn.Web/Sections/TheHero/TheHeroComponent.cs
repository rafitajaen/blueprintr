using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Sections.TheHero;

[HtmlTargetElement(SectionTargets.TheHero)]
public class TheHeroComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public int LatestJobCount { get; set; }
}
