using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;
using CodeIn.Web.Components.Icon;

namespace CodeIn.Web.Components.ActionSelect;

[HtmlTargetElement(ComponentTargets.ActionSelect)]
public class ActionSelectComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public IconTypes? IconType { get; set; }

    [HtmlAttributeName]
    public string? Placeholder { get; set; }

    [HtmlAttributeName]
    public string? Label { get; set; }

    [HtmlAttributeName]
    public string ActionText { get; set; }

    [HtmlAttributeName]
    public bool? InnerButton { get; set; }

    [HtmlAttributeName]
    public string? Autocomplete { get; set; }

    [HtmlAttributeName]
    public string? Value { get; set; }

    [HtmlAttributeName]
    public Dictionary<string, string> Options { get; set; } = [];

}
