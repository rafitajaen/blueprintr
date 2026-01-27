using Boilerplatr.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;
using CodeIn.Web.Components.Icon;

namespace CodeIn.Web.Components.ActionInput;

[HtmlTargetElement(ComponentTargets.ActionInput.Base)]
public class ActionInputComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string? PreText { get; set; }

    [HtmlAttributeName]
    public IconTypes? IconType { get; set; }

    [HtmlAttributeName]
    public string? Placeholder { get; set; }

    [HtmlAttributeName]
    public string? Label { get; set; }

    [HtmlAttributeName]
    public string Type { get; set; }

    [HtmlAttributeName]
    public string ActionText { get; set; }

    [HtmlAttributeName]
    public bool? InnerButton { get; set; }

    [HtmlAttributeName]
    public bool? Required { get; set; }

    [HtmlAttributeName]
    public string? Autocomplete { get; set; }

    [HtmlAttributeName]
    public string? Value { get; set; }

}
