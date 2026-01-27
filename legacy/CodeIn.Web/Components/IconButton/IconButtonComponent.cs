using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;
using CodeIn.Web.Components.Icon;

namespace CodeIn.Web.Components.IconButton;

[HtmlTargetElement(ComponentTargets.IconButton)]
public class IconButtonComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public IconTypes? Icon { get; set; }

    [HtmlAttributeName]
    public string? Type { get; set; }

    [HtmlAttributeName]
    public string? OnClick { get; set; }

    [HtmlAttributeName]
    public bool? IncludeLoader { get; set; }

}
