using Boilerplatr.Shared;
using CodeIn.Web.Components;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.AvatarImage;

[HtmlTargetElement(ComponentTargets.Avatar.Image)]
public class AvatarImageComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string? Src { get; set; }

    [HtmlAttributeName]
    public string? Alt { get; set; }

    [HtmlAttributeName]
    public string Initials { get; set; }

}
