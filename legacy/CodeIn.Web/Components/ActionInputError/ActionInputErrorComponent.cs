using Boilerplatr.Shared;
using CodeIn.Web.Components;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeIn.Web.Components.ActionInputError;

[HtmlTargetElement(ComponentTargets.ActionInput.Error)]
public class ActionInputErrorComponent : ComponentBaseTagHelper
{
    [HtmlAttributeName]
    public string Type { get; set; }
}
