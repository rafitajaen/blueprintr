using Boilerplatr.Shared;
using CodeIn.Domain.Companies;
using CodeIn.Domain.Jobs;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.CompanyCard;

[HtmlTargetElement(ComponentTargets.Company.Card)]
public class CompanyCardComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public Company Company { get; set; }

    [HtmlAttributeName]
    public IEnumerable<Job>? Jobs { get; set; }

    [HtmlAttributeName]
    public string? HideId { get; set; }

    [HtmlAttributeName]
    public string? HeaderClass { get; set; }

    [HtmlAttributeName]
    public Instant Now { get; set; }
}
