using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.AnchorButton;

[HtmlTargetElement(ComponentTargets.AnchorButton)]
public class AnchorButtonComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string Href { get; set; }

    [HtmlAttributeName]
    public string? Rel { get; set; }

    [HtmlAttributeName]
    public bool? TrackActive { get; set; }

}
