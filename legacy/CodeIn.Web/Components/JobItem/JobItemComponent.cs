using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;
using CodeIn.Domain.Jobs;

namespace CodeIn.Web.Components.JobItem;

[HtmlTargetElement(ComponentTargets.JobItem)]
public class JobItemComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string Href { get; set; }

    [HtmlAttributeName]
    public Job Job { get; set; }

    [HtmlAttributeName]
    public Instant Now { get; set; }

    [HtmlAttributeName]
    public string? HideId { get; set; }
}
