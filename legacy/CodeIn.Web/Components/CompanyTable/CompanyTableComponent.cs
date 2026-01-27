using Boilerplatr.Shared;
using CodeIn.Domain.Companies;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.CompanyTable;

[HtmlTargetElement(ComponentTargets.Company.Table)]
public class CompanyTableComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public IEnumerable<Company> Companies { get; set; }
}
