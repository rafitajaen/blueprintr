using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.SocialGroup;

[HtmlTargetElement(ComponentTargets.SocialGroup)]
public class SocialGroupComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public Social? Social { get; set; }

    [HtmlAttributeName]
    public bool? HideEmail { get; set; }
}
