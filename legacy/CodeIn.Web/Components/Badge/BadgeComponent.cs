using Boilerplatr.Shared;
using CodeIn.Web.Components;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.Badge;

[HtmlTargetElement(ComponentTargets.Badge)]
public class BadgeComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string? Href { get; set; }

}
