using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.Logo;

[HtmlTargetElement(ComponentTargets.Logo)]
public class LogoComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string? Href { get; set; }

    [HtmlAttributeName]
    public LogoTypes Type { get; set; }

}
