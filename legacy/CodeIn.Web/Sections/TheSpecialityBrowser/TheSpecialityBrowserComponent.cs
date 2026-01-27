using Boilerplatr.Shared;
using CodeIn.Domain.Topics;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Sections.TheSpecialityBrowser;

[HtmlTargetElement(SectionTargets.TheSpecialityBrowser)]
public class TheSpecialityBrowserComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public IEnumerable<Speciality> Specialities { get; set; }
}
