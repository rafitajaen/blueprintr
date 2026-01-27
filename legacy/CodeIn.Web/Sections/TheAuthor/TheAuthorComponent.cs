using Boilerplatr.Shared;
using CodeIn.Domain.Users;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Sections.TheAuthor;

[HtmlTargetElement(SectionTargets.TheAuthor)]
public class TheAuthorComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public User? Author { get; set; }
}
