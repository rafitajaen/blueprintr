using Boilerplatr.Shared;
using CodeIn.Domain.Companies;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.CompanyHeader;

[HtmlTargetElement(ComponentTargets.Company.Header)]
public class CompanyHeaderComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public Company Company { get; set; }

    [HtmlAttributeName]
    public bool? ShowExcerpt { get; set; }

    [HtmlAttributeName]
    public bool? IsTitle { get; set; }
}
