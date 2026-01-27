using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.Icon;

[HtmlTargetElement(ComponentTargets.Icon)]
public class IconComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string? Href { get; set; }

    [HtmlAttributeName]
    public string? ActionId { get; set; }

    [HtmlAttributeName]
    public IconTypes Type { get; set; }

}
