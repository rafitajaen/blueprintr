using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.Card;

[HtmlTargetElement(ComponentTargets.Card.Base)]
public class CardComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string? Href { get; set; }

    [HtmlAttributeName]
    public string? HideId { get; set; }

    [HtmlAttributeName]
    public string? EntityId { get; set; }

}
