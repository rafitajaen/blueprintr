using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;
using CodeIn.Web.Components.Icon;

namespace CodeIn.Web.Components.Highlight;

[HtmlTargetElement(ComponentTargets.Highlight)]
public class HighlightComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public IconTypes? Icon { get; set; }

    [HtmlAttributeName]
    public string? Text { get; set; }
}
